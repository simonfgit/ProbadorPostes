using Eternet.Mikrotik;

namespace Pole.Tester.Integration.Tests
{
    public class PoleTesterIntegrationTests
    {
        private readonly ITikConnection _connection;

        private const string Host = "192.168.0.5";
        private const string ApiUser = "admin";
        private const string ApiPass = "";

        private static ITikConnection GetMikrotikConnection(string host, string user, string pass)
        {
            var connection = ConnectionFactory.CreateConnection(TikConnectionType.Api);
            connection.Open(host, user, pass);
            return connection;
        }

        public PoleTesterIntegrationTests()
        {
            _connection = GetMikrotikConnection(Host, ApiUser, ApiPass);
        }

        //[Fact]
        //public void BandwithTestShouldBeRun()
        //{
        //    var ethReader = _connection.CreateEntityReader<InterfaceEthernet>();

        //    var neighReader = _connection.CreateEntityReader<IpNeighbor>();

        //    var poleTester = new PoleTester(Serilog.Log.Logger, _connection);

        //    var ifacesToTest = poleTester.GetNeighborsOnRunningInterfaces(ethReader, neighReader);

        //    var ifacesNegotiation = poleTester.GetInterfacesNegotiation(ethReader);

        //    var btList = poleTester.RunBandwithTests(ifacesToTest, ifacesNegotiation);


        //}

    }

}
