using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Ip;
using Microsoft.Extensions.Configuration;
using Pole.Tester;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var interfacesToTest = PoleTester.GetNeighborsOnRunningInterfaces(etherReader, neighReader, Log.Logger);

            foreach (var ethtotest in interfacesToTest)
            {
                Console.WriteLine(ethtotest.ToString());
            }

            connection.Dispose();
            Log.Logger.Information("Done!, press any key to end");
            Console.ReadLine();

            //if (args == null) throw new ArgumentNullException(nameof(args));

            //// Set up configuration sources.
            //var mycfg = InitialSetup();

            //var connection = GetMikrotikConnection(mycfg.Host, mycfg.ApiUser, mycfg.ApiPass);

            //var neighReader = connection.CreateEntityReader<IpNeighbor>();

            //var etherReader = connection.CreateEntityReader<InterfaceEthernet>();

            //var runningethers = etherReader.GetAll().Where(p => p.Running == true);

            //var interfacesToTest = new List<(string, string)>();

            //foreach (var ether in runningethers)
            //{
            //    if (neighReader.GetAll().Count(i => i.Interface.Contains(ether.Name)) == 1)
            //    {
            //        var neigh = neighReader.Get(n => n.Interface.Contains(ether.Name));
            //        Log.Logger.Information("En la interface {Interface} se encuentra un equipo {Modelo} con la MAC {MacAddress}", neigh.Interface, neigh.Board, neigh.MacAddress);
            //        if (neigh.Address4 != "")
            //        {
            //            interfacesToTest.Add((neigh.Interface, neigh.Address4));
            //            Log.Logger.Information("Se agrego a la lista de interfaces a probar a Iface {Interface} con IP {Address}", neigh.Interface, neigh.Address4);
            //        }
            //        else
            //        {
            //            Log.Logger.Information("El vecino en la {Interface} NO tiene IP y NO se agrega a la lista de pruebas", neigh.Interface);
            //        }
            //    }
            //    else
            //    {
            //        Log.Logger.Information("La interface {Interface} esta running y contiene varios o NO contiene ningun vecino/vecinos", ether.Name);
            //    }
            //}

            //foreach (var ethtotest in interfacesToTest)
            //{
            //    Console.WriteLine(ethtotest.ToString());
            //}

            //connection.Dispose();
            //Log.Logger.Information("Done!, press any key to end");
            //Console.ReadLine();
        }
    }
}
