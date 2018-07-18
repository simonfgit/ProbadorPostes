using Eternet.Mikrotik.Entities.ReadWriters;
using System;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Ip;
using Serilog;
using System.Linq;
using System.Collections.Generic;

namespace Pole.Tester
{
    public class PoleTester
    {
        public static List<(string, string)> GetNeighborsOnRunningInterfaces(IEntityReader<InterfaceEthernet> ethReader, IEntityReader<IpNeighbor> neigReader, ILogger log)
        {
            var interfacesToTest = new List<(string, string)>();

            var runningethers = ethReader.GetAll().Where(p => p.Running == true);

            foreach (var ether in runningethers)
            {
                if (neigReader.GetAll().Count(i => i.Interface.Contains(ether.Name)) == 1)
                {
                    var neig = neigReader.Get(n => n.Interface.Contains(ether.Name));
                    Log.Logger.Information("En la interface {Interface} se encuentra un equipo {Modelo} con la MAC {MacAddress}", neig.Interface, neig.Board, neig.MacAddress);
                    if (neig.Address4 != "")
                    {
                        interfacesToTest.Add((neig.Interface, neig.Address4));
                        Log.Logger.Information("Se agrego a la lista de interfaces a probar a Iface {Interface} con IP {Address}", neig.Interface, neig.Address4);
                    }
                    else
                    {
                        Log.Logger.Information("El vecino en la {Interface} NO tiene IP y NO se agrega a la lista de pruebas", neig.Interface);
                    }
                }
                else
                {
                    Log.Logger.Information("La interface {Interface} esta running y contiene varios o NO contiene ningun vecino/vecinos", ether.Name);
                }
            }

            return interfacesToTest;
        }
    }
}
