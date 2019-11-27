using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Client
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync("http://localhost:5001").Result;
            if (disco.IsError)
            {
            div.InnerText = disco.Error;
                return;
            }

            // request token
            var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "ipd",
                ClientSecret = "secret",
                Scope = "esign"
            }).Result;

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            div.InnerText = tokenResponse.Json.ToString();
        }
    }
}