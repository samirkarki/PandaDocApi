using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
//using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocument;
using PandaDoc.Models.GetDocuments;
using PandaDoc.Models.SendDocument;
using RestSharp;

namespace PandaDoc.Standard
{
    public class PandaDocHttpClient : IDisposable
    {
        private HttpClient httpClient;
        //private JsonMediaTypeFormatter jsonFormatter;
        private PandaDocBearerToken bearerToken;
        
        public PandaDocHttpClient()
        {
            HttpClient = new HttpClient();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public HttpClient HttpClient
        {
            get { return httpClient; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                httpClient = value;
            }
        }

        //public JsonMediaTypeFormatter JsonFormatter
        //{
        //    get { return jsonFormatter; }
        //    set
        //    {
        //        if (value == null) throw new ArgumentNullException("value");
        //        jsonFormatter = value;
        //    }
        //}

        public PandaDocBearerToken BearerToken
        {
            get { return bearerToken; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                bearerToken = value;

                httpClient.DefaultRequestHeaders.Clear();
                //httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken.AccessToken);
                httpClient.DefaultRequestHeaders.Add("Authorization", "API-Key " + bearerToken.ApiKey);
            }
        }

        public void SetBearerToken(PandaDocBearerToken value)
        {
            BearerToken = value;
        }

        //public async Task<PandaDocBearerToken> Login(string username, string password)
        //{
        //    if (username == null) throw new ArgumentNullException("username");
        //    if (password == null) throw new ArgumentNullException("password");

        //    var values = new Dictionary<string, string>
        //    {
        //        {"grant_type", "password"},
        //        {"username", username},
        //        {"password", password},
        //        {"client_id", settings.ClientId},
        //        {"client_secret", settings.ClientSecret},
        //        {"scope", "read write read+write"}
        //    };

        //    var content = new FormUrlEncodedContent(values);

        //    HttpResponseMessage httpResponse = await httpClient.PostAsync(ESignConfig.PandaDocAuthUrl + "oauth2/access_token", content);

        //    PandaDocHttpResponse<PandaDocBearerToken> response = await httpResponse.ToPandaDocResponseAsync<PandaDocBearerToken>();
        //    var response = await httpResponse.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<PandaDocBearerToken>(response);
        //}

        public async Task<GetDocumentsResponse> GetDocuments()
        {
            HttpResponseMessage httpResponse = await httpClient.GetAsync(ESignConfig.PandaDocApiUrl + "/public/v1/documents");

            //PandaDocHttpResponse<GetDocumentsResponse> response = await httpResponse.ToPandaDocResponseAsync<GetDocumentsResponse>();
            var response = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetDocumentsResponse>(response);
        }

        public async Task<CreateDocumentResponse> CreateDocument(byte[] fileContent, CreateDocumentRequest document)
        {
            var client = new RestClient("https://api.pandadoc.com/public/v1/documents");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Host", "api.pandadoc.com");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", $"API-Key {ESignConfig.PandaDocApiKey}");
            request.AddHeader("content-type", "multipart/form-data");

            request.AddFileBytes("file", fileContent, "panda.pdf", "application/pdf");
            var json = JsonConvert.SerializeObject(document);
            request.AddParameter("data", json);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created) {
                return  JsonConvert.DeserializeObject<CreateDocumentResponse>(response.Content);
            }
            return new CreateDocumentResponse();
        }

        public async Task<CreateDocumentResponse> CreateDocument(byte[] fileContent, string document)
        {
            var client = new RestClient(ESignConfig.PandaDocApiUrl + "/public/v1/documents");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Host", "api.pandadoc.com");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", $"API-Key {ESignConfig.PandaDocApiKey}");
            request.AddHeader("content-type", "multipart/form-data");

            request.AddFileBytes("file", fileContent, "panda.pdf", "application/pdf");
            request.AddParameter("data", document);
            request.AlwaysMultipartFormData = true;
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<CreateDocumentResponse>(response.Content);
            }
            return new CreateDocumentResponse();
        }

        public async Task<GetDocumentResponse> GetDocument(string uuid)
        {
            var httpResponse = await httpClient.GetAsync(ESignConfig.PandaDocApiUrl + "/public/v1/documents/" + uuid);
            //return JsonConvert.DeserializeObject<GetDocumentResponse>(httpResponse.Content.ToString());
            var response = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetDocumentResponse>(response);
        }

        public dynamic GetDocumentDetail(string uuid)
        {
            var client = new RestClient($"{ESignConfig.PandaDocApiUrl}/public/v1/documents/{uuid}/details");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"API-Key {ESignConfig.PandaDocApiKey}");
            var response = client.Execute(request);
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }

        public async Task<SendDocumentResponse> SendDocument(string uuid, SendDocumentRequest request)
        {
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(request), UnicodeEncoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await httpClient.PostAsync(ESignConfig.PandaDocApiUrl + "/public/v1/documents/" + uuid + "/send", httpContent);

            //PandaDocHttpResponse<SendDocumentResponse> response = await httpResponse.ToPandaDocResponseAsync<SendDocumentResponse>();

            var response = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SendDocumentResponse>(response);
        }

        public async Task<ShareDocumentResponse> ShareDocument(string uuid, ShareDocumentRequest request)
        {
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(request), UnicodeEncoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await httpClient.PostAsync(ESignConfig.PandaDocApiUrl + "/public/v1/documents/" + uuid + "/session", httpContent);

            // PandaDocHttpResponse<ShareDocumentResponse> response = await httpResponse.ToPandaDocResponseAsync<ShareDocumentResponse>();
            var response = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ShareDocumentResponse>(response);
            //return response;
        }

        public async Task<PandaDocHttpResponse> DeleteDocument(string uuid)
        {
            HttpResponseMessage httpResponse = await httpClient.DeleteAsync(ESignConfig.PandaDocApiUrl + "/public/v1/documents/" + uuid);

            //PandaDocHttpResponse response = await httpResponse.ToPandaDocResponseAsync();
            var response = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PandaDocHttpResponse>(response);
            //return response;
        }
    }

    public static class MultiPartFormDataContentExtensions
    {
        public static void Add(this MultipartFormDataContent form, HttpContent content, object formValues)
        {
            Add(form, content, formValues);
        }

        public static void Add(this MultipartFormDataContent form, HttpContent content, string name, object formValues)
        {
            Add(form, content, formValues, name: name);
        }

        public static void Add(this MultipartFormDataContent form, HttpContent content, string name, string fileName, object formValues)
        {
            Add(form, content, formValues, name: name, fileName: fileName);
        }

        private static void Add(this MultipartFormDataContent form, HttpContent content, object formValues, string name = null, string fileName = null)
        {
            var header = new ContentDispositionHeaderValue("form-data");
            header.Name = name;
            header.FileName = fileName;
            header.FileNameStar = fileName;

            //var headerParameters = new Dictionary<string, string>(formValues);
            //foreach (var parameter in headerParameters)
            //{
            //    header.Parameters.Add(new NameValueHeaderValue(parameter.Key, parameter.Value.ToString()));
            //}

            content.Headers.ContentDisposition = header;
            form.Add(content);
        }
    }
}