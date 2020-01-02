using Azure.Storage.Files.DataLake;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worker.Models;
using Worker.Models.Persistence;
using Worker.Models.TableEntities;

namespace Worker.Tasks
{
    public static class UpdateContentViewDataLakeReports
    {
        public static async Task<bool> RunAsync(List<SourceActivityLog> telemetryData, IDataLakeStorageSharedKey dataLakeStorageSharedKey, TemporalState temporalState)
        {
            // This task updates .csv files in the data lake file system with new view statistics
            DataLakeServiceClient serviceClient = new DataLakeServiceClient(new Uri(dataLakeStorageSharedKey.URIs.DistributedFileSystem), dataLakeStorageSharedKey.SharedKeyCredentials); ;

            // Connect to namespace
            var fileSystemName = "content";
            DataLakeFileSystemClient filesystem = serviceClient.GetFileSystemClient(fileSystemName);
            //filesystem.Create();

            // First we group by same content id for this time span
            var groupedContent = telemetryData
            .GroupBy(u => u.ContentId)
            .Select(grp => grp.ToList())
            .ToList();

            foreach (var contentViewList in groupedContent)
            {
                // We get the count of each list and append to the view count for each content
                string contentId = contentViewList[0].ContentId;
                string accountId = contentViewList[0].AccountId;
                int newViews = contentViewList.Count;

                // Create DataLake Directory for the day of the view data
                var dateTimeViewsPath = contentId + "/views/" + temporalState.Year + "/" + temporalState.Month + "/" + temporalState.Day;
                DataLakeDirectoryClient directory = filesystem.GetDirectoryClient(dateTimeViewsPath);
                directory.Create();

                // Create a DataLake File for the hour using a DataLake Directory
                var fileName = temporalState.Hour + ".csv";
                DataLakeFileClient file = directory.GetFileClient(fileName);

                //Create if not exists
                var fileExists = false;

                try
                {
                    var fileProperties = file.GetProperties();
                    if(fileProperties != null)
                    {
                        fileExists = true;
                    }                    
                }
                catch
                {
                    file.Create();
                }

                var fileData = new StringBuilder();

                if(!fileExists)
                {
                    // File is new so add csv column names
                    fileData.Append("ContentID, ViewCount, AccountId, Date, Time");
                    fileData.Append("\r\n");
                }

                // Append new view info for the minute
                fileData.Append($"{contentId}, {newViews}, {accountId}, {temporalState.Month}/{temporalState.Day}/{temporalState.Year}, {temporalState.Hour}:{temporalState.Minute}");
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

            }

            return true;
        }
    }
}
