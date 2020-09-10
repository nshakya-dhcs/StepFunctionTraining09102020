using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Runtime.SharedInterfaces;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using Demo.Lib.Data;
using Console = System.Console;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Demo.Services.Lambda
{
    public class Functions
    {
        public const string StateMachineArnKey = "STATE_MACHINE_ARN";
        public const string StepFunctionDemoBucketKey = "STEP_FUNCTION_DEMO_BUCKET";
        private readonly string _oddEvenStateMachineArnKey;
        private readonly string _stepFunctionDemoBucketName;
        private readonly IAmazonStepFunctions _stepFunctionsClient;
        private readonly IAmazonS3 _s3Client;

        public Functions() : this(
            new AmazonStepFunctionsClient(),
            new AmazonS3Client(),
            System.Environment.GetEnvironmentVariable(StateMachineArnKey), 
            System.Environment.GetEnvironmentVariable(StepFunctionDemoBucketKey)
            )
        {

        }

        public Functions(
            IAmazonStepFunctions stepFunctionsClient, 
            IAmazonS3 s3Client,
            string stateMachineArn, 
            string stepFunctionDemoBucketName)
        {
            _oddEvenStateMachineArnKey = stateMachineArn;
            _stepFunctionDemoBucketName = stepFunctionDemoBucketName;
            _stepFunctionsClient = stepFunctionsClient;
            _s3Client = s3Client;
        }
        public Input OddOrEvenFunction(Input state, ILambdaContext context)
        {
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine($"Input:{state.Value.ToString()}");
            state.Value = state.Value;
            state.Result = state.Value % 2 == 0 ? "Even" : "Odd";
            return state;
        }

        public JobInfo Process1Function(Input state, ILambdaContext context)
        {
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine($"Process 1::{state.Value}");
            return new JobInfo()
            {
                JobId = Guid.NewGuid().ToString(),
                Data = state.Value,
                ECSPayload = new []{state.Value.ToString()},
                Override = state.Override
            };
        }

        //public JobInfo TaskExecutionFunction(Input state, ILambdaContext context)
        //{

        //}

        public void Process2Function(Input state, ILambdaContext context)
        {
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine($"Process 2");
        }


        //public JobInfo Process11Function(JobInfo jobInfo, ILambdaContext context)
        //{
        //    System.Threading.Thread.Sleep(3000);
        //    jobInfo.Resolved = jobInfo.Data == 10 ? true : false;
        //    return jobInfo;
        //}

        public bool Process11Function(JobInfo jobInfo, ILambdaContext context)
        {
            System.Threading.Thread.Sleep(3000);
            return jobInfo.Data == 10 ? true : false;
            
        }

        public async Task<JobInfo> Process12Function(JobInfo jobInfo, ILambdaContext context)
        {
            System.Threading.Thread.Sleep(3000);
            var key = $"DemoResults/Result_{System.DateTime.Now:MMddyyyy}.json";
            Console.WriteLine($"{_stepFunctionDemoBucketName}/{key}");
            Console.WriteLine(JsonSerializer.Serialize(jobInfo));
            await _s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _stepFunctionDemoBucketName,
                Key = key,
                ContentBody = JsonSerializer.Serialize(jobInfo)
            });

            return jobInfo;
        }

        public async Task TaskTokenExecutor(JobWithTaskToken state, ILambdaContext context)
        {
            var key = $"TaskRequest/Request_{System.DateTime.Now.ToString("MMddyyyyhhmmss")}.json";
            Console.WriteLine($"{_stepFunctionDemoBucketName}/{key}");
            Console.WriteLine(JsonSerializer.Serialize(state));
            await new AmazonS3Client().PutObjectAsync(new PutObjectRequest
            {
                BucketName = _stepFunctionDemoBucketName,
                Key = key,
                ContentBody = JsonSerializer.Serialize(state)

            });
            Console.WriteLine("Complete");
        }

        public async Task InvokeOddEvenStepFunction(Input state, ILambdaContext context)
        {
            // Start step function execution
            var execution = await _stepFunctionsClient.StartExecutionAsync(new StartExecutionRequest
            {
               StateMachineArn = _oddEvenStateMachineArnKey,
               Name = $"StepFunctionExecution-{context.AwsRequestId}",
               Input = JsonSerializer.Serialize(state)
               
            });
        }
    }
}
