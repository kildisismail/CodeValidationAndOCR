using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

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
            public string? Locale { get; set; }
            public string? Description { get; set; }
            public BoundingPoly? BoundingPoly { get; set; }
        }

        public class BoundingPoly
        {
            public List<Vertex>? Vertices { get; set; }
        }

        public class Vertex
        {
            public int? X { get; set; }
            public int? Y { get; set; }
        }

        public class RawList
        {
            public int? CurrentY { get; set; }
            public int? CurrentX { get; set; }
            public int? RowNumber { get; set; }
            public string? Text { get; set; }
        }


        [HttpPost("uploadjson")]
        public IActionResult VerifyCode([FromBody] JsonElement body)
        {
            //maksimum Y düzlem deðeri ve row aralýk deðeri
            const int maxY = 550;
            const int maxRowGap = 15;
            //
            List<RawList> rawList = new();
            List<RawList> normalizedList = new();
            try
            {
                //body deðiþkenini json serialize ederek OCRText modelimize uygun hale getiriliyor
                string jsonText = System.Text.Json.JsonSerializer.Serialize(body);
                var responses = JsonConvert.DeserializeObject<OCRText[]>(jsonText);

                //rawList ile Json datadaki textler ile koordinatlarý ayýrýp düzenleniyor
                for (int i = 1; i < responses?.Length; i++)
                {
                    rawList.Add(new RawList
                    {
                        CurrentY = responses?[i].BoundingPoly?.Vertices?[0].Y,
                        CurrentX = responses?[i].BoundingPoly?.Vertices?[0].X,
                        RowNumber = i,
                        Text = responses?[i].Description
                    });

                }
                //normalizedList e maxRowGap deðiþkenindeki Y aralýðýna göre ayný düzlemde olduðu belirleniyor. Text ve RowNumbers düzenlemesi yapýlýyor
                int rowCounter = 1;
                for (int i = 0; i < rawList.Count; i++)
                {
                    if (rawList.Count() - 1 == i)
                        break;
                    if (rawList[i].CurrentY >= 0 &&
                        rawList[i].CurrentY <= maxY &&
                        rawList[i + 1].CurrentX - rawList[i].CurrentX < maxRowGap)
                    {
                        normalizedList.Add(new RawList { Text = rawList[i].Text, RowNumber = rowCounter });

                    }
                    if (rawList[i + 1].CurrentY - rawList[i].CurrentY > maxRowGap && rawList.Count > i)
                    {
                        rowCounter++;
                    }
                    else
                    {
                        normalizedList.Add(new RawList { Text = rawList[i].Text, RowNumber = rowCounter });
                    }

                }
                //rownumber a göre gruplanýp description birleþtiriliyor
                var datax = normalizedList.GroupBy(l => new { l.RowNumber })
                                          .Select(g => new { v = string.Join(",", g.Select(i => i.Text)) })
                                          .ToList();

                string finalText = "";
                //Description yanýna satýr sýra numarasý eklenerek düzenleniyor
                for (int i = 0; i < datax.Count; i++)
                {
                    finalText += $"{i + 1} : {datax[i].v} \n";
                }

                return Ok(finalText);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

}