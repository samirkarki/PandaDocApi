using PandaDocSigner.Models.CreateDocument;
using PandaDocSigner.Models.GetDocument;
using PandaDocSigner.Models.SendDocument;

namespace PandaDocSignerSigner.Service
{
    public class PandaDocSignerHelper
    {
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocSignerPdf_FieldTags.pdf";
        public async Task<PandaDocSignerHttpResponse<GetDocumentResponse>> CreateDocument(string filePath, CreateDocumentRequest request)
        {
            var sharedDocuments = new List<ShareDocumentResponse>();
            using (var client = SetApiKey())
            {
                if (request == null)
                    request = CreateDocumentRequest();

                var response = await client.CreateDocument(filePath, request);
                if (!string.IsNullOrEmpty(response.Uuid))
                {
                    var sendRequest = new SendDocumentRequest
                    {
                        Message = "Please sign this document"
                    };
                    var sendDocResponse = await client.SendDocument(response.Uuid, sendRequest);

                    var getDocResponse = await client.GetDocument(response.Uuid);
                    return getDocResponse;
                }
                else
                    return null;
                //foreach (var recipient in request.Recipients)
                //{
                //    var shareRequest = new ShareDocumentRequest
                //    {
                //        Recipient = recipient.Email,
                //        LifeTime = 90000
                //    };
                //    var shareDocResponse = await client.ShareDocument(response.Uuid, shareRequest);
                //    shareDocResponse.Value.Recipient = recipient.Email;
                //    sharedDocuments.Add(shareDocResponse.Value);
                //}
            }
            // return sharedDocuments;
        }

        public async Task<PandaDocSignerHttpResponse<ShareDocumentResponse>> ShareDocument(string documentId, string recipientEmail)
        {
            var shareRequest = new ShareDocumentRequest
            {
                Recipient = recipientEmail,
                LifeTime = 90000
            };
            using (var client = SetApiKey())
            {
                return await client.ShareDocument(documentId, shareRequest);
            }
        }

        public async Task<PandaDocSignerHttpResponse<PandaDocSigner.Models.GetDocuments.GetDocumentsResponse>> GetAllDocuments()
        {
            using (var client = SetApiKey())
            {
                var response = await client.GetDocuments();
                return response;
            }
        }

        public async Task<PandaDocSignerHttpResponse<PandaDocSigner.Models.GetDocument.GetDocumentResponse>> GetDocument(string documentId)
        {
            using (var client = SetApiKey())
            {
                var response = await client.GetDocument(documentId);
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

        protected PandaDocSignerHttpClient SetApiKey()
        {
            var settings = new PandaDocSignerHttpClientSettings();
            var client = new PandaDocSignerHttpClient(settings);

            var bearerToken = new PandaDocSignerBearerToken { ApiKey = "c6caae24740bb7bfffc0895f27bbf1ca7fe6bbe9" };
            client.SetBearerToken(bearerToken);

            return client;
        }
    }
}
