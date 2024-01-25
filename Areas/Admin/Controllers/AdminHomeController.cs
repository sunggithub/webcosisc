using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Models;
using Microsoft.AspNetCore.Authorization;
using Website.Helpers;
using System.Xml.Linq;

namespace Website.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class HomeAdminController : Controller
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public HomeAdminController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [HttpGet]
        public IActionResult Index()
        {
            string sql = @"SELECT [NewsId],
                        [NewsCategoryId],
                        [NewsTitle],
                        [NewsAuthor],
                        [NewsImage],
                        [NewsDate],
                        [NewsContent],
                        [NewsStatus],
                        [NewsView] 
                    FROM News ORDER BY NewsDate DESC";
            var news = _dapper.LoadData<News>(sql);

            string sqlcate = @"SELECT [NewsCategoryId],
                        [NewsCategoryTitle] FROM NewsCategory";
            var newsCategory = _dapper.LoadData<NewsCategory>(sqlcate);

            string sqlcontact = @"SELECT [ContactId],
                                [UserNameContact],
                                [UserEmailContact],
                                [ContentContact],
                                [ContactDate] FROM Contact ORDER BY ContactDate DESC";
            var contact = _dapper.LoadData<Contact>(sqlcontact);

            ViewBag.Contact = contact;
            ViewBag.News = news;
            ViewBag.NewsCategory = newsCategory;

            return View();
            
        }

        public IActionResult CreateNews()
        {
            string sqlcate = @"SELECT [NewsCategoryId],
                            [NewsCategoryTitle] FROM NewsCategory";
            var newsCategory = _dapper.LoadData<NewsCategory>(sqlcate);
            ViewBag.NewsCategory = newsCategory;
            return View();
        }
        [HttpPost]
        public IActionResult CreateNews(News createNews)
        {
            string sql = @"
            INSERT INTO News(
                [NewsCategoryId],
                [NewsTitle],
                [NewsAuthor],
                [NewsImage],
                [NewsDate],
                [NewsContent],
                [NewsStatus],
                [NewsView]) VALUES ("
                + "N'" + createNews.NewsCategoryId
                + "',N'" + createNews.NewsTitle
                + "',N'" + createNews.NewsAuthor
                + "',N'" + createNews.NewsImage
                + "', GETDATE() " 
                + ",N'" + createNews.NewsContent
                + "',N'" + createNews.NewsStatus
                + "',N'" + createNews.NewsView +
                "'" +")";
            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("Index");
            }
            throw new Exception("Failed to create news!");
        }

        public IActionResult EditNews()
        {
            // Logic lấy ID và đối tượng từ query string
            if (HttpContext.Request.Query.TryGetValue("editNews", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sql = "SELECT * FROM News WHERE NewsId = " + itemId;
                    var news = _dapper.LoadData<News>(sql);

                    string sqlcate = @"SELECT [NewsCategoryId],
                            [NewsCategoryTitle] FROM NewsCategory";
                    var newsCategory = _dapper.LoadData<NewsCategory>(sqlcate);

                    ViewBag.NewsCategory = newsCategory;

                    var firstNews = news.FirstOrDefault(); // hoặc SingleOrDefault hoặc First

                    return View(firstNews); // Trả về chỉ một đối tượng News cho View để chỉnh sửa// Trả về View để chỉnh sửa
                }
            }
            throw new Exception("Failed to dislay news to edit!");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult EditNews(News editNews, IFormFile fileImage)
        {
            if(fileImage != null && fileImage.Length > 0) 
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
                editNews.NewsImage = filename;
                string sql = @"
                UPDATE News 
                    SET NewsCategoryId = '" + editNews.NewsCategoryId +
                   "', NewsTitle = N'" + editNews.NewsTitle +
                   "', NewsAuthor = N'" + editNews.NewsAuthor +
                   "', NewsImage = N'" + editNews.NewsImage +
                   @"', NewsDate = GETDATE(), 
                        NewsContent = N'" + editNews.NewsContent +
                   "', NewsStatus = N'" + editNews.NewsStatus +
                   "', NewsView = '" + editNews.NewsView +
                   "' WHERE NewsId = " + editNews.NewsId.ToString();
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("Failed to edit!");
            }else
            {
                string sql = @"
                UPDATE News 
                    SET NewsCategoryId = '" + editNews.NewsCategoryId +
                   "', NewsTitle = N'" + editNews.NewsTitle +
                   "', NewsAuthor = N'" + editNews.NewsAuthor +
                   @"', NewsDate = GETDATE(), 
                        NewsContent = N'" + editNews.NewsContent +
                   "', NewsStatus = N'" + editNews.NewsStatus +
                   "', NewsView = '" + editNews.NewsView +
                   "' WHERE NewsId = " + editNews.NewsId.ToString();
                Console.WriteLine(sql);
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("Index");
                }else
                {
                    throw new Exception("Failed to edit!");
                }
                
            }
            throw new Exception("Failed to Upload File!");
        }

        public IActionResult DeleteNews(int delete) 
        {
            string sql = "DELETE FROM News WHERE NewsId = " + delete;

            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("Index");
            }
            throw new Exception("Failed to delete news!");
        }

        public IActionResult CreateNewsCategory()
        {
            string sql = @"SELECT 
                                [NewsCategoryId],
                                [NewsCategoryTitle] FROM NewsCategory";
            var categoryNews = _dapper.LoadData<NewsCategory>(sql);

            ViewBag.CategoryNews = categoryNews;
            return View();
        }

        [HttpPost]
        public IActionResult CreateNewsCategory(NewsCategory newsCategory) 
        {
            if (!string.IsNullOrEmpty(newsCategory.NewsCategoryTitle))
            {
                string sqlAdd = @"INSERT INTO NewsCategory (NewsCategoryTitle)
                                VALUES (N'" + newsCategory.NewsCategoryTitle + "');";
                if (_dapper.ExecuteSql(sqlAdd))
                {
                    return RedirectToAction("CreateNewsCategory");
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult DeleteNewsCategory(int delete)
        {
            string sql = "DELETE FROM NewsCategory WHERE NewsCategoryId = " + delete;

            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("CreateNewsCategory");
            }
            throw new Exception("Failed to delete category!");
        }

        [HttpPost]
        public IActionResult ApplyAction(IFormCollection form)
        {
            string action = form["action"].FirstOrDefault() ?? string.Empty;
            string selectedIds = form["selectedIds"].FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(selectedIds))
            {
                return RedirectToAction("Error");
            }

            List<int> selectedIdList = selectedIds.Split(',').Select(int.Parse).ToList();

            switch (action)
            {
                case "published":
                    foreach (int newsId in selectedIdList)
                    {
                        string sqlpublish = @"UPDATE News SET NewsStatus = 'published' WHERE NewsId = " + newsId;
                        _dapper.ExecuteSql(sqlpublish);
                    }
                    break;

                case "draft":
                    foreach (int newsId in selectedIdList)
                    {
                        string sqldraft = @"UPDATE News SET NewsStatus = 'draft' WHERE NewsId = " + newsId;
                        _dapper.ExecuteSql(sqldraft);
                    }
                    break;

                case "delete":
                    foreach (int newsId in selectedIdList)
                    {
                        string sqldelete = @"DELETE FROM NewsCategory WHERE NewsCategoryId = " + newsId;
                        _dapper.ExecuteSql(sqldelete);
                    }
                    break;

                case "clone":
                    foreach (int newsId in selectedIdList)
                    {
                        string sqlclone = @"INSERT INTO News (
                                                [NewsCategoryId],
                                                [NewsTitle],
                                                [NewsAuthor],
                                                [NewsImage],
                                                [NewsDate],
                                                [NewsContent],
                                                [NewsStatus],
                                                [NewsView])
                                                SELECT
                                                [NewsCategoryId],
                                                [NewsTitle],
                                                [NewsAuthor],
                                                [NewsImage],
                                                [NewsDate],
                                                [NewsContent],
                                                [NewsStatus],
                                                [NewsView]
                                                FROM News
                                                WHERE NewsId = " + newsId;
                        _dapper.ExecuteSql(sqlclone);
                    }
                    break;

                default:
                    // Xử lý khi action không hợp lệ
                    // Có thể chuyển hướng đến trang lỗi hoặc hiển thị thông báo lỗi
                    return RedirectToAction("Error");
            }

            // Chuyển hướng hoặc trả về view tùy thuộc vào logic của bạn
            return RedirectToAction("Index");
        }
    }
}
