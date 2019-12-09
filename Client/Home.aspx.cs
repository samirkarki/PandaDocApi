using Esign.Client;
using IdentityModel.Client;
using Newtonsoft.Json;
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
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocPdf_FieldTags.pdf";

        protected void Page_Load(object sender, EventArgs e)
        {
            //EsignApiClient esignApiClient = new EsignApiClient();
            //var docs = esignApiClient.GetDocuments().Result;
            //var file = File.OpenRead("D:\\panda.pdf");
            //byte[] fileContent;
            //using (var ms = new MemoryStream())
            //{
            //    file.CopyTo(ms);
            //    fileContent = ms.ToArray();
            //}
            //var upload = esignApiClient.UploadDocument(fileContent, new CreateDocumentRequest());
            // discover endpoints from metadata
            //var client = new HttpClient();

            //var disco = client.GetDiscoveryDocumentAsync("http://localhost:5000").Result;
            //if (disco.IsError)
            //{
            //    identity.InnerText = disco.Error;
            //    return;
            //}

            //// request token
            //var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            //{
            //    Address = disco.TokenEndpoint,
            //    ClientId = "ipd",
            //    ClientSecret = "secret",
            //    Scope = "esign"
            //}).Result;

            //if (tokenResponse.IsError)
            //{
            //    identity.InnerText = tokenResponse.Error;
            //    return;
            //}

            //identity.InnerText = tokenResponse.Json.ToString();
            //Console.WriteLine("\n\n");

            //// call api
            //var apiClient = new HttpClient();
            //apiClient.SetBearerToken(tokenResponse.AccessToken);

            //var response = apiClient.GetAsync("https://localhost:44307/identity").Result;
            //if (!response.IsSuccessStatusCode)
            //{
            //    apiresult.InnerText = response.ToString();
            //}
            //else
            //{
            //    var content = response.Content.ReadAsStringAsync().Result;
            //    apiresult.InnerText = content;
            //}
            EsignApiClient esignClient = new EsignApiClient();
            byte[] fileContent = File.ReadAllBytes("D:\\panda.pdf");
            var docRequest = CreateDocumentRequest();
            var response = esignClient.UploadDocument(fileContent, docRequest);
            apiresult.InnerText = Convert.ToString(response);
        }


        private CreateDocumentRequest CreateDocumentRequest()
        {
            return new CreateDocumentRequest
            {
                Name = "Sample Document 1",
                Url = SampleDocUrl,
                Recipients = new[]
                {
                    new Recipient
                    {
                        Email = "samir.ctec@gmail.com",
                        FirstName = "Samir",
                        LastName = "Ctec",
                        Role = "role1",
                    },
                    new Recipient
                    {
                        Email = "samir@teksewa.com",
                        FirstName = "Samir",
                        LastName = "Teksewa",
                        Role = "role2",
                    }
                },
                Fields = new Dictionary<string, Field>
                {
                    {"optId", new Field {Title = "Field 1"}}
                }
            };
        }
    }
}