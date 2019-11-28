using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PandaDoc;
using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocument;
using PandaDoc.Models.SendDocument;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Esign.Api.Controllers
{
    [Authorize]
    public class PandaController : Controller
    {
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocPdf_FieldTags.pdf";

        [HttpGet]
        [Route("~/api/documents")]
        public async Task<PandaDoc.Models.GetDocuments.GetDocumentsResponse> Documents()
        {
            var pandaDocHelper = new PandaDocHelper();
            var response = await pandaDocHelper.GetAllDocuments();
            return response.Value;
        }

        [HttpPost]
        [Route("~/api/document/uploadfilepath")]
        public async Task<GetDocumentResponse> Upload(string fileName)
        {
            var sharedDocuments = new List<ShareDocumentResponse>();
            var pandaDocHelper = new PandaDocHelper();
            CreateDocumentRequest request = CreateDocumentRequest();
            byte[] fileContent = System.IO.File.ReadAllBytes(fileName);
            var response = await pandaDocHelper.CreateDocument(fileContent, request);
            return response.Value;
        }

        [HttpPost]
        [Route("~/api/document/uploadfile")]
        public async Task<GetDocumentResponse> Upload(IFormFile file)
        {
            byte[] fileContent;
            GetDocumentResponse response = new GetDocumentResponse();
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileContent = ms.ToArray();
                }
                var pandaDocHelper = new PandaDocHelper();
                CreateDocumentRequest request = CreateDocumentRequest();
                var docresponse = await pandaDocHelper.CreateDocument(fileContent, request);
                response = docresponse.Value;
            }
            return response;
        }

        [HttpPost]
        [Route("~/api/document/uploadbytes")]
        public async Task<GetDocumentResponse> Upload(byte[] fileContent)
        {
            GetDocumentResponse response = new GetDocumentResponse();
            if (fileContent.Length > 0)
            {
                var pandaDocHelper = new PandaDocHelper();
                CreateDocumentRequest request = CreateDocumentRequest();
                var docresponse = await pandaDocHelper.CreateDocument(fileContent, request);
                response = docresponse.Value;
            }
            return response;
        }

        [HttpPost]
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
