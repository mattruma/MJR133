using Azure.Storage.Blobs.Models;
using System;

namespace Client.Models
{
    public class FileModel
    {
        public BlobItem BlobItem { get; set; }

        public Uri Uri { get; set; }
    }
}
