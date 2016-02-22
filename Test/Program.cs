using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        private static void Main(string[] args)
        {
            var ad = AppDomain.CreateDomain("Plugin");
            var t = new T();
            t.Result = "hello";
            
            Assign("hello,world", t.Result);

            //var type = ad.Load()
        }


        public static void Assign(string input, string output)
        {
            output = input;
        }

        private class T
        {
            public string Result { get; set; }

        }
    }

   
}
