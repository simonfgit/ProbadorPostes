using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Interface.Ethernet.Poe;
using Eternet.Mikrotik.Entities.Ip;
using Eternet.Mikrotik.Entities.ReadWriters;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using Eternet.Mikrotik.Entities.Interface.Ethernet;

namespace Pole.Tester
{
    public class PoleTester
    {
        private readonly ILogger _logger;
        private readonly ITikConnection _connection;

        public PoleTester(ILogger logger, ITikConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public List<(string iface, string ip)> GetNeighborsOnRunningInterfaces(IEntityReader<InterfaceEthernet> ethReader, IEntityReader<IpNeighbor> neigReader)
        {
            var results = new List<(string iface, string ip)>();
            var runningethers = ethReader.GetAll().Where(p => p.Running);
            var neigList = neigReader.GetAll();

            foreach (var ether in runningethers)
            {
                var neig = neigList.FirstOrDefault(n => n.Interface.ToLower().Equals(ether.Name.ToLower()));
                if (neig != null)
                {
                    _logger.Information("En la interface {Interface} se encuentra un equipo {Modelo} con la MAC {MacAddress}", neig.Interface, neig.Board, neig.MacAddress);
                    if (!string.IsNullOrEmpty(neig.Address4))
                    {
                        results.Add((neig.Interface, neig.Address4));
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
            return results;
        }

        public List<(string iface, EthernetPoeStatus poeStatus)> GetInterfacesPoeStatus(IMonitoreable<MonitorPoeResults>[] poeReader)
        {
            var results = new List<(string, EthernetPoeStatus)>();
            foreach (var ethPoe in poeReader)
            {
                var poeStatus = ethPoe.MonitorOnce(_connection);
                _logger.Information("La interface {Interface} tiene un estado Poe {PoeStatus}", poeStatus.Name, poeStatus.PoeOutStatus);
                results.Add((poeStatus.Name, poeStatus.PoeOutStatus));
            }
            return results;
        }

        public List<(string, string, bool, EthernetRates)> GetInterfacesNegotiation(IMonitoreable<MonitorEthernetResults>[] negotiationReader)
        {
            var interfacesRunningNegotiation = new List<(string, string, bool, EthernetRates)>();

            foreach (var iface in negotiationReader)
            {
                var negoStatus = iface.MonitorOnce(_connection);
                _logger.Information("Interface {Interface}, Autonegotiation {AutoStatus}, Full Dulplex {FullStatus}, Rate {RateStatus}",
                    negoStatus.Name, negoStatus.AutoNegotiation, negoStatus.FullDuplex, negoStatus.Rate);
                interfacesRunningNegotiation.Add((negoStatus.Name, negoStatus.AutoNegotiation, negoStatus.FullDuplex, negoStatus.Rate));
            }

            return interfacesRunningNegotiation;
        }

    }
}
