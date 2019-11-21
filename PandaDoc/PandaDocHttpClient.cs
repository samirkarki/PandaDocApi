using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocument;
using PandaDoc.Models.GetDocuments;
using PandaDoc.Models.SendDocument;
using RestSharp;

namespace PandaDoc
{
    public class PandaDocHttpClient : IDisposable
    {
        private PandaDocHttpClientSettings settings;
        private HttpClient httpClient;
        private JsonMediaTypeFormatter jsonFormatter;
        private PandaDocBearerToken bearerToken;

        public PandaDocHttpClient()
            : this(new PandaDocHttpClientSettings())
        {
        }

        public PandaDocHttpClient(PandaDocHttpClientSettings settings)
        {
            Settings = settings;
            HttpClient = new HttpClient();
            JsonFormatter = new JsonMediaTypeFormatter();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public PandaDocHttpClientSettings Settings
        {
            get { return settings; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                settings = value;
            }
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

        public JsonMediaTypeFormatter JsonFormatter
        {
            get { return jsonFormatter; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                jsonFormatter = value;
            }
        }

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

        public async Task<PandaDocHttpResponse<PandaDocBearerToken>> Login(string username, string password)
        {
            if (username == null) throw new ArgumentNullException("username");
            if (password == null) throw new ArgumentNullException("password");

            var values = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", username},
                {"password", password},
                {"client_id", settings.ClientId},
                {"client_secret", settings.ClientSecret},
                {"scope", "read write read+write"}
            };

            var content = new FormUrlEncodedContent(values);

            HttpResponseMessage httpResponse = await httpClient.PostAsync(settings.AuthUri + "oauth2/access_token", content);

            PandaDocHttpResponse<PandaDocBearerToken> response = await httpResponse.ToPandaDocResponseAsync<PandaDocBearerToken>();

            return response;
        }

        public async Task<PandaDocHttpResponse<GetDocumentsResponse>> GetDocuments()
        {
            HttpResponseMessage httpResponse = await httpClient.GetAsync(settings.ApiUri + "public/v1/documents");

            PandaDocHttpResponse<GetDocumentsResponse> response = await httpResponse.ToPandaDocResponseAsync<GetDocumentsResponse>();

            return response;
        }

        public async Task<CreateDocumentResponse> CreateDocument(string filePath, CreateDocumentRequest document)
        {
            var client = new RestClient("https://api.pandadoc.com/public/v1/documents");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Host", "api.pandadoc.com");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "API-Key c6caae24740bb7bfffc0895f27bbf1ca7fe6bbe9");
            request.AddHeader("content-type", "multipart/form-data");

            byte[] fileContent = File.ReadAllBytes("D:\\panda.pdf");
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

        public async Task<PandaDocHttpResponse<GetDocumentResponse>> GetDocument(string uuid)
        {
            var httpResponse = await httpClient.GetAsync(settings.ApiUri + "public/v1/documents/" + uuid);
            PandaDocHttpResponse<GetDocumentResponse> response = await httpResponse.ToPandaDocResponseAsync<GetDocumentResponse>();

            return response;
        }

        public async Task<PandaDocHttpResponse<SendDocumentResponse>> SendDocument(string uuid, SendDocumentRequest request)
        {
            HttpContent httpContent = new ObjectContent<SendDocumentRequest>(request, JsonFormatter);

            HttpResponseMessage httpResponse = await httpClient.PostAsync(settings.ApiUri + "public/v1/documents/" + uuid + "/send", httpContent);

            PandaDocHttpResponse<SendDocumentResponse> response = await httpResponse.ToPandaDocResponseAsync<SendDocumentResponse>();

            return response;
        }

        public async Task<PandaDocHttpResponse<ShareDocumentResponse>> ShareDocument(string uuid, ShareDocumentRequest request)
        {
            HttpContent httpContent = new ObjectContent<ShareDocumentRequest>(request, JsonFormatter);

            HttpResponseMessage httpResponse = await httpClient.PostAsync(settings.ApiUri + "public/v1/documents/" + uuid + "/session", httpContent);

            PandaDocHttpResponse<ShareDocumentResponse> response = await httpResponse.ToPandaDocResponseAsync<ShareDocumentResponse>();

            return response;
        }

        public async Task<PandaDocHttpResponse> DeleteDocument(string uuid)
        {
            HttpResponseMessage httpResponse = await httpClient.DeleteAsync(settings.ApiUri + "public/v1/documents/" + uuid);

            PandaDocHttpResponse response = await httpResponse.ToPandaDocResponseAsync();

            return response;
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