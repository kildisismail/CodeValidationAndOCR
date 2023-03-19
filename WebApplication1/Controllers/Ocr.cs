using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static WebApplication1.Controllers.Ocr;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Ocr : ControllerBase
    {
        private readonly ILogger<Codes> _logger;

        public Ocr(ILogger<Codes> logger)
        {
            _logger = logger;
        }

        public class OCRText
        {
            public string Locale { get; set; }
            public string Description { get; set; }
            public BoundingPoly BoundingPoly { get; set; }
        }

        public class BoundingPoly
        {
            public List<Vertex> Vertices { get; set; }
        }

        public class Vertex
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        [HttpPost("uploadjson")]
        public IActionResult JsonParsed([FromBody] JsonElement body)
        {
            string? text = "";
            try
            {
                string jsonText = System.Text.Json.JsonSerializer.Serialize(body);
                var responses = JsonConvert.DeserializeObject<OCRText[]>(jsonText);
                Dictionary<int, string> ocrDatas = new Dictionary<int, string>();

                for (int i = 1; i < responses?.Length; i++)
                {
                    text = responses[i].Description;
                    var vertices = responses[i].BoundingPoly.Vertices;

                    vertices = vertices.OrderBy(v => v.X).ThenBy(v => v.Y).ToList();

                    ocrDatas.Add(i, text);
                }

                return Ok(ocrDatas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

}