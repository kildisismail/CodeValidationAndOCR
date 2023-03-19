using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Codes : ControllerBase
    {
        string characters = "ACDEFGHKLMNPRTXYZ";
        string numbers = "234579";

        private readonly ILogger<Codes> _logger;

        public Codes(ILogger<Codes> logger)
        {
            _logger = logger;
        }

        [HttpGet("generated")]
        public async Task<IActionResult> GenerateCode()
        {
            Random random = new Random();
            //code1 ve code2 olarak ayr� ayr� 2 kod �retilir.
            //const olarak tan�mlanan characters keyine g�re random 3 karakter �retilir.
            string code1 = new string(Enumerable.Repeat(characters, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            string code2 = new string(Enumerable.Repeat(characters, 3).Select(s => s[random.Next(s.Length)]).ToArray());

            //const olarak tan�mlanan numbers keyine g�re random 2 karakter �retilir.
            string numbers1 = new string(Enumerable.Repeat(numbers, 2).Select(s => s[random.Next(s.Length)]).ToArray());

            //�retilen code1 ve code2 nin sonuna number1 eklenip birle�tirilerek 8 haneli bir kod �retilir.
            string concatCode = code1 + numbers1.Substring(0, 1) + code2 + numbers1.Substring(1, 1);

            //�retilen 8 haneli kod bu yard�mc� method ile kodun karakterlerinin ascii de�erleri hesaplarak listeye eklenir.
            List<int> codes = AsciiCharControl(concatCode);

            //8 haneli kodun ascii de�erlerinin 2 - 4 - 6 ve 1 - 3 - 7 haneleri toplanarak mod2 al�n�r. E�it ise kod �retilir.
            //De�il ise GenerateCode methodu tekrar �al��arak recursive �ekilde valid kod generate edilmi� olur. 
            bool isValid = CalculateCodes(codes);

            if (isValid)
            {
                return Ok(concatCode);
            }
            else
            {
                return await GenerateCode();
            }
        }

        [HttpPost("verified")]
        public IActionResult VerifyCode(string code)
        {
            //bo� kod girildi�inde do�rudan code parameters is not null!! d�n�l�r
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("code parameters is not null!!");
            }

            try
            {
                //kullan�c�n�n girdi�i 8 haneli kod bu yard�mc� method ile kodun karakterlerinin ascii de�erleri hesaplarak listeye eklenir.
                List<int> codes = AsciiCharControl(code);

                //kullan�c�n�n girdi�i 8 haneli kodun ascii de�erlerinin 2 - 4 - 6 ve 1 - 3 - 7 haneleri toplanarak mod2 al�n�r. E�it ise true, de�il ise false d�ner.
                bool isValid = CalculateCodes(codes);

                if (isValid)
                {
                    return Ok(true);
                }
                return NotFound(false);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private static List<int> AsciiCharControl(string code)
        {
            List<int> codes = new List<int>();
            int codesAsciiValues;
            // kodun karakterlerinin ascii de�erleri hesaplarak listeye eklenir.
            for (int i = 0; code.Length > i; i++)
            {
                codesAsciiValues = Convert.ToInt32(Convert.ToChar(code.Substring(i, 1)));
                codes.Add(codesAsciiValues);
            }

            return codes;
        }

        private bool CalculateCodes(List<int> numbers)
        {
            //8 haneli kodun ascii de�erlerinin 2 - 4 - 6 ve 1 - 3 - 7 haneleri toplanarak mod2 al�n�r. E�it ise true, de�il ise false d�ner.
            int evenNumbers = numbers[2] + numbers[4] + numbers[6];
            int oddNumbers = numbers[1] + numbers[3] + numbers[7];
            if (evenNumbers % 2 == oddNumbers % 2)
            {
                return true;
            }
            return false;
        }
    }

}