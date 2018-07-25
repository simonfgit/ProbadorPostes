using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Ip;
using Microsoft.Extensions.Configuration;
using Pole.Tester;
using Serilog;
using System;
using System.IO;
using System.Linq;
using Eternet.Mikrotik.Entities.Interface.Ethernet.Poe;
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

            var poeReader = connection.CreateEntityReader<EthernetPoe>();

            var poeInterfaces = poeReader.GetAll().ToArray();

            var interfacesToTest = PoleTester.GetNeighborsOnRunningInterfaces(etherReader, neighReader, Log.Logger);

            var interfacesPoeStatus = PoleTester.GetInterfacesPoeStatus(connection, poeInterfaces, Log.Logger);

            foreach (var ethtotest in interfacesToTest)
            {
                Log.Logger.Information("Agregada para testear {InterfaceTesteable}", ethtotest.ToString());
            }

            foreach (var ethPoeStatus in interfacesPoeStatus)
            {
                Log.Logger.Information("Estado Poe {InterfacePoeStatus}", ethPoeStatus.ToString());
            }

            connection.Dispose();
            Log.Logger.Information("Done!, press any key to end");
            Console.ReadLine();
        }
    }
}
