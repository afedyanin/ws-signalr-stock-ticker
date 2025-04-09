using PrimeConnector.Extensions;
using PrimeConnector.Model;

namespace PrimeConnectorTests
{
    public class SubscribeCommandTests
    {
        [Test]
        public void CanSerializeCommand()
        {
            var json = new SubscribeCommand("FUT.QCL.C").ToJson();
            Assert.That(json, Is.Not.Null);
            Console.WriteLine(json);
        }
    }
}