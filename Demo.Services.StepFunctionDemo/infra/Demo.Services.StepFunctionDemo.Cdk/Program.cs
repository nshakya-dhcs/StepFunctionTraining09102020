using Amazon.CDK;


namespace Demo.Services.StepFunctionDemo.Cdk
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            _ = new StepFunctionDemoStack(app, "StepFunctionDemoStack", new StackProps
            {
                Env = MakeEnv()
            });
            app.Synth();
        }

        private static Environment MakeEnv(string account = null, string region = null)
        {
            return new Amazon.CDK.Environment
            {
                Account = account ??
                          System.Environment.GetEnvironmentVariable("CDK_DEPLOY_ACCOUNT") ??
                          System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                Region = region ??
                         System.Environment.GetEnvironmentVariable("CDK_DEPLOY_REGION") ??
                         System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
            };
        }
    }
}
