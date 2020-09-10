using System;

namespace Demo.Services.ECS
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var result = false;
            if (args?[0] != null)
            {
                Console.WriteLine(args[0]);
                result=Convert.ToInt32(args[0]) == 10 ? true : false;
            }
        }
    }
}
