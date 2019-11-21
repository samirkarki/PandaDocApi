using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocument;
using PandaDoc.Models.SendDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PandaDoc.Api.Controllers
{
    public class PandaController : ApiController
    {
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocPdf_FieldTags.pdf";

        [HttpGet]
        [Route("~/api/documents")]
        public async Task<Models.GetDocuments.GetDocumentsResponse> Documents()
        {
            var pandaDocHelper = new PandaDocHelper();
            var response = await pandaDocHelper.GetAllDocuments();
            return response.Value;
        }

        [HttpGet]
        [Route("~/api/document/upload")]
        public async Task<GetDocumentResponse> Upload()
        {
            var sharedDocuments = new List<ShareDocumentResponse>();
            var pandaDocHelper = new PandaDocHelper();
            CreateDocumentRequest request = CreateDocumentRequest();
            var filePath = "D:\\panda.pdf";
            var response = await pandaDocHelper.CreateDocument(filePath, request);
            return response.Value;
        }

        [HttpGet]
        [Route("~/api/document/share")]
        public async Task<ShareDocumentResponse> Share(string documentId, string email)
        {
            var pandaDocHelper = new PandaDocHelper();
            var response = await pandaDocHelper.ShareDocument(documentId, email);
            return response.Value;
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
            var settings = new PandaDocHttpClientSettings();
            var client = new PandaDocHttpClient(settings);

            var bearerToken = new PandaDocBearerToken { ApiKey = "c6caae24740bb7bfffc0895f27bbf1ca7fe6bbe9" };
            client.SetBearerToken(bearerToken);

            return client;
        }
        
    }
}
