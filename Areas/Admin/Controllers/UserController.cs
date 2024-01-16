using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Models;

namespace Website.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly DataContextDapper _dapper;

        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index()
        {
            string sql = @"SELECT 
                            [UserId],
                            [UserName],
                            [UserFirstName],
                            [UserLastName],
                            [UserRole] FROM Users";
            return View(_dapper.LoadData<User>(sql));
        }
        public IActionResult EditUser() 
        {
            if (HttpContext.Request.Query.TryGetValue("editUser", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sql = "SELECT * FROM Users WHERE UserId = " + itemId;
                    var user = _dapper.LoadData<User>(sql);
                    var firstUser = user.FirstOrDefault(); // hoặc SingleOrDefault hoặc First

                    return View(firstUser);
                }
            }
            throw new Exception("Failed to dislay user to edit!");
        }

        [HttpPost]
        public IActionResult EditUser(User editUser, IFormFile fileImage)
        {
            if (fileImage != null && fileImage.Length > 0)
            {
                string filename = Path.GetFileName(fileImage.FileName);
                string uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                string filePath = Path.Combine(uploadFolderPath, filename);

                if (!Directory.Exists(uploadFolderPath))
                {
                    Directory.CreateDirectory(uploadFolderPath);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileImage.CopyTo(fileStream);
                }
                editUser.UserImage = filename;
                string sql = @"
                UPDATE Users 
                    SET UserRole = '" + editUser.UserRole +
                   @"' WHERE UserId = " + editUser.UserId.ToString();
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("Failed to edit User!");
            }
            else
            {
                string sql = @"
                UPDATE Users 
                    SET UserRole = '" + editUser.UserRole +
                   @"' WHERE UserId = " + editUser.UserId.ToString();
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("Failed to edit User!");
            }
        }
        public IActionResult DeleteUser(int delete)
        {
            string sql = "DELETE FROM Users WHERE UserId = " + delete;

            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("Index");
            }
            throw new Exception("Failed to delete User!");
        }
    }
}
