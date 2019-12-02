using Esign.Client;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using PandaDoc.Models.CreateDocument;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Client
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EsignApiClient esignApiClient = new EsignApiClient();
            var docs = esignApiClient.GetDocuments().Result;
            var file = File.OpenRead("D:\\panda.pdf");
            byte[] fileContent;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileContent = ms.ToArray();
            }
            var upload = esignApiClient.UploadDocument(fileContent, new CreateDocumentRequest());
            // discover endpoints from metadata
        //    var client = new HttpClient();

        //    var disco = client.GetDiscoveryDocumentAsync("http://localhost:5000").Result;
        //    if (disco.IsError)
        //    {
        //        identity.InnerText = disco.Error;
        //        return;
        //    }

        //    // request token
        //    var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //    {
        //        Address = disco.TokenEndpoint,
        //        ClientId = "ipd",
        //        ClientSecret = "secret",
        //        Scope = "esign"
        //    }).Result;

        //    if (tokenResponse.IsError)
        //    {
        //        identity.InnerText = tokenResponse.Error;
        //        return;
        //    }

        //    identity.InnerText = tokenResponse.Json.ToString();
        //    Console.WriteLine("\n\n");

        //    // call api
        //    var apiClient = new HttpClient();
        //    apiClient.SetBearerToken(tokenResponse.AccessToken);

        //    var response = apiClient.GetAsync("https://localhost:44307/identity").Result;
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        apiresult.InnerText = response.ToString();
        //    }
        //    else
        //    {
        //        var content = response.Content.ReadAsStringAsync().Result;
        //        apiresult.InnerText = content;
        //    }

        }
    }
}