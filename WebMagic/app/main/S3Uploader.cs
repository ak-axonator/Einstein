using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WebMagic
{
    public class S3Uploader
    {
        private string bucketName;
        private string folderPrefix;
        private AmazonS3Client s3Client;

        private string accessKey;
        private string secretKey;

        public S3Uploader(string region, string bucketName, string _folderPrefix = "")
        {
            this.bucketName = bucketName;
            folderPrefix = _folderPrefix;
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string accessKey = configuration.GetValue<string>("accessKey");
            string secretKey = configuration.GetValue<string>("secretKey");
            this.s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public void uploadFiles(List<String> files)
        {
            foreach (string file in files)
            {
                Console.WriteLine("Uploading " + file + "...");
                // Determine the S3 key for the file based on the prefix and file name
                string s3Key = Path.Combine(folderPrefix, System.IO.Path.GetFileName(file));
                // Create a transfer utility to upload the file to S3
                TransferUtility transferUtility = new TransferUtility(s3Client);
                transferUtility.Upload(file, bucketName, s3Key);
            }
        }

        public void deleteFiles(List<String> files)
        {
            foreach (string file in files)
            {
                Console.WriteLine("Deleting " + file + "...");

                // Determine the S3 key for the file based on the prefix and file name
                string s3Key = Path.Combine(folderPrefix, System.IO.Path.GetFileName(file));
                // Create a transfer utility to upload the file to S3
                TransferUtility transferUtility = new TransferUtility(s3Client);
                // Create a transfer utility request object for the file to be deleted
                string _fileName = Path.ChangeExtension(Path.GetRelativePath(Path.Combine(GlobalPaths.ProjectFolder,"pages"),file),"html");
                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = _fileName
                };

                // Use the transfer utility object to delete the file from S3
                s3Client.DeleteObjectAsync(request);
            }
        }
    }
}
