using System.Threading.Tasks;
using System.Web.Mvc;

namespace PandaDoc.Api.Controllers
{
    public class HomeController: Controller
    {
        PandaDocHelper pandaDoc = new PandaDocHelper();

        [HttpGet]
        public ViewResult Index(string sessionId)
        {
            return View(sessionId);
        }

        [HttpGet]
        public async Task<ViewResult> List()
        {
            var docs = await pandaDoc.GetAllDocuments();
            return View(docs.Value.Results);
        }

        [HttpGet]
        public async Task<ViewResult> Details(string documentId)
        {
            var docs = await pandaDoc.GetDocument(documentId);
            return View(docs.Value);
        }


    }
}
