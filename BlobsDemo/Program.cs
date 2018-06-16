using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = GetContainer("images");

            //UploadBlob(container, @"C:\temp\emma2.jpg", "emma2.jpg");

            //ListBlobs(container);

            //DownloadBlob(container, "emma2.jpg",
            //    @"C:\temp\emma2_downloaded.jpg");

            //CopyBlob(container, "emma2.jpg", "emma2_copy.jpg");

            //UploadBlob(container, @"C:\temp\emma2.jpg",
            //    @"emma\emma2.jpg");

            //var dic = new Dictionary<string, string>();
            //dic.Add("actress", "Emma Watson");
            //SetMetaData(container, dic);

            //GetMetaData(container);

            Console.WriteLine(LeaseBlob(container, "emma2.jpg"));

            Console.WriteLine("Operación exitosa");
            Console.ReadLine();
        }

        static CloudBlobContainer 
            GetContainer(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager
                .GetSetting("StorageData"));

            var client = storageAccount.CreateCloudBlobClient();

            var container = client
                .GetContainerReference(containerName);

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            return container;
        }

        static void UploadBlob(CloudBlobContainer container,
            string path, string blobTo)
        {
            var blockBlob = container
                .GetBlockBlobReference(blobTo);

            using (var stream = File.OpenRead(path))
            {
                blockBlob.UploadFromStream(stream);
            }
        }        

        static void ListBlobs(CloudBlobContainer container)
        {
            var blobs = container.ListBlobs();
            foreach(var blob in blobs)
            {
                Console.WriteLine(blob.Uri);
            }
        }

        static void DownloadBlob(CloudBlobContainer container,
            string blobName, string localPath)
        {
            var blockBlob =
                container.GetBlockBlobReference(blobName);
            using (var stream = File.OpenWrite(localPath))
            {
                blockBlob.DownloadToStream(stream);
            }
        }

        static void CopyBlob(CloudBlobContainer container,
            string blobFrom, string blobTo)
        {
            var blockBlob =
                container.GetBlockBlobReference(blobFrom);
            var blockBlobCopy =
                container.GetBlockBlobReference(blobTo);

            blockBlobCopy.BeginStartCopy(blockBlob, callback,
                null);
        }

        private static void callback(IAsyncResult ar)
        {
            Console.WriteLine("Blob copied!");
        }
        
        static void SetMetaData(CloudBlobContainer container,
            Dictionary<string,string> data)
        {
            container.Metadata.Clear();
            foreach(var d in data)
            {
                container.Metadata.Add(d);
            }
            container.SetMetadata();
        }

        static void GetMetaData(CloudBlobContainer container)
        {
            container.FetchAttributes();
            foreach(var item in container.Metadata)
            {
                Console.WriteLine($"{item.Key}, {item.Value}");
            }
        }

        static string LeaseBlob(CloudBlobContainer container,
            string blobName)
        {
            var blockBlob = container.GetBlockBlobReference(blobName);

            TimeSpan? leaseTime = TimeSpan.FromSeconds(60);

            string leaseID =
                blockBlob.AcquireLease(leaseTime, null);

            return leaseID;
        }
    }
}
