using Eternet.Mikrotik;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Interface.Ethernet;
using Eternet.Mikrotik.Entities.Interface.Ethernet.Poe;
using Eternet.Mikrotik.Entities.Ip;
using Eternet.Mikrotik.Entities.ReadWriters;
using Eternet.Mikrotik.Entities.Tool;
using Moq;
using Serilog;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;


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
                },
                new InterfaceEthernet
                {
                    Name = "ether7",
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
                    Address4 = "192.168.0.7",
                    MacAddress = "64:D1:54:85:89:07",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether1"
                },
                new IpNeighbor
                {
                    Address4 = "192.168.0.1",
                    MacAddress = "64:D1:54:85:89:01",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether1"
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
                    Address4 = "192.168.0.5",
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
                },
                new IpNeighbor
                {
                    Address4 = "",
                    MacAddress = "64:D1:54:85:89:08",
                    Board = "RB3011UiAS",
                    Interface = "ether7"
                }
            };
        }

        private static List<(string, string)> GetFakeEthToTestResults()
        {
            var fakeInterfacesToTestResults = new List<(string, string)>
            {
                ("ether2", "192.168.0.2"),("ether3", "192.168.0.5"),
                ("ether4", "192.168.0.4"),("ether6", "192.168.0.6")
            };

            return fakeInterfacesToTestResults;
        }

        private static readonly MonitorPoeResults Ether4FakeMonitorPoeResults = new MonitorPoeResults
        {
            Name = "ether4",
            PoeOut = EthernetPoeModes.AutoOn,
            PoeOutStatus = EthernetPoeStatus.ShortCircuit,
            PoeOutCurrent = "123mA",
            PoeOutVoltage = "24.3V",
            PoeOutPower = "8W"
        };

        private static readonly MonitorPoeResults Ether6FakeMonitorPoeResults = new MonitorPoeResults
        {
            Name = "ether6",
            PoeOut = EthernetPoeModes.AutoOn,
            PoeOutStatus = EthernetPoeStatus.PoweredOn,
            PoeOutCurrent = "123mA",
            PoeOutVoltage = "24.3V",
            PoeOutPower = "8W"
        };

        private static List<(string, EthernetPoeStatus)> GetFakeInterfacesPoeStatusResults()
        {
            var fakeInterfacesPoeStatusResults = new List<(string, EthernetPoeStatus)>
            {
                ("ether4", EthernetPoeStatus.ShortCircuit),("ether6", EthernetPoeStatus.PoweredOn)
            };

            return fakeInterfacesPoeStatusResults;
        }

        private static readonly MonitorEthernetResults Ether2FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether2",
            AutoNegotiation = "done",
            FullDuplex = true,
            Rate = EthernetRates.Rate100Mbps
        };

        private static readonly MonitorEthernetResults Ether3FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether3",
            AutoNegotiation = "done",
            FullDuplex = false,
            Rate = EthernetRates.Rate1Gbps
        };

        private static readonly MonitorEthernetResults Ether4FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether4",
            AutoNegotiation = "done",
            FullDuplex = true,
            Rate = EthernetRates.Rate10Mbps
        };

        private static readonly MonitorEthernetResults Ether6FakeMonitorNegotiation = new MonitorEthernetResults
        {
            Name = "ether6",
            AutoNegotiation = "done",
            FullDuplex = true,
            Rate = EthernetRates.Rate1Gbps
        };

        private static List<(string, string, bool, EthernetRates)> GetFakeInterfacesNegotiation()
        {
            var fakeInterfacesNegociation = new List<(string, string, bool, EthernetRates)>
            {
               ("ether2", "done", true, EthernetRates.Rate100Mbps), ("ether3", "done", false, EthernetRates.Rate1Gbps),
               ("ether4", "done", true, EthernetRates.Rate10Mbps), ("ether6", "done", true, EthernetRates.Rate1Gbps)
            };

            return fakeInterfacesNegociation;
        }

        private static List<(string iface, string bandwith)> GetFakeBandwidthTestInterfaces()
        {
            var fakeBandwithTestInterfaces = new List<(string iface, string bandwith)> { ("ether2", "80M"), ("ether6", "120M") };

            return fakeBandwithTestInterfaces;
        }

        private static readonly BandwidthTestResult[] Ether2FakeBandwidthTestResult = new BandwidthTestResult[1] {new BandwidthTestResult
            {
                Duration = "20s",
                TxTotalAverage = 80,
                RxTotalAverage = 80,
                LostPackets = 0
            }
        };

        private static readonly BandwidthTestResult[] Ether6FakeBandwidthTestResult = new BandwidthTestResult[1] {new BandwidthTestResult
            {
                Duration = "20s",
                TxTotalAverage = 120,
                RxTotalAverage = 120,
                LostPackets = 0
            }
        };

        private ITestOutputHelper _out;
        private static int _count;
        private static int _count2;
        private static int _count3;

        private readonly Mock<IEntityReader<InterfaceEthernet>> _ethReader;

        private readonly Mock<ITikConnection> _connection;

        private readonly Mock<ILogger> _logger;

        #endregion

        public PoleTesterUnitTests(ITestOutputHelper output)
        {
            _out = output;
            _count = 0;
            _count2 = 0;
            _count3 = 0;

            _connection = new Mock<ITikConnection>();

            _ethReader = new Mock<IEntityReader<InterfaceEthernet>>();

            _logger = new Mock<ILogger>();
        }

        [Fact]
        public void ExpectedNeighborsOnRunningInterfaces()
        {
            var ethlist = GetFakeEthList();

            _ethReader.Setup(r => r.GetAll()).Returns(ethlist.ToArray);

            var fakeInterfaceListToTestResults = GetFakeEthToTestResults();

            var neighList = GetFakeNeighList();

            var neigReader = new Mock<IEntityReader<IpNeighbor>>();

            neigReader.Setup(r => r.GetAll()).Returns(neighList.ToArray);

            var poleTester = new PoleTester(_logger.Object, _connection.Object);

            var interfaceToTest = poleTester.GetNeighborsOnRunningInterfaces
                (_ethReader.Object, neigReader.Object);

            Assert.Equal(fakeInterfaceListToTestResults, interfaceToTest);
        }

        [Fact]
        public void ExpectedInterfacesPoeStatus()
        {
            var fakeInterfacesPoeStatusResults = GetFakeInterfacesPoeStatusResults();

            var eth4 = new Mock<EthernetPoe>();
            eth4.Object.Name = "ether4";
            eth4.Setup(r => r.MonitorOnce(It.IsAny<ITikConnection>()))
                .Returns<ITikConnection>(MonitorPoEOnceLocal);

            var eth6 = new Mock<EthernetPoe>();
            eth6.Object.Name = "ether6";
            eth6.Setup(r => r.MonitorOnce(It.IsAny<ITikConnection>()))
                .Returns<ITikConnection>(MonitorPoEOnceLocal);

            var list = new List<EthernetPoe> { eth4.Object, eth6.Object };

            var poeReader = new Mock<IEntityReader<EthernetPoe>>();

            poeReader.Setup(r => r.GetAll()).Returns(() =>
            {
                var results = list.ToArray();
                return results;
            });

            var poleTester = new PoleTester(_logger.Object, _connection.Object);

            var interfacesPoeStatus = poleTester.GetInterfacesPoeStatus(poeReader.Object);

            Assert.Equal(fakeInterfacesPoeStatusResults, interfacesPoeStatus);
        }

        [Fact]
        public void ExpectedInterfaceNegotiation()
        {
            var fakeInterfacesNegotiation = GetFakeInterfacesNegotiation();

            var eth2 = new Mock<InterfaceEthernet>();
            eth2.Object.Name = "ether2";
            eth2.Object.Running = true;
            eth2.Setup(r => r.MonitorOnce(It.IsAny<ITikConnection>()))
                .Returns<ITikConnection>(MonitorOnceLocal);

            var eth3 = new Mock<InterfaceEthernet>();
            eth3.Object.Name = "ether3";
            eth3.Object.Running = true;
            eth3.Setup(r => r.MonitorOnce(It.IsAny<ITikConnection>()))
                .Returns<ITikConnection>(MonitorOnceLocal);

            var eth4 = new Mock<InterfaceEthernet>();
            eth4.Object.Name = "ether4";
            eth4.Object.Running = true;
            eth4.Setup(r => r.MonitorOnce(It.IsAny<ITikConnection>()))
                .Returns<ITikConnection>(MonitorOnceLocal);

            var eth5 = new Mock<InterfaceEthernet>();
            eth5.Object.Name = "ether5";
            eth5.Object.Running = false;
            eth5.Setup(r => r.MonitorOnce(It.IsAny<ITikConnection>()))
                .Returns<ITikConnection>(MonitorOnceLocal);

            var eth6 = new Mock<InterfaceEthernet>();
            eth6.Object.Name = "ether6";
            eth6.Object.Running = true;
            eth6.Setup(r => r.MonitorOnce(It.IsAny<ITikConnection>()))
                .Returns<ITikConnection>(MonitorOnceLocal);

            var list = new List<InterfaceEthernet>
                { eth2.Object, eth3.Object, eth4.Object, eth5.Object, eth6.Object };

            _ethReader.Setup(r => r.GetAll()).Returns(() =>
            {
                var results = list.ToArray();
                return results;
            });

            var poleTester = new PoleTester(_logger.Object, _connection.Object);

            var interfacesNegotiation = poleTester.GetInterfacesNegotiation(_ethReader.Object);

            Assert.Equal(fakeInterfacesNegotiation, interfacesNegotiation);
        }

        [Fact]
        public void ExpectedBandwidthTestInterfaces()
        {
            var fakeInterfaceListToTestResults = GetFakeEthToTestResults();

            var fakeInterfacesNegotiation = GetFakeInterfacesNegotiation();

            var fakeBandwidthTestInterfaces = GetFakeBandwidthTestInterfaces();

            var btest = new Mock<IBandwidthTest>();
            btest.Setup(b => b.Run(It.IsAny<BandwidthTestParameters>(), It.IsAny<int>()))
                .Returns(RunLocal);

            var poleTester = new PoleTester(_logger.Object, _connection.Object);

            var btResults = poleTester.RunBandwidthTests
                (fakeInterfaceListToTestResults, fakeInterfacesNegotiation, btest.Object);

            Assert.Equal(fakeBandwidthTestInterfaces, btResults);

        }

        private static MonitorEthernetResults MonitorOnceLocal(ITikConnection arg)
        {
            _count++;
            switch (_count)
            {
                case 1:
                    return Ether2FakeMonitorNegotiation;
                case 2:
                    return Ether3FakeMonitorNegotiation;
                case 3:
                    return Ether4FakeMonitorNegotiation;
                default:
                    return Ether6FakeMonitorNegotiation;
            }
        }

        private static BandwidthTestResult[] RunLocal()
        {
            _count2++;
            return _count2 == 1 ? Ether2FakeBandwidthTestResult : Ether6FakeBandwidthTestResult;
        }

        private static MonitorPoeResults MonitorPoEOnceLocal(ITikConnection arg)
        {
            _count3++;
            return _count3 == 1 ? Ether4FakeMonitorPoeResults : Ether6FakeMonitorPoeResults;
        }

    }

}
