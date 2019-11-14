using System.Configuration;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PandaDoc.Tests
{
    [TestFixture]
    public abstract class BaseTest
    {
        protected readonly string Username = ConfigurationManager.AppSettings["pandadoc:username"];
        protected readonly string Password = ConfigurationManager.AppSettings["pandadoc:password"];
        protected readonly string SampleDocUrl = ConfigurationManager.AppSettings["pandadoc:sampledocurl"];

        protected async Task<PandaDocHttpClient> EnsureLoggedIn()
        {
            var settings = new PandaDocHttpClientSettings();
            var client = new PandaDocHttpClient(settings);

            PandaDocHttpResponse<PandaDocBearerToken> login = await client.Login(username: Username, password: Password);
            login.Value.ApiKey = "c6caae24740bb7bfffc0895f27bbf1ca7fe6bbe9";
            client.SetBearerToken(login.Value);

            return client;
        }
    }
}