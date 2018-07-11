using System;
using System.IO;
using System.Linq;
using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities;
using Eternet.Mikrotik.Entities.Interface;
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

        private static ConfigurationClass InitialSetup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false);

            var cfg = builder.Build();

            var mycfg = new ConfigurationClass();
            cfg.GetSection("ConfigurationClass").Bind(mycfg);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(cfg)
                .CreateLogger();
            return mycfg;
        }

        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            // Set up configuration sources.
            var mycfg = InitialSetup();

            var connection = GetMikrotikConnection(mycfg.Host, mycfg.ApiUser, mycfg.ApiPass);

            var neighReader = connection.CreateEntityReader<IpNeighbor>();

            var etherReader = connection.CreateEntityReader<InterfaceEthernet>();

            var runningethers = etherReader.GetAll().Where(p => p.Running == true);

            foreach (var ether in runningethers)
            {
                if (neighReader.GetAll().Count(i => i.Interface.Contains(ether.Name)) == 1)
                {
                    var neigh = neighReader.Get(n => n.Interface.Contains(ether.Name));
                    Console.WriteLine($"En la interface {neigh.Interface} se encuentra un equipo {neigh.Board} con la MAC {neigh.MacAddress}");
                }
                else
                {
                    Console.WriteLine($"La interface {ether.Name} esta running y no ve ningun vecino");
                }
            }
            connection.Dispose();
            Log.Logger.Information("Done!, press any key to end");
            Console.ReadLine();
        }
    }
}
