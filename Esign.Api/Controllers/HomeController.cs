using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PandaDoc;
using PandaDoc.Models.CreateDocument;
using PandaDoc.Models.GetDocument;
using PandaDoc.Models.SendDocument;
using PandaDoc.Standard;

namespace Esign.Api.Controllers
{
    public class HomeController : Controller
    {
        protected readonly string SampleDocUrl = "https://cdn2.hubspot.net/hubfs/2127247/public-templates/SamplePandaDocPdf_FieldTags.pdf";
        private AppData appData;
        public HomeController(IOptions<AppData> config)
        {
            appData = config.Value;
        }

        [Authorize]
        [HttpGet]
        [Route("identity")]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {

            return new JsonResult("test");
        }

        [HttpGet]
        [Route("documents")]
        public async Task<PandaDoc.Models.GetDocuments.GetDocumentsResponse> Documents()
        {
            var pandaDocHelper = new PandaDocHelper();
            var response = await pandaDocHelper.GetAllDocuments();
            return response;
        }

        [HttpPost]
        [Route("uploadfilepath")]
        public async Task<GetDocumentResponse> Upload(string fileName, CreateDocumentRequest request)
        {
            var sharedDocuments = new List<ShareDocumentResponse>();
            var pandaDocHelper = new PandaDocHelper();
            byte[] fileContent = System.IO.File.ReadAllBytes(fileName);
            var response = await pandaDocHelper.CreateDocument(fileContent, request);
            return response;
        }

        [HttpPost]
        [Route("uploadfile")]
        public async Task<GetDocumentResponse> Upload(IFormFile file, CreateDocumentRequest request)
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
                var docresponse = await pandaDocHelper.CreateDocument(fileContent, request);
                response = docresponse;
            }
            return response;
        }

        [HttpPost]
        [Route("uploadbytes")]
        public async Task<GetDocumentResponse> Upload(byte[] fileBytes, CreateDocumentRequest request)
        {
            GetDocumentResponse response = new GetDocumentResponse();
            if (fileBytes.Length > 0)
            {
                var pandaDocHelper = new PandaDocHelper();
                var docresponse = await pandaDocHelper.CreateDocument(fileBytes, request);
                response = docresponse;
            }
            return response;
        }

        [HttpPost]
        [Route("sharedocument")]
        public async Task<ShareDocumentResponse> Share(string documentId, string email)
        {
            var pandaDocHelper = new PandaDocHelper();
            var response = await pandaDocHelper.ShareDocument(documentId, email);
            return response;
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

            var bearerToken = new PandaDocBearerToken { ApiKey = appData.PandaDocApiKey };
            client.SetBearerToken(bearerToken);

            return client;
        }
    }
}

