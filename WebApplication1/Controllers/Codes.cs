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
            //code1 ve code2 olarak ayrý ayrý 2 kod üretilir.
            //const olarak tanýmlanan characters keyine göre random 3 karakter üretilir.
            string code1 = new string(Enumerable.Repeat(characters, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            string code2 = new string(Enumerable.Repeat(characters, 3).Select(s => s[random.Next(s.Length)]).ToArray());

            //const olarak tanýmlanan numbers keyine göre random 2 karakter üretilir.
            string numbers1 = new string(Enumerable.Repeat(numbers, 2).Select(s => s[random.Next(s.Length)]).ToArray());

            //üretilen code1 ve code2 nin sonuna number1 eklenip birleþtirilerek 8 haneli bir kod üretilir.
            string concatCode = code1 + numbers1.Substring(0, 1) + code2 + numbers1.Substring(1, 1);

            //üretilen 8 haneli kod bu yardýmcý method ile kodun karakterlerinin ascii deðerleri hesaplarak listeye eklenir.
            List<int> codes = AsciiCharControl(concatCode);

            //8 haneli kodun ascii deðerlerinin 2 - 4 - 6 ve 1 - 3 - 7 haneleri toplanarak mod2 alýnýr. Eþit ise kod üretilir.
            //Deðil ise GenerateCode methodu tekrar çalýþarak recursive þekilde valid kod generate edilmiþ olur. 
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
            //boþ kod girildiðinde doðrudan code parameters is not null!! dönülür
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("code parameters is not null!!");
            }

            try
            {
                //kullanýcýnýn girdiði 8 haneli kod bu yardýmcý method ile kodun karakterlerinin ascii deðerleri hesaplarak listeye eklenir.
                List<int> codes = AsciiCharControl(code);

                //kullanýcýnýn girdiði 8 haneli kodun ascii deðerlerinin 2 - 4 - 6 ve 1 - 3 - 7 haneleri toplanarak mod2 alýnýr. Eþit ise true, deðil ise false döner.
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
            // kodun karakterlerinin ascii deðerleri hesaplarak listeye eklenir.
            for (int i = 0; code.Length > i; i++)
            {
                codesAsciiValues = Convert.ToInt32(Convert.ToChar(code.Substring(i, 1)));
                codes.Add(codesAsciiValues);
            }

            return codes;
        }

        private bool CalculateCodes(List<int> numbers)
        {
            //8 haneli kodun ascii deðerlerinin 2 - 4 - 6 ve 1 - 3 - 7 haneleri toplanarak mod2 alýnýr. Eþit ise true, deðil ise false döner.
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