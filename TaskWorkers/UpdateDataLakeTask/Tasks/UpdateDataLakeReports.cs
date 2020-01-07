using Azure.Storage.Files.DataLake;
using Microsoft.Azure.Cosmos.Table;
using Shared.Models.QueueMessages;
using Shared.Persistence.Storage.Application;
using Shared.Persistence.Storage.DataLake;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UpdateDataLakeTask.Tasks
{
    class UpdateDataLakeReports
    {
        public async static Task<bool> RunAsync(IDataLakeStorageSharedKey dataLakeStorageSharedKey, ContentViewsQueueMessage message)
        {
            // This task updates .csv files in the data lake file system with new view statistics
            DataLakeServiceClient serviceClient = new DataLakeServiceClient(new Uri(dataLakeStorageSharedKey.URIs.DistributedFileSystem), dataLakeStorageSharedKey.SharedKeyCredentials); ;

            // Connect to namespace
            var fileSystemName = "content";
            DataLakeFileSystemClient filesystem = serviceClient.GetFileSystemClient(fileSystemName);
            //filesystem.Create(); //<-- Should already be created via Azure 


            // We get the count of each list and append to the view count for each content
            string contentId = message.ContentId;
            string accountId = message.AccountId;
            int newViews = message.Views;

            // Create DataLake Directory for the day of the view data
            var dateTimeViewsPath = contentId + "/views/" + message.Year + "/" + message.Month + "/" + message.Day;
            DataLakeDirectoryClient directory = filesystem.GetDirectoryClient(dateTimeViewsPath);
            directory.Create();

            // Create a DataLake File for the hour using a DataLake Directory
            var fileName = message.Hour + ".csv";
            DataLakeFileClient file = directory.GetFileClient(fileName);

            //Create if not exists
            var fileExists = false;

            try
            {
                var fileProperties = file.GetProperties();
                if (fileProperties != null)
                {
                    fileExists = true;
                }
            }
            catch
            {
                file.Create();
            }

            var fileData = new StringBuilder();

            if (!fileExists)
            {
                // File is new so add csv column names
                fileData.Append("ContentID, ViewCount, AccountId, Date, Time");
                fileData.Append("\r\n");
            }

            // Append new view info for the minute
            fileData.Append($"{contentId}, {newViews}, {accountId}, {message.Month}/{message.Day}/{message.Year}, {message.Hour}:{message.Minute}");
            fileData.Append("\r\n");

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] dataToWrite = Encoding.UTF8.GetBytes(fileData.ToString());

                ms.Write(dataToWrite, 0, dataToWrite.Length);
                ms.Position = 0;

                long appendPosition = 0;
                try
                {
                    var fileContents = file.Read();
                    appendPosition = fileContents.Value.ContentLength;
                }
                catch (Exception e)
                {

                }

                await file.AppendAsync(ms, appendPosition);

                var flushPosition = dataToWrite.Length + appendPosition;
                await file.FlushAsync(flushPosition);
            }

            return true;
        }
    }
}
