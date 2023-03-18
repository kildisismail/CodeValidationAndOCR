using Microsoft.AspNetCore.Mvc;
using System.Text;

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

        [HttpGet("codes")]
        public static string GenerateGuid()
        {
            const string chars = "ACDEFGHKLMNPRTXYZ234579";
            var guid = Guid.NewGuid().ToString("N").Substring(0, 8);

            var result = new StringBuilder();
            for (var i = 0; i < guid.Length; i++)
            {
                var c = guid[i];
                if (char.IsLetterOrDigit(c) && chars.Contains(c))
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

    }
}