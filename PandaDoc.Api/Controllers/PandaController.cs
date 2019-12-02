using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocument;
using PandaDoc.Models.SendDocument;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PandaDoc.Api.Controllers
{
    //[Authorize]
    public class PandaController : ApiController
    {
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocPdf_FieldTags.pdf";
        PandaDocHelper pandaDocHelper = new PandaDocHelper();


        [HttpGet]
        [Route("~/api/documents")]
        public async Task<Models.GetDocuments.GetDocumentsResponse> Documents()
        {
            var response = await pandaDocHelper.GetAllDocuments();
            return response.Value;
        }

        [HttpGet]
        [Route("~/api/document/detail")]
        public async Task<HttpResponseMessage> DocumentDetail(string documentId)
        {
            return await pandaDocHelper.GetDocumentDetail(documentId);
        }

        [HttpGet]
        [Route("~/api/document/upload")]
        public async Task<GetDocumentResponse> Upload(string fileName)
        {
            var sharedDocuments = new List<ShareDocumentResponse>();
            CreateDocumentRequest request = CreateDocumentRequest();
            byte[] fileContent = File.ReadAllBytes(fileName);
            var response = await pandaDocHelper.CreateDocument(fileContent, request);
            return response.Value;
        }

        [HttpGet]
        [Route("~/api/document/uploadbytes")]
        public async Task<GetDocumentResponse> Upload()
        {
            byte[] fileContent;
            GetDocumentResponse response = new GetDocumentResponse();
            if (HttpContext.Current.Request.Files.Count == 1)
            {
                using (var fs = HttpContext.Current.Request.Files[0].InputStream)
                {
                    BinaryReader br = new BinaryReader(fs);
                    fileContent = br.ReadBytes((int)fs.Length);
                }
                var pandaDocHelper = new PandaDocHelper();
                CreateDocumentRequest request = CreateDocumentRequest();
                var docresponse = await pandaDocHelper.CreateDocument(fileContent, request);
                response = docresponse.Value;
            }
            return response;
        }

        [HttpGet]
        [Route("~/api/document/share")]
        public async Task<ShareDocumentResponse> Share(string documentId, string email)
        {
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
