using NUnit.Framework;

namespace PandaDoc.Tests
{
    [TestFixture]
    public class PandaDocBearerTokenTests
    {
        [Test]
        public void AccessToken()
        {
            var token = new PandaDocBearerToken { AccessToken = "test" };
            
            Assert.AreEqual("test", token.AccessToken);
        }

        [Test]
        public void RefreshToken()
        {
            var token = new PandaDocBearerToken { RefreshToken = "test" };

            Assert.AreEqual("test", token.RefreshToken);
        }

        [Test]
        public void ApiKey()
        {
            var token = new PandaDocBearerToken { ApiKey = "test" };

            Assert.AreEqual("test", token.ApiKey);
        }
    }
}
