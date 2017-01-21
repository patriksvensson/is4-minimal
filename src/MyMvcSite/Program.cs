using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MyMvcSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "MVC";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:8000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
