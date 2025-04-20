using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Minimarket;

using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Minimarket.Models;

namespace LoginApp
{
    public class Program
    {

        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        

       
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}