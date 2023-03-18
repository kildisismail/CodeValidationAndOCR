using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Codes : ControllerBase
    {

        private readonly ILogger<Codes> _logger;

        public Codes(ILogger<Codes> logger)
        {
            _logger = logger;
        }

        [HttpPost("verified")]
        public IActionResult VerifyCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("code parameters is not null!!");
            }

            try
            {
                int asciiSum = code.Sum(c => (int)c);

                if (asciiSum != 500)
                {
                    return NotFound(false);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("generated")]
        public async Task<IActionResult> GetKodlar()
        {
            string characters = "ACDEFGHKLMNPRTXYZ234579";
            Random random = new Random();

            string code1 = new string(Enumerable.Repeat(characters, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string code2 = new string(Enumerable.Repeat(characters, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string concatCode = code1 + code2;
            int totalAsciiValue = concatCode.Sum(c => (int)c);

            if (totalAsciiValue == 500)
            {
                return Ok(concatCode);
            }
            else
            {
                return await GetKodlar();
            }
        }

    }

}