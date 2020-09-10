using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Demo.Lib.Data;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;

namespace Demo.Services.ECS1
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var s3Client = new AmazonS3Client();
            var stepFunctionclient = new AmazonStepFunctionsClient();
            var bucket = Environment.GetEnvironmentVariable("STEP_FUNCTION_DEMO_BUCKET");
            var requestObjects = await s3Client.ListObjectsAsync(new ListObjectsRequest
            {
                BucketName = bucket,
                Prefix = "TaskRequest"
            });
            foreach (var obj in requestObjects.S3Objects)
            {
                var s3object = await s3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = obj.Key

                });

                using var sr = new StreamReader(s3object.ResponseStream);
                var data = JsonSerializer.Deserialize<JobWithTaskToken>(await sr.ReadToEndAsync());
                if (data.State.Data == 10)
                {
                    data.State.Resolved = true;
                }

                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = $"TaskResponse/Response-{System.DateTime.Now.ToString("MMddyyyyhhmmss")}.json",
                    ContentBody = JsonSerializer.Serialize(data)

                });

                if (!string.IsNullOrEmpty(data.TaskToken))
                {
                    await stepFunctionclient.SendTaskSuccessAsync(new SendTaskSuccessRequest
                    {
                        TaskToken = data.TaskToken,
                        Output = JsonSerializer.Serialize(data.State)
                    });
                }

                await s3Client.CopyObjectAsync(new CopyObjectRequest
                {
                    SourceBucket = obj.BucketName,
                    DestinationBucket = bucket,
                    SourceKey = obj.Key,
                    DestinationKey = obj.Key.Replace("TaskRequest","Completed")
                });

                await s3Client.DeleteObjectAsync(bucket, obj.Key);


            }
        }
    }
}
