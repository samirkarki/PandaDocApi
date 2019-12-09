using Newtonsoft.Json;
using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocument;
using PandaDoc.Models.SendDocument;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PandaDoc.Standard
{
    public class PandaDocHelper
    {
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocPdf_FieldTags.pdf";
        public async Task<GetDocumentResponse> CreateDocument(byte[] fileContent, CreateDocumentRequest request)
        {
            var sharedDocuments = new List<ShareDocumentResponse>();
            using (var client = SetApiKey())
            {
                if (request == null)
                    request = CreateDocumentRequest();

                var response = await client.CreateDocument(fileContent, request);
                if (!string.IsNullOrEmpty(response.Uuid))
                {
                    var sendRequest = new SendDocumentRequest
                    {
                        Message = "Please sign this document",
                        Silent = request.DisableEmail
                    };
                    var sendDocResponse = await client.SendDocument(response.Uuid, sendRequest);

                    var getDocResponse = await client.GetDocument(response.Uuid);
                    return getDocResponse;
                }
                else
                    return null;
            }
        }

        public async Task<dynamic> CreateDocument(byte[] fileContent, string documentJson)
        {
            var document = JsonConvert.DeserializeObject<CreateDocumentRequest>(documentJson);
            using (var client = SetApiKey())
            {
                var response = await client.CreateDocument(fileContent, documentJson);
                if (!string.IsNullOrEmpty(response.Uuid))
                {
                    var sendRequest = new SendDocumentRequest
                    {
                        Message = "Please sign this document",
                        Silent = document.DisableEmail
                    };
                    var sendDocResponse = await client.SendDocument(response.Uuid, sendRequest);

                    var detailResponse = client.GetDocumentDetail(response.Uuid);
                    return detailResponse;
                }
                else
                    return null;
            }
        }

        public async Task<ShareDocumentResponse> ShareDocument(string documentId, string shareDocumentRequest)
        {
            var shareRequest = JsonConvert.DeserializeObject<ShareDocumentRequest>(shareDocumentRequest);
            shareRequest.LifeTime = ESignConfig.DocumentLifetime;
            using (var client = SetApiKey())
            {
                return await client.ShareDocument(documentId, shareRequest);
            }
        }

        public async Task<PandaDoc.Models.GetDocuments.GetDocumentsResponse> GetAllDocuments()
        {
            using (var client = SetApiKey())
            {
                var response = await client.GetDocuments();
                return response;
            }
        }

        public async Task<GetDocumentResponse> GetDocument(string documentId)
        {
            using (var client = SetApiKey())
            {
                var response = await client.GetDocument(documentId);
                return response;
            }
        }

        public dynamic GetDocumentDetail(string documentId)
        {
            using (var client = SetApiKey())
            {
                var response = client.GetDocumentDetail(documentId);
                return response;
            }
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

        protected PandaDocHttpClient SetApiKey()
        {
            var client = new PandaDocHttpClient();

            var bearerToken = new PandaDocBearerToken { ApiKey = "c6caae24740bb7bfffc0895f27bbf1ca7fe6bbe9" };
            client.SetBearerToken(bearerToken);

            return client;
        }
    }
}
