using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;
using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocuments;
using PandaDoc.Models.SendDocument;
using RestSharp;

namespace Esign.Client
{
    public class EsignApiClient
    {
        private static string accessToken;
        private static object accessTokenLock = new object();
        private static string esignApiUrl;
        private static string idServer;
        private static string clientId;
        private static string clientSecret;
        private static string clientScope;

        public EsignApiClient()
        {
            
            lock (accessTokenLock)
            {
                esignApiUrl = ConfigurationManager.AppSettings["EsignApiUrl"];
                idServer = ConfigurationManager.AppSettings["IdServerUrl"];
                clientId = ConfigurationManager.AppSettings["IdServerClientId"];
                clientSecret = ConfigurationManager.AppSettings["IdServerClientSecret"];
                clientScope = ConfigurationManager.AppSettings["IdServerScope"];

                var client = new HttpClient();
                var disco = client.GetDiscoveryDocumentAsync(idServer).Result;
                if (!disco.IsError)
                {
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        client.SetBearerToken(accessToken);
                        var response = client.GetAsync($"{esignApiUrl}/identity").Result;
                        if (response.IsSuccessStatusCode)
                            return;
                    }

                    var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Scope = clientScope
                    }).Result;

                    if (!tokenResponse.IsError)
                    {
                        accessToken = tokenResponse.AccessToken;
                    }
                }
            }
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("ESign Authentication error!");

        }

        public async Task<GetDocumentsResponse> GetDocuments()
        {
            HttpClient client = new HttpClient();
            client.SetBearerToken(accessToken);
            var response = await client.GetAsync($"{esignApiUrl}/documents");
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetDocumentsResponse>(responseString);

        }

        public dynamic UploadDocument(byte[] pdfBytes, CreateDocumentRequest document)
        {
            var client = new RestClient($"{esignApiUrl}/uploadfileWithJson");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("content-type", "multipart/form-data");

            request.AddFileBytes("file", pdfBytes, "panda.pdf", "application/pdf");
            var json = JsonConvert.SerializeObject(document);
            request.AddParameter("createDocumentRequestJson", json);
            request.AlwaysMultipartFormData = true;
            IRestResponse response =  client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<dynamic>(response.Content);
            }
            return new CreateDocumentResponse();
        }

        public async Task<dynamic> ShareDocument(string documentId, string recipientEmail)
        {
            HttpClient client = new HttpClient();
            var shareRequest = new ShareDocumentRequest
            {
                Recipient = recipientEmail,
            };
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(shareRequest), UnicodeEncoding.UTF8, "application/json");

            client.SetBearerToken(accessToken);
            var response = await client.PostAsync($"{esignApiUrl}/api/sharedocument/{documentId}", httpContent);
            return JsonConvert.DeserializeObject<dynamic>(response.Content.ToString());
        }
    }
}
