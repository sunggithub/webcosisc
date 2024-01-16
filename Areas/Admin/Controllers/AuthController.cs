using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using Website.Data;
using Website.Dtos;
using Website.Helpers;
using Website.Models;


namespace Website.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AuthController : Controller
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.UserPasswordConfirm == userForRegistration.UserPassword)
            {
                string sqlCheckUserExists = "SELECT Email FROM Auth WHERE Email = '" +
                    userForRegistration.UserName + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.UserPassword, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO Auth([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('" + userForRegistration.UserName +
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
                                [UserRole]
                            ) VALUES (" +
                                "'" + userForRegistration.UserName +
                                "', '" + userForRegistration.UserFirstName +
                                "', '" + userForRegistration.UserLastName +
                                "', 'user')";
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to register user.");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Passwords do not match!");

        }


        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT 
                [PasswordHash],
                [PasswordSalt] FROM Auth WHERE Email = '" +
                userForLogin.Email + "'";

            UserForLoginConfirmationDto userForConfirmation = _dapper
                .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
            if (userForConfirmation == null)
            {
                return StatusCode(401, "User not found!");
            }

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.UserPassword, userForConfirmation.PasswordSalt);

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Password Incorrect");
                }
                else
                {
                    string sql = @"SELECT [UserRole] FROM Users WHERE UserName = '" + userForLogin.Email + "'";
                    string role = _dapper.LoadDataSingle<string>(sql);
                    string selectsql = @"SELECT 
                                        [UserId],
                                        [UserName],
                                        [UserFirstName],
                                        [UserLastName],
                                        [UserPassword],
                                        [UserRole] FROM Users WHERE UserName = '" + userForLogin.Email + "'";
                    var userData = _dapper.LoadDataSingle<User>(selectsql);
                    if (!string.IsNullOrEmpty(role) && role.Trim().ToLower() == "admin" && userData != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userData.UserName),
                            new Claim("UserFirstName", userData.UserFirstName),
                            new Claim("UserLastName", userData.UserLastName),
                            new Claim(ClaimTypes.Role, userData.UserRole),
                            // Thêm các thông tin khác vào claim nếu cần
                        };

                        // Tạo identity của người dùng
                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);


                        // Tạo principal từ identity
                        var authProperties = new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20), // Thời gian hết hạn phiên (20 phút)
                            IsPersistent = false,
                            AllowRefresh = true
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        // Chuyển hướng sau khi đăng nhập thành công
                        return RedirectToAction("Index", "Admin", new { area = "" });
                    }
                    else if (!string.IsNullOrEmpty(role) && role.Trim().ToLower() == "user" && userData != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userData.UserName),
                            new Claim("UserFirstName", userData.UserFirstName),
                            new Claim("UserLastName", userData.UserLastName),
                            new Claim(ClaimTypes.Role, userData.UserRole),
                            // Thêm các thông tin khác vào claim nếu cần
                        };

                        // Tạo identity của người dùng
                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);


                        // Tạo principal từ identity
                        var authProperties = new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20), // Thời gian hết hạn phiên (20 phút)
                            IsPersistent = false,
                            AllowRefresh = true
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);
                        return Redirect("/Home/Index");
                    }
                }
            }

            string userIdSql = @"
                SELECT UserId FROM Users WHERE UserName = '" +
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return View(userId);
        }

        [AllowAnonymous]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM Users WHERE UserId = '" +
                User.FindFirst("UserId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }

    }
}