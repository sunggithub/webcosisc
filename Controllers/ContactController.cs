using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Models;

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
                TempData["SendSuccessMessage"] = "Chúng tôi đã ghi nhận thông tin của bạn. Chúng tôi sẽ trả lời trong 24h làm việc.";
                return RedirectToAction("Index", "Contact", new {area = "" });
            }
            return View();
        }
    }
}
