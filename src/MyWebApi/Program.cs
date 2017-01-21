﻿using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MyWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "API";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:7000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
