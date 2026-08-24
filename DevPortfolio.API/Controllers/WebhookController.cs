using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DevPortfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        [HttpPost("resend")]
        public async Task<IActionResult> ResendWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            Console.WriteLine("RESEND WEBHOOK RECEIVED:");
            Console.WriteLine(body);

            return Ok();
        }
    }
}