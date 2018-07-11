using System;
using System.IO;
using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities;
using Eternet.Mikrotik.Entities.Ip;
using Microsoft.Extensions.Configuration;
using Serilog;
using Log = Serilog.Log;

namespace ProbadorPostes
{
    internal class Program
    {
        private static ITikConnection GetMikrotikConnection(string host, string user, string pass)
        {
            var connection = ConnectionFactory.CreateConnection(TikConnectionType.Api);
            connection.Open(host, user, pass);
            return connection;
        }

        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false);

            var cfg = builder.Build();

            var mycfg = new ConfigurationClass();
            cfg.GetSection("ConfigurationClass").Bind(mycfg);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(cfg)
                .CreateLogger();

            var connection = GetMikrotikConnection(mycfg.Host, mycfg.ApiUser, mycfg.ApiPass);

            var neighReader = connection.CreateEntityReader<IpNeighbor>();

            var neighbors = neighReader.GetAll();

            foreach (var neigh in neighbors)
            {
                Console.WriteLine($"En la interface {neigh.Interface} se encuentra un equipo {neigh.Board} con la MAC {neigh.MacAddress}");
            }
            Console.WriteLine("Press any key to end");
            Console.ReadLine();
        }
    }
}
