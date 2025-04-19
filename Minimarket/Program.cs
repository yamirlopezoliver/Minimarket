using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Minimarket;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Minimarket.Models;
using Microsoft.Data.SqlClient;

namespace LoginApp
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            EjecutarScriptSql(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    var env = context.HostingEnvironment;

                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static void EjecutarScriptSql(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var config = services.GetRequiredService<IConfiguration>();
            var connectionString = config["ConexionString"];

            var scriptPath = Path.Combine(AppContext.BaseDirectory, "Data", "proyectoIntegrador.sql");
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine("El archivo no existe.");
                return;
            }

            var script = File.ReadAllText(scriptPath);
            Console.WriteLine("Script leído correctamente.");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(script, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Script ejecutado exitosamente.");
            }
        }


    }
}