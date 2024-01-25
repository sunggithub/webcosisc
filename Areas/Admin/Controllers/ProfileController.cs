using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Website.Data;
using Website.Models;

namespace Website.Areas.Admin.Controllers
{
    public class ProfileController : Controller
    {
        private readonly DataContextDapper _dapper;

        public ProfileController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue("UserId");

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int itemId))
            {
                string sql = "SELECT * FROM Users WHERE UserId = " + itemId;
                var user = _dapper.LoadData<User>(sql);
                var firstUser = user.FirstOrDefault(); // hoặc SingleOrDefault hoặc First

                if (firstUser != null)
                {
                    bool isAdmin = User.IsInRole("admin");
                    if (isAdmin)
                    {
                        // Nếu là admin, hiển thị thông tin người dùng dưới dạng biểu mẫu chỉnh sửa
                        return View("Index", firstUser); // Giả sử bạn có một view cho việc chỉnh sửa thông tin người dùng
                    }
                }
            }

            // Nếu có vấn đề hoặc ID người dùng không khả dụng, bạn có thể chuyển hướng hoặc hiển thị thông báo lỗi.
            return RedirectToAction("Index");
        }
    }
}
