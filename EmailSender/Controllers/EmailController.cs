using EmailSender.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> SendMail()
        {
            var ip =
        HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0]
        ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrWhiteSpace(ip) || ip == "::1")
            {
                return Ok(new
                {
                    Message = "IP address not available (local or internal request)",
                    IpAddress = ip
                });
            }

            // 2️⃣ Call IP Location API
            using var httpClient = new HttpClient();
            var apiUrl = $"https://ip-api.com/json/{ip}";

            var response = await httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Failed to fetch location details");
            }

            var json = await response.Content.ReadAsStringAsync();
            var location =JsonConvert.DeserializeObject<Data>(json);

            // 3️⃣ Return IP + Location details
            var emailBody = $@"
New Email Request Received

User Email: bhagwat
Subject: prasad

--- Location Details ---
IP Address : {location.Query}
City       : {location.City}
State      : {location.RegionName}
Country    : {location.Country}
ISP        : {location.Isp}
Timezone   : {location.Timezone}

--- Message ---
test message location
";
            try
            {
                await _emailService.SendEmailAsync("bhagwat@xelentor.com", "Test", emailBody);
                //await _emailService.SendEmailByMailKitAsync(emailFormModel.email, emailFormModel.subject, emailFormModel.message);
                //await _emailService.SendReEmailAsync(emailFormModel.email);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email sending failed: {ex.Message}");
            }
            return Ok(new
            {
                IpAddress = location?.Query,
                Country = location.Country,
                State = location.RegionName,
                City = location.City,
                Zip = location.Zip,
                ISP = location.Isp,
                Latitude = location.Lat,
                Longitude = location.Lon,
                TimeZone = location.Timezone
            });

            //return Ok();
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
