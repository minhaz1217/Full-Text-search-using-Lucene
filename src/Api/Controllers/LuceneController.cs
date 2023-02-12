using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace Full_Text_search_using_Lucene.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuceneController : ControllerBase
    {
        private readonly LuceneService _luceneService;
        public LuceneController()
        {
            _luceneService = new LuceneService();
        }

        [HttpGet]
        public IActionResult Get()
        {
            _luceneService.CreateIndex();
            return Ok("CALLED");
        }
    }
}
