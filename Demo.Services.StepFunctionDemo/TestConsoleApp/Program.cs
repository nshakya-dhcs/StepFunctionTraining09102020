using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;


namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i <= 10; i++)
            {
                using (var md5 = MD5.Create())
                {
                    //var y = Encoding.ASCII.GetBytes("ABCDEF");
                    //var x =int.Parse(string.Join("",Encoding.ASCII.GetBytes("ABCDEF").Select(a=>a.ToString()).ToArray()));
                    var x = RandomString.StringHashDeterministic("ABCDEFFFFFFF");
                    Console.WriteLine("x: "+ x);
                    //var xyz = (int)long.Parse(string.Join("", md5.ComputeHash(Encoding.ASCII.GetBytes("ABCDEFGHasdlfihlsdahflksdahf")).Select(a=>a.ToString())))%int.MaxValue;
                    //Console.WriteLine("x: " + xyz);
                }
            }

        }

        

        static class RandomString
        {
            public static string Generate(int length, string seed = null)
            {
                StringBuilder sb = new StringBuilder();
                Random random;
                if (seed != null)
                {
                   
                    random = new Random(seed.GetHashCode());
                    random = new Random(-20123456);


                }
                else
                {
                    random = new Random();
                }

                char letter;

                for (int i = 0; i < length; i++)
                {
                    double flt = random.NextDouble();
                    //Console.WriteLine(flt);
                    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                    letter = Convert.ToChar(shift + 65);
                    sb.Append(letter);
                }

                return sb.ToString();
            }

            public static int StringHashDeterministic(string str)
            {
                unchecked
                {
                    return str.Aggregate(23,(p,q)=>p=p*31+q);
                }
            }
        }
    }
}
