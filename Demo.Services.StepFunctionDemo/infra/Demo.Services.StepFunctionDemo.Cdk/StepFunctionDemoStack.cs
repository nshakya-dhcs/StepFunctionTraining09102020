using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Demo.Services.Lambda;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.DynamoDB;

namespace Demo.Services.StepFunctionDemo.Cdk
{
    public class StepFunctionDemoStack : Stack
    {
         internal StepFunctionDemoStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Bucket stepFunctionDemoBucket = new Bucket(this, "StepFunctionDemoBucket", new BucketProps
            {
                Encryption = BucketEncryption.S3_MANAGED,
                RemovalPolicy = RemovalPolicy.RETAIN
            });

            Table stepFunctionDemoTable = new Table(this,"StepFunctionDemoTable",new TableProps{
                BillingMode = BillingMode.PROVISIONED,
                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute
                {
                    Name = "Id",
                    Type = AttributeType.STRING
                },
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            //Step Function invoking Lambda function
            Function invokeOddEvenStepFunction = new Function(this, "InvokeOddEvenStepFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::InvokeOddEvenStepFunction",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Lambda Function that invokes the Demo Step Function",

            });


            //Function to calculate Odd or Even
            Function oddOrEvenFunction = new Function(this, "OddOrEvenFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::OddOrEvenFunction",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Lambda Function that calculates odd or even",

            });

            //Demo Lambda to perform Process 1
            Function process1Function = new Function(this, "Process1Function", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::Process1Function",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Demo Lambda Function that runs process1",

            });

            Function processAFunction = new Function(this, "ProcessAFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::Process1Function",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Demo Lambda Function that runs process1",

            });

            //Demo Lambda to perform Process 2
            Function process2Function = new Function(this, "Process2Function", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::Process2Function",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Demo Lambda Function that runs process2",

            });

            //Demo Lambda to perform Process 1
            Function process11Function = new Function(this, "Process11Function", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::Process11Function",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Demo Lambda Function that runs job process1",

            });

            //Demo Lambda to perform Process 2
            Function process12Function = new Function(this, "Process12Function", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::Process12Function",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Demo Lambda Function that runs job process2",

            });

            Function taskTokenExecutorFunction = new Function(this, "TaskTokenExecutorFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/Demo.Services.Lambda/bin/Release/netcoreapp3.1/Demo.Services.Lambda.zip"),
                Handler = "Demo.Services.Lambda::Demo.Services.Lambda.Functions::TaskTokenExecutor",
                Timeout = Duration.Minutes(5),
                MemorySize = 512,
                Description = "Demo Lambda Function that executes Task Token Step",
                Environment = new Dictionary<string, string>(){
                    ["STEP_FUNCTION_DEMO_BUCKET"]=stepFunctionDemoBucket.BucketName
                }

            });

            stepFunctionDemoBucket.GrantReadWrite(taskTokenExecutorFunction);


            var oddEvenFunction = new Task(this, "OddEvenFunction", new TaskProps
            {
                Task = new InvokeFunction(oddOrEvenFunction.LatestVersion)
            });


            var process1 = new Task(this, "Process1", new TaskProps
            {
                Task = new InvokeFunction(process1Function.LatestVersion)
            });

            var processA = new Task(this, "ProcessA", new TaskProps
            {
                Task = new InvokeFunction(processAFunction.LatestVersion)
            });


            var process2 = new Task(this, "Process2", new TaskProps
            {
                Task = new InvokeFunction(process2Function.LatestVersion)
            });

            var process11 = new Task(this, "Process11", new TaskProps
            {
                Task = new InvokeFunction(process11Function.LatestVersion),
                ResultPath = "$.Resolved"
            });

            var process12 = new Task(this, "Process12", new TaskProps
            {
                Task = new InvokeFunction(process12Function.LatestVersion)
            });

            var taskTokenExecutor = new Task(this, "TaskTokenExecutor", new TaskProps
            {
                Task = new RunLambdaTask(taskTokenExecutorFunction.LatestVersion,new RunLambdaTaskProps() 
                {
                   IntegrationPattern = ServiceIntegrationPattern.WAIT_FOR_TASK_TOKEN,
                   Payload = TaskInput.FromContextAt("$$.Task.Token")
                }),
                Parameters = new Dictionary<string, object>
                {
                    ["Payload"] = new Dictionary<string, object>
                    {
                        ["TaskToken.$"] = "$$.Task.Token",
                        ["State.$"] = "$"
                    }
                }

            });

   
            //Choice to go to Process 1 or Process 2 based on input number is odd or even.
            var isEven = new Choice(this, "Is the number Even?");
            var isResolvedOrOverriden = new Choice(this, "Is Resolved Or Overriden?");

            //var chain1 = Chain.Start(oddEvenFunction)
            //    .Next(isEven
            //            .When(
            //                Condition.StringEquals("$.Result", "Even"),
            //                Chain.Start(process1)
            //                    .Next(process11)
            //                    .Next(isResolvedOrOverriden
            //                        .When(
            //                            Condition.Or(
            //                                new[]
            //                                {
            //                                    Condition.BooleanEquals("$.Resolved", true),
            //                                    Condition.BooleanEquals("$.Override", true)
            //                                }), process12)
            //                        .Otherwise(process2)))
            //            .When(Condition.StringEquals("$.Result", "Odd"), process2));

            var chain1 = Chain.Start(oddEvenFunction)
                .Next(isEven
                    .When(
                        Condition.StringEquals("$.Result", "Even"),
                        Chain.Start(process1)
                            .Next(taskTokenExecutor)
                            .Next(isResolvedOrOverriden
                                .When(
                                    Condition.Or(
                                        new[]
                                        {
                                            Condition.BooleanEquals("$.Resolved", true),
                                            Condition.BooleanEquals("$.Override", true)
                                        }), process12)
                                .Otherwise(process2)))
                    .When(Condition.StringEquals("$.Result", "Odd"), process2));


            //State Machine

            var stateMachine = new StateMachine(this, "JobDemoStateMachine", new StateMachineProps
            {
                StateMachineName = "JobDemoStateMachine",
                Timeout = Duration.Minutes(5),
                Definition = chain1
            });

            stateMachine.Role?.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(this, "DynamoDBFullAccessForStepFunction", "arn:aws:iam::aws:policy/AmazonDynamoDBFullAccess"));

            stateMachine.Role?.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(this, "LambdaFullAccessForStepFunction", "arn:aws:iam::aws:policy/AWSLambdaFullAccess"));     

            var demofargateTask1 = new FargateTaskDefinition(this,
                "demoECSTask1Definition", new FargateTaskDefinitionProps
                {
                    MemoryLimitMiB = 4096,
                    Cpu = 2048
                });
            var demofargateTask2 = new FargateTaskDefinition(this,
                "demoECSTask2Definition", new FargateTaskDefinitionProps
                {
                    MemoryLimitMiB = 4096,
                    Cpu = 2048
                });
            stepFunctionDemoBucket.GrantReadWrite(demofargateTask2.TaskRole);

            IVpc publicVpc = Vpc.FromLookup(this, "PublicVPC", new VpcLookupOptions
            {
                Tags = new Dictionary<string, string>
                {
                    ["Paces:VpcType"] = "Public"
                }
            });
            var cluster = Cluster.FromClusterAttributes(this, "PublicCluster", new ClusterAttributes
            {
                ClusterName = "OHC-PACES",
                Vpc = publicVpc,
                SecurityGroups = new[]
                    {SecurityGroup.FromSecurityGroupId(this, "SecurityGroup", "sg-0a1bab8166d8fb715")}
            });

            var container1 = demofargateTask1.AddContainer("app", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromAsset(".",new AssetImageProps
                {
                    File = "Dockerfile"
                }),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps
                {
                    LogGroup = new LogGroup(this, "demoECSTask1LogGroup", new LogGroupProps
                    {
                        LogGroupName = "/ecs/demoECSTask1-" + RandomString.Generate(10, StackId),
                    }),
                    StreamPrefix = "logs"
                }),
                

            });


            var container2 = demofargateTask2.AddContainer("app", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromAsset(".", new AssetImageProps
                {
                    File = "Dockerfile.1"
                }),
                Environment = new Dictionary<string, string>
                {
                    ["STEP_FUNCTION_DEMO_BUCKET"] =stepFunctionDemoBucket.BucketName
                },
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps
                {
                    LogGroup = new LogGroup(this, "demoECSTask2LogGroup", new LogGroupProps
                    {
                        LogGroupName = $"/ecs/demoECSTask2-{RandomString.Generate(10, StackId)}",
                    }),
                    StreamPrefix = "logs"
                })

            });

      
            Rule rule = new Rule(this, "DemoJobRule", new RuleProps
            {
                Schedule = Schedule.Cron(new CronOptions
                {
                    Day = "*",
                    Hour = "*",
                    Minute = "1",
                    Month = "*",
                    Year = "*"
                }),
                Description = "Runs demo job fargate task",
                Targets = new IRuleTarget[]
                {
                    new EcsTask(
                        new EcsTaskProps
                        {
                            Cluster = cluster,
                            TaskDefinition = demofargateTask2,
                            SubnetSelection = new SubnetSelection
                            {
                                OnePerAz = true
                            }
                        })
                }
            });



            //var ecsTask1 = new Task(this, "ecsTask1", new TaskProps
            //{
            //    InputPath = "$",
            //    Task = new CustomTask(new RunEcsFargateTaskProps
            //    {

            //        Cluster = Cluster.FromClusterAttributes(this, "PublicCluster", new ClusterAttributes
            //        {
            //            ClusterName = "OHC-PACES",
            //            Vpc = publicVpc,
            //            SecurityGroups = new[] { SecurityGroup.FromSecurityGroupId(this, "SecurityGroup", "sg-0a1bab8166d8fb715") }
            //        }),
            //        TaskDefinition = fargateTask1,

            //        ContainerOverrides = new[]
            //        {
            //            new ContainerOverride
            //            {
            //               ContainerDefinition = container,
            //               Command = new []{"$.Data"}
            //            },

            //        }
            //    })
            //});

            var ecsTask1 = new Task(this, "EcsTask1", new TaskProps
            {
                InputPath = "$",
                Task = new RunEcsFargateTask(new RunEcsFargateTaskProps
                {

                    Cluster = cluster,
                    TaskDefinition = demofargateTask1,

                    //ContainerOverrides = new[]
                    //{
                    //    new ContainerOverride
                    //    {
                    //        ContainerDefinition = container,
                    //        
                    //    },

                    //}
                }),
                Parameters = new Dictionary<string, object>
                {
                    ["Overrides"] = new Dictionary<string, object>
                    {
                        ["ContainerOverrides"] = new Dictionary<string, string>[]
                        {
                            new Dictionary<string, string> {
                                ["Name"]="app",
                                ["Command.$"]= "$.ECSPayload"
                            }
                        }
                    }
                }
            });

            var chain2 = Chain.Start(processA).Next(ecsTask1);


            var stateMachineWithTask = new StateMachine(this, "JobDemoStateMachine-1", new StateMachineProps
            {
                StateMachineName = "JobDemoStateMachine-1",
                Timeout = Duration.Minutes(15),
                Definition = chain2,
                Role = Role.FromRoleArn(this, "StateMachineWithTaskRole",
                    "arn:aws:iam::342600918501:role/service-role/PacesEdi274DefaultStateMachineRole")
            });
            //All Policies
            // 1. Invoke function policies

            invokeOddEvenStepFunction.Role?.AddManagedPolicy(ManagedPolicy.FromManagedPolicyArn(this, "InvokeLambdaPolicy", "arn:aws:iam::aws:policy/AWSLambdaFullAccess"));

            var policyStatement = new PolicyStatement
            {
                Sid = "CanInvokeStepFunctions",
                Effect = Effect.ALLOW
            };
            policyStatement.AddActions(new[] { "states:StartExecution" });

            invokeOddEvenStepFunction.AddToRolePolicy(policyStatement);
            policyStatement.AddResources(stateMachine.StateMachineArn);
            invokeOddEvenStepFunction.AddEnvironment(Functions.StateMachineArnKey, stateMachine.StateMachineArn);

            process12Function.AddEnvironment(Functions.StepFunctionDemoBucketKey, stepFunctionDemoBucket.BucketName);

            stepFunctionDemoBucket.GrantReadWrite(process12Function);


            var policyStatementDemofargateTask2 = new PolicyStatement
            {
                Sid = "CanNotifyStepFunction",
                Effect = Effect.ALLOW
            };
            policyStatementDemofargateTask2.AddActions(new[] { "states:SendTask*" });
            demofargateTask2.AddToExecutionRolePolicy(policyStatementDemofargateTask2);
            demofargateTask2.AddToTaskRolePolicy(policyStatementDemofargateTask2);

            policyStatementDemofargateTask2.AddAllResources();
           
        }


       static class RandomString
    {
        public static string Generate(int length, string seed = null)
        {
            StringBuilder sb = new StringBuilder();
            Random random;
            if (seed != null)
            {
                random = new Random(StringHashDeterministic(seed));
            }
            else
            {
                random = new Random();
            }

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                sb.Append(letter);
            }

            return sb.ToString();
        }

        private static int StringHashDeterministic(string str)
        {
            unchecked
            {
                return str.Aggregate(23,(p,q)=>p=p*31+q);
            }
        }
    }
    }
}
