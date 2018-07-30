using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Interface.Ethernet.Poe;
using Eternet.Mikrotik.Entities.Ip;
using Eternet.Mikrotik.Entities.ReadWriters;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Pole.Tester
{
    public class PoleTester
    {
        private readonly ILogger _logger;

        public PoleTester(ILogger logger)
        {
            _logger = logger;
        }

        public List<(string, string)> GetNeighborsOnRunningInterfaces(IEntityReader<InterfaceEthernet> ethReader, IEntityReader<IpNeighbor> neigReader)
        {
            var interfacesToTest = new List<(string, string)>();

            var runningethers = ethReader.GetAll().Where(p => p.Running == true);
            var neigList = neigReader.GetAll();

            foreach (var ether in runningethers)
            {
                if (neigList.Count(i => i.Interface.Contains(ether.Name)) == 1)
                {
                    var neig = neigList.FirstOrDefault(n => n.Interface.Contains(ether.Name));
                    _logger.Information("En la interface {Interface} se encuentra un equipo {Modelo} con la MAC {MacAddress}", neig.Interface, neig.Board, neig.MacAddress);
                    if (neig.Address4 != "")
                    {
                        interfacesToTest.Add((neig.Interface, neig.Address4));
                        _logger.Information("Se agrego a la lista de interfaces a probar a la {Interface} con IP {Address}", neig.Interface, neig.Address4);
                    }
                    else
                    {
                        _logger.Error("El vecino en la {Interface} NO tiene IP y NO se agrega a la lista de pruebas", neig.Interface);
                    }
                }
                else
                {
                    _logger.Error("La interface {Interface} esta running y contiene NINGUN o VARIOS vecino/s", ether.Name);
                }
            }
            return interfacesToTest;
        }

        public List<(string, EthernetPoeStatus)> GetInterfacesPoeStatus(ITikConnection connection, IMonitoreable<MonitorPoeResults>[] poeReader)
        {
            var interfacesPoeStatus = new List<(string, EthernetPoeStatus)>();

            foreach (var ethPoe in poeReader)
            {
                var poeStatus = ethPoe.MonitorOnce(connection);
                _logger.Information("La interface {Interface} tiene un estado Poe {PoeStatus}", poeStatus.Name, poeStatus.PoeOutStatus);
                interfacesPoeStatus.Add((poeStatus.Name, poeStatus.PoeOutStatus));
            }

            return interfacesPoeStatus;
        }
    }
}
