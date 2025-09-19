using EmailSender.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailSender.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpGet("sendmail")]
        public IActionResult SendMail()
        {
            return Ok();
        }
        [HttpPost("sendemail")]
        [Consumes("application/json")]
        public async Task<IActionResult> SendMail([FromBody]EmailFormModel emailFormModel )
        {
           

            try
            {
                await _emailService.SendEmailAsync(emailFormModel.email, emailFormModel.subject, emailFormModel.message);
                //await _emailService.SendEmailByMailKitAsync(emailFormModel.email, emailFormModel.subject, emailFormModel.message);
                await _emailService.SendReEmailAsync(emailFormModel.email);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email sending failed: {ex.Message}");
            }
        }
    }
}
