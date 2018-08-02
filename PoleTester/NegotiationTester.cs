using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Interface.Ethernet;
using Serilog;
using System.Collections.Generic;

namespace Pole.Tester
{
    public class NegotiationTester
    {
        private readonly ILogger _logger;

        public NegotiationTester(ILogger logger)
        {
            _logger = logger;
        }

        public List<(string, string, bool, EthernetRates)> GetInterfacesNegotiation(ITikConnection connection, IMonitoreable<MonitorEthernetResults>[] negotiationReader)
        {
            var interfacesRunningNegotiation = new List<(string, string, bool, EthernetRates)>();

            foreach (var iface in negotiationReader)
            {
                var negoStatus = iface.MonitorOnce(connection);
                _logger.Information("Interface {Interface}, Autonegotiation {AutoStatus}, Full Dulplex {FullStatus}, Rate {RateStatus}",
                                    negoStatus.Name, negoStatus.AutoNegotiation, negoStatus.FullDuplex, negoStatus.Rate);
                interfacesRunningNegotiation.Add((negoStatus.Name, negoStatus.AutoNegotiation, negoStatus.FullDuplex, negoStatus.Rate));
            }

            return interfacesRunningNegotiation;
        }
    }
}
