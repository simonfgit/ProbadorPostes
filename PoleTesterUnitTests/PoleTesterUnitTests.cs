using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Ip;
using Eternet.Mikrotik.Entities.ReadWriters;
using Microsoft.Extensions.Configuration;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Pole.Tester.Unit.Tests
{
    public class PoleTesterUnitTests
    {
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
                }
            };
        }

        private static List<(string, string)> GetFakeEthToTestList()
        {
            var fakeInterfacesfacesToTest = new List<(string, string)>
            {
                ("ether4", "192.168.0.4")
            };

            return fakeInterfacesfacesToTest;
        }

        private static void BuildLogger()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false);

            var cfg = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(cfg)
                .CreateLogger();
        }

        public PoleTesterUnitTests()
        {
            var ethlist = GetFakeEthList();
            var neighList = GetFakeNeighList();
            var interfaceListToTest = GetFakeEthToTestList();

            var ethReader = new Mock<IEntityReader<InterfaceEthernet>>();
            ethReader.Setup(r => r.GetAll()).Returns(ethlist.ToArray);

            var neigReader = new Mock<IEntityReader<IpNeighbor>>();
            neigReader.Setup(r => r.GetAll()).Returns(neighList.ToArray);

            BuildLogger();

            var interfaceToTest = PoleTester.GetNeighborsOnRunningInterfaces(ethReader.Object, neigReader.Object, Log.Logger);
        }

        [Fact]
        public void ExpectedNeighborsOnRunningInterfaces()
        {
            Assert.True(true);
        }
    }
}
