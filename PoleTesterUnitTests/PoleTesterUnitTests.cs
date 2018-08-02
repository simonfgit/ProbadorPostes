using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Interface.Ethernet;
using Eternet.Mikrotik.Entities.Interface.Ethernet.Poe;
using Eternet.Mikrotik.Entities.Ip;
using Eternet.Mikrotik.Entities.ReadWriters;
using Moq;
using Serilog;
using System.Collections.Generic;
using Xunit;


namespace Pole.Tester.Unit.Tests
{
    public class PoleTesterUnitTests
    {
        #region private_fields&consts

        private static List<InterfaceEthernet> GetFakeEthList()
        {
            return new List<InterfaceEthernet>
            {
                new InterfaceEthernet
                {
                    Name = "ether2",
                    Running = true
                },
                new InterfaceEthernet
                {
                    Name = "ether3",
                    Running = true
                },
                new InterfaceEthernet
                {
                    Name = "ether4",
                    Running = true
                },
                new InterfaceEthernet
                {
                    Name = "ether5",
                    Running = false
                },
                new InterfaceEthernet
                {
                    Name = "ether6",
                    Running = true
                }
            };
        }

        private static List<IpNeighbor> GetFakeNeighList()
        {
            return new List<IpNeighbor>
            {
                new IpNeighbor
                {
                    Address4 = "192.168.0.1",
                    MacAddress = "64:D1:54:85:89:01",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether2"
                },
                new IpNeighbor
                {
                    Address4 = "192.168.0.2",
                    MacAddress = "64:D1:54:85:89:02",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether2"
                },
                new IpNeighbor
                {
                    Address4 = "",
                    MacAddress = "64:D1:54:85:89:03",
                    Board = "RB3011UiAS",
                    Interface = "ether3"
                },
                new IpNeighbor
                {
                    Address4 = "192.168.0.4",
                    MacAddress = "64:D1:54:85:89:04",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether4"
                },
                new IpNeighbor
                {
                    Address4 = "192.168.0.6",
                    MacAddress = "64:D1:54:85:89:06",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether6"
                }
            };
        }

        private static List<(string, string)> GetFakeEthToTestResults()
        {
            var fakeInterfacesToTestResults = new List<(string, string)>
            {
                ("ether4", "192.168.0.4"),("ether6", "192.168.0.6")
            };

            return fakeInterfacesToTestResults;
        }

        private static MonitorPoeResults _ether4FakeMonitorPoeResults = new MonitorPoeResults
        {
            Name = "ether4",
            PoeOut = EthernetPoeModes.AutoOn,
            PoeOutStatus = EthernetPoeStatus.ShortCircuit,
            PoeOutCurrent = "123mA",
            PoeOutVoltage = "24.3V",
            PoeOutPower = "8W"
        };

        private static MonitorPoeResults _ether6FakeMonitorPoeResults = new MonitorPoeResults
        {
            Name = "ether6",
            PoeOut = EthernetPoeModes.AutoOn,
            PoeOutStatus = EthernetPoeStatus.PoweredOn,
            PoeOutCurrent = "123mA",
            PoeOutVoltage = "24.3V",
            PoeOutPower = "8W"
        };

        private readonly List<(string, string)> _fakeIinterfaceListToTestResults;

        private readonly List<(string, string)> _interfaceToTest;

        private static List<(string, EthernetPoeStatus)> GetFakeInterfacesPoeStatusResults()
        {
            var fakeInterfacesPoeStatusResults = new List<(string, EthernetPoeStatus)>
            {
                ("ether4", EthernetPoeStatus.ShortCircuit),("ether6", EthernetPoeStatus.PoweredOn)
            };

            return fakeInterfacesPoeStatusResults;
        }

        private readonly List<(string, EthernetPoeStatus)> _fakeInterfacesPoeStatusResults;

        private readonly List<(string, EthernetPoeStatus)> _interfacesPoeStatus;

        private readonly List<(string, string, bool, EthernetRates)> _fakeInterfacesNegotiation;

        private readonly List<(string, string, bool, EthernetRates)> _interfacesNegotiation;

        private static MonitorEthernetResults _ether2FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether2",
            AutoNegotiation = "done",
            FullDuplex = true,
            Rate = EthernetRates.Rate100Mbps
        };

        private static MonitorEthernetResults _ether3FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether3",
            AutoNegotiation = "done",
            FullDuplex = false,
            Rate = EthernetRates.Rate1Gbps
        };

        private static MonitorEthernetResults _ether4FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether4",
            AutoNegotiation = "done",
            FullDuplex = true,
            Rate = EthernetRates.Rate1Gbps
        };

        private static MonitorEthernetResults _ether6FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether6",
            AutoNegotiation = "done",
            FullDuplex = false,
            Rate = EthernetRates.Rate1Gbps
        };

        private static List<(string, string, bool, EthernetRates)> GetFakeInterfacesNegotiation()
        {
            var fakeInterfacesNegociation = new List<(string, string, bool, EthernetRates)>
            {
               ("ether2", "done", true, EthernetRates.Rate100Mbps), ("ether3", "done", false, EthernetRates.Rate1Gbps),
               ("ether4", "done", true, EthernetRates.Rate1Gbps), ("ether6", "done", false, EthernetRates.Rate1Gbps)
            };

            return fakeInterfacesNegociation;
        }


        //private static void ConfigureLogger()
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Path.Combine(AppContext.BaseDirectory))
        //        .AddJsonFile("appsettings.json", optional: false);

        //    var cfg = builder.Build();

        //    Log.Logger = new LoggerConfiguration()
        //        .ReadFrom.Configuration(cfg)
        //        .CreateLogger();
        //}

        #endregion

        public PoleTesterUnitTests()
        {
            //Arrange
            var ethlist = GetFakeEthList();
            var neighList = GetFakeNeighList();

            _fakeIinterfaceListToTestResults = GetFakeEthToTestResults();
            _fakeInterfacesPoeStatusResults = GetFakeInterfacesPoeStatusResults();
            _fakeInterfacesNegotiation = GetFakeInterfacesNegotiation();

            var logger = new Mock<ILogger>();

            var connection = new Mock<ITikConnection>();

            var eth4Poe = new Mock<IMonitoreable<MonitorPoeResults>>();

            var eth6Poe = new Mock<IMonitoreable<MonitorPoeResults>>();

            var poeList = new List<IMonitoreable<MonitorPoeResults>> { eth4Poe.Object, eth6Poe.Object }.ToArray();

            eth4Poe.Setup(c => c.MonitorOnce(It.IsAny<ITikConnection>())).Returns(_ether4FakeMonitorPoeResults);
            eth6Poe.Setup(c => c.MonitorOnce(connection.Object)).Returns(_ether6FakeMonitorPoeResults);

            var ethReader = new Mock<IEntityReader<InterfaceEthernet>>();
            ethReader.Setup(r => r.GetAll()).Returns(ethlist.ToArray);

            var neigReader = new Mock<IEntityReader<IpNeighbor>>();
            neigReader.Setup(r => r.GetAll()).Returns(neighList.ToArray);

            var eth2Negotiation = new Mock<IMonitoreable<MonitorEthernetResults>>();

            var eth3Negotiation = new Mock<IMonitoreable<MonitorEthernetResults>>();

            var eth4Negotiation = new Mock<IMonitoreable<MonitorEthernetResults>>();

            var eth6Negotiation = new Mock<IMonitoreable<MonitorEthernetResults>>();

            var negotiationList = new List<IMonitoreable<MonitorEthernetResults>> { eth2Negotiation.Object, eth3Negotiation.Object, eth4Negotiation.Object, eth6Negotiation.Object }.ToArray();

            eth2Negotiation.Setup(c => c.MonitorOnce(It.IsAny<ITikConnection>())).Returns(_ether2FakeMonitorNegotiation);
            eth3Negotiation.Setup(c => c.MonitorOnce(It.IsAny<ITikConnection>())).Returns(_ether3FakeMonitorNegotiation);
            eth4Negotiation.Setup(c => c.MonitorOnce(It.IsAny<ITikConnection>())).Returns(_ether4FakeMonitorNegotiation);
            eth6Negotiation.Setup(c => c.MonitorOnce(It.IsAny<ITikConnection>())).Returns(_ether6FakeMonitorNegotiation);

            //Act
            var poleTester = new PoleTester(logger.Object);
            _interfaceToTest = poleTester.GetNeighborsOnRunningInterfaces(ethReader.Object, neigReader.Object);
            _interfacesPoeStatus = poleTester.GetInterfacesPoeStatus(connection.Object, poeList);
            var negotiationTester = new NegotiationTester(logger.Object);
            _interfacesNegotiation = negotiationTester.GetInterfacesNegotiation(connection.Object, negotiationList);
        }

        //Assert

        [Fact]
        public void ExpectedNeighborsOnRunningInterfaces()
        {
            Assert.Equal(_fakeIinterfaceListToTestResults, _interfaceToTest);
        }

        [Fact]
        public void ExpectedInterfacesPoeStatus()
        {
            Assert.Equal(_fakeInterfacesPoeStatusResults, _interfacesPoeStatus);
        }

        [Fact]
        public void ExpectedInterfaceNegotiation()
        {
            Assert.Equal(_fakeInterfacesNegotiation, _interfacesNegotiation);
        }


    }
}
