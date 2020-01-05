using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Persistence.Storage.Preprocessor
{
    public interface IPreprocessorStorageContext
    {
        CloudStorageAccount StorageAccount { get; set; }
        PreprocessorStorageSettings Settings { get; set; }
    }

    public class PreprocessorStorageSettings
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
