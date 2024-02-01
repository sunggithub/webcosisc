using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Mail;
using Website.Data;
using Website.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net;
using Google.Apis.Services;

namespace Website.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContextDapper _dapper;

        public ContactController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult SendContact(Contact saveContact)
        {
            string sql = @"INSERT INTO Contact (
                            [UserNameContact],
                            [UserEmailContact],
                            [ContentContact],
                            [ContactDate]
                            ) VALUES (
                                N'" + saveContact.UserNameContact 
                                + @"' ,N'" + saveContact.UserEmailContact 
                                + @"',N'" + saveContact.ContentContact
                                + @"', GETDATE())";
            if (_dapper.ExecuteSql(sql))
            {
                SendEmailWithOAuthAsync(saveContact.UserEmailContact, "Chủ đề Email", "Nội dung Email");
                TempData["SendSuccessMessage"] = "Chúng tôi đã ghi nhận thông tin của bạn. Chúng tôi sẽ trả lời trong 24h làm việc.";
                return RedirectToAction("Index", "Contact", new {area = "" });
            }
            return View();
        }

        private async Task SendEmailWithOAuthAsync(string recipientEmail, string subject, string body)
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "547910764483-iiolih1j56esi15eug1u43rtjk4pcm5e.apps.googleusercontent.com",
                    ClientSecret = "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"
                },
                new[] { GmailService.Scope.GmailSend },
                "user",
                CancellationToken.None
            );

            // Tạo một đối tượng GmailService để gửi email
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "YourAppName"
            });

            // Tạo thư từ
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Anh/Chị", "hathienngan190301@gmail.com"));
            mailMessage.To.Add(new MailboxAddress("Recipient Name", recipientEmail));
            mailMessage.Subject = subject;

            var builder = new BodyBuilder();
            builder.TextBody = body;

            mailMessage.Body = builder.ToMessageBody();

            // Chuyển đổi MimeMessage sang chuỗi base64url để gửi qua Gmail API
            var message = new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = Base64UrlEncode(mailMessage.ToString())
            };

            // Gửi email qua Gmail API
            service.Users.Messages.Send(message, "me").Execute();
        }

        // Hàm hỗ trợ chuyển đổi chuỗi sang base64url
        private string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

    }
}
