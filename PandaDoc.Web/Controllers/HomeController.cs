using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.SendDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PandaDoc.Web.Controllers
{
    public class HomeController : ApiController
    {
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocPdf_FieldTags.pdf";
        [HttpGet]
        [Route("~/api/documents")]
        public async Task<IList<ShareDocumentResponse>> Documents(string DocumentId)
        {
            var sharedDocuments = new List<ShareDocumentResponse>();
            using (var client = SetApiKey())
            {
                CreateDocumentRequest request = CreateDocumentRequest();

                var response = await client.CreateDocument(request);
                var sendRequest = new SendDocumentRequest
                {
                    Message = "Please sign this document"
                };
                var sendDocResponse = await client.SendDocument(response.Value?.Uuid ?? DocumentId, sendRequest);
                foreach (var recipient in request.Recipients)
                {
                    var shareRequest = new ShareDocumentRequest
                    {
                        Recipient = recipient.Email,
                        LifeTime = 90000
                    };
                    var shareDocResponse = await client.ShareDocument(response.Value.Uuid, shareRequest);
                    shareDocResponse.Value.Recipient = recipient.Email;
                    sharedDocuments.Add(shareDocResponse.Value);
                }
            }
            return sharedDocuments;
        }

        private CreateDocumentRequest CreateDocumentRequest()
        {
            return new CreateDocumentRequest
            {
                Name = "Sample Document",
                Url = SampleDocUrl,
                File = "C:\\Users\\i81211\\Desktop\\sample.pdf",
                Recipients = new[]
                {
                    new Recipient
                    {
                        Email = "samir.ctec@gmail.com",
                        FirstName = "Samir",
                        LastName = "Ctec",
                        Role = "u1",
                    },
                    new Recipient
                    {
                        Email = "samir@teksewa.com",
                        FirstName = "Samir",
                        LastName = "Teksewa",
                        Role = "u1",
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
            var settings = new PandaDocHttpClientSettings();
            var client = new PandaDocHttpClient(settings);

            var bearerToken = new PandaDocBearerToken { ApiKey = "c6caae24740bb7bfffc0895f27bbf1ca7fe6bbe9" };
            client.SetBearerToken(bearerToken);

            return client;
        }
    }
}
