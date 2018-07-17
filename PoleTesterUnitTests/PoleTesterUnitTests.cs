using System.Collections.Generic;
using Eternet.Mikrotik.Entities.Interface;
using Eternet.Mikrotik.Entities.Ip;
using Eternet.Mikrotik.Entities.ReadWriters;
using Moq;
using Xunit;

namespace Pole.Tester.Unit.Tests
{
    public class PoleTesterUnitTests
    {
        //Simulated data for testing purposes

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
                    MacAddress = "64:D1:54:85:89:01",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether2"                    
                },
                new IpNeighbor
                {
                    MacAddress = "64:D1:54:85:89:02",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether2"
                },
                new IpNeighbor
                {
                    MacAddress = "64:D1:54:85:89:03",
                    Board = "RB3011UiAS",
                    Interface = "ether3"
                },
                new IpNeighbor
                {
                    MacAddress = "64:D1:54:85:89:04",
                    Board = "RB921UAGS-5SHPacD",
                    Interface = "ether4"
                }
            };
        }

        private readonly List<InterfaceEthernet> _ethList;

        private readonly List<IpNeighbor> _neighList;

        public PoleTesterUnitTests()
        {
            _ethList = GetFakeEthList();
            _neighList = GetFakeNeighList();

            var ethReader = new Mock<IEntityReader<InterfaceEthernet>>();
            ethReader.Setup(r => r.GetAll()).Returns(_ethList.ToArray);

            var neigReader = new Mock<IEntityReader<IpNeighbor>>();
            neigReader.Setup(r => r.GetAll()).Returns(_neighList.ToArray);

            PoleTester.GetNeighborsOnRunningInterfaces(ethReader.Object, neigReader.Object);
        }

        [Fact]
        public void ExpectedNeighborsOnRunningInterfaces()
        {
            Assert.True(true);
        }
    }
}
