using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using Website.Data;
using Website.Helpers;
using Website.Models;

namespace Website.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class UserController : Controller
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
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

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User createUser, IFormFile fileImage)
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
                createUser.UserImage = filename;

                string sqlCheckUserExists = "SELECT Email FROM Auth WHERE Email = '" +
                    createUser.UserName + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(createUser.UserPassword, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO Auth([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('" + createUser.UserName +
                        @"', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {

                        string sqlAddUser = @"
                            INSERT INTO Users(
                                [UserName], 
                                [UserFirstName],
                                [UserLastName],
                                [UserRole],
                                [UserImage]) VALUES ("
                                + "N'" + createUser.UserName
                                + "',N'" + createUser.UserFirstName
                                + "',N'" + createUser.UserLastName
                                + "',N'" + createUser.UserRole
                                + "',N'" + createUser.UserImage
                                + "')";
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return RedirectToAction("Index");
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to CREATE user.");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Failed to Upload File!");
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
                    "', UserImage = N'" + editUser.UserImage +
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
        public IActionResult DeleteUser(int delete, string deleteUser)
        {
            string sql = "DELETE FROM Users WHERE UserId = @UserId";
            string sqlAuth = "DELETE FROM Auth WHERE Email = @Email";

            if (_dapper.ExecuteSql(sql, new { UserId = delete }) && _dapper.ExecuteSql(sqlAuth, new { Email = deleteUser }))
            {
                return RedirectToAction("Index");
            }

            throw new Exception("Failed to delete User!");
        }
    }
}
