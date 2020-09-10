using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Amazon.JSII.Runtime.Deputy;
using Newtonsoft.Json;


namespace Demo.Services.StepFunctionDemo.Cdk
{
    public class CustomTask : RunEcsFargateTask
    {
        private IRunEcsFargateTaskProps _props;

        //public CustomTask(string taskDefinitionArn)
        //{
        //    _taskDefinitionArn = _taskDefinitionArn;
        //}   
        //public IStepFunctionsTaskConfig Bind(Task task)
        //{
        //    return new StepFunctionsTaskConfig
        //    {
        //        ResourceArn = "",
        //        Parameters = new Dictionary<string, object>
        //        {
        //            ["LaunchType"]= "FARGATE",
        //            ["Cluster"]= "arn:aws:ecs:us-west-2:342600918501:cluster/OHC-PACES",
        //            ["TaskDefinition"]= "s",
        //            ["Overrides"]= new Dictionary<string,object>
        //            {
        //                ["ContainerOverrides"]=new Dictionary<string,string>[]
        //                {
        //                   new Dictionary<string, string> {
        //                        ["Name"]="app",
        //                        ["Environment.$"]= "$.Data"
        //                    }
        //                }
        //            }
        //        }
        //    };

        //}
        [JsiiMethod("bind", "{\"type\":{\"fqn\":\"@aws-cdk/aws-stepfunctions.StepFunctionsTaskConfig\"}}", "[{\"name\":\"task\",\"type\":{\"fqn\":\"@aws-cdk/aws-stepfunctions.Task\"}}]", false, true)]
        public override IStepFunctionsTaskConfig Bind(Task task)
        {
            Console.WriteLine("Value:"+_props.Cluster.Vpc.PublicSubnets[0].SubnetId);
            return new StepFunctionsTaskConfig
            {
                ResourceArn= "arn:aws:states:::ecs:runTask.sync",
                Parameters = new Dictionary<string, object>
                {
                    ["LaunchType"] = "FARGATE",
                    ["Cluster"] = _props.Cluster.ClusterArn,
                    ["NetworkConfiguration"]=new Dictionary<string, object>
                    { 
                        ["AwsvpcConfiguration"]=new Dictionary<string, object>
                        { 
                            ["Subnets"]=_props.Cluster.Vpc.PrivateSubnets.Select(p=>p.SubnetId).ToArray()
                            //["SecurityGroups"]=_props.Cluster.Vpc.
                        }
                    },
                    ["TaskDefinition"] = _props.TaskDefinition.TaskDefinitionArn,
                    ["Overrides"] = new Dictionary<string, object>
                    {
                        ["ContainerOverrides"] = new Dictionary<string, string>[]
                        {
                               new Dictionary<string, string> {
                                    ["Name"]="app",
                                    ["Command.$"]= "$.Data"
                                }
                        }
                    }
                }
            };
            //return this.InvokeInstanceMethod<IStepFunctionsTaskConfig>(new Type[1]
            //{
            //    typeof (Task)
            //}, new object[1] { (object)task }, nameof(Bind));
        }
        public CustomTask(IRunEcsFargateTaskProps props) : base(props)
        {
            _props = props;
        }

        protected CustomTask(ByRefValue reference) : base(reference)
        {
        }

        protected CustomTask(DeputyProps props) : base(props)
        {
        }
    }
}
