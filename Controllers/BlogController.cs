using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Website.Data;
using Website.Models;
using X.PagedList;

namespace Website.Controllers
{
    public class BlogController : Controller
    {
        private readonly DataContextDapper _dapper;

        public BlogController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index(int? page)
        {
            int pageSize = 3; // Số lượng sản phẩm trên mỗi trang
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
            var news = _dapper.LoadData<News>(sql).ToPagedList(page ?? 1, pageSize);


            return View(news);
        }

        private void UpdateViewsForNews(int blog)
        {
            string updateSql = @"UPDATE News SET NewsView = NewsView + 1 WHERE NewsId = " + blog;
            _dapper.ExecuteSql(updateSql);
        }

        public IActionResult BlogDetail(string searchKeyword, int? blog)
        {
            if (HttpContext.Request.Query.TryGetValue("blog", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sql = @"SELECT * FROM News WHERE NewsId = " + itemId;
                    var blogs = _dapper.LoadData<News>(sql);

                    string sqlnew = @"SELECT TOP 3 * FROM News ORDER BY NewsDate DESC";
                    var newNews = _dapper.LoadData<News>(sqlnew);

                    string sqlcategory = @"SELECT 
                                    [NewsCategoryId],
                                    [NewsCategoryTitle] FROM NewsCategory";
                    var categoryNews = _dapper.LoadData<NewsCategory>(sqlcategory);

                    string sqlcomment = @"SELECT * FROM Comment WHERE CommentNewsId = " + itemId + @" AND CommentStatus = 'approved' ORDER BY CommentId DESC" ;
                    var comment = _dapper.LoadData<Comment>(sqlcomment);

                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        return RedirectToAction("SearchResults");
                    }

                    if (blog.HasValue && blog > 0)
                    {
                        UpdateViewsForNews(blog.Value);
                    }

                    ViewBag.SingleBlog = blogs;
                    ViewBag.NewNews = newNews;
                    ViewBag.CategoryNews = categoryNews;
                    ViewBag.Comment = comment;

                    return View();

                }
            }
            throw new Exception("Failed to dislay news!");
        }

        [HttpPost]
        public IActionResult BlogDetail(Comment createComment)
        {
            if (HttpContext.Request.Query.TryGetValue("blog", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sqlcreate = @"
                            INSERT INTO Comment (
                                [CommentNewsId],
                                [CommentAuthor],
                                [CommentEmail],
                                [CommentStatus],
                                [CommentContent],
                                [CommentDate]
                            ) VALUES (
                                " + itemId + @",
                                N'" + createComment.CommentAuthor + @"',
                                N'" + createComment.CommentEmail + @"',
                                N'Unapproved',
                                N'" + createComment.CommentContent + @"',
                                GETDATE()
                            )";
                    if (_dapper.ExecuteSql(sqlcreate))
                    {
                        TempData["CommentSuccessMessage"] = "Bình luận của bạn đã được ghi nhận và đang chờ xét duyệt.";
                        return RedirectToAction("BlogDetail", "Blog" , new { blog = itemId , area = ""});
                    }
                    else
                    {
                        return View();
                    }

                }
            }
            throw new Exception("Failed to dislay news!");
        }

        [HttpPost]
        public IActionResult SearchResults(string searchKeyword)
        {
            string sql = @"SELECT [NewsId],
                            [NewsCategoryId],
                            [NewsTitle],
                            [NewsAuthor],
                            [NewsImage],
                            [NewsDate],
                            [NewsContent],
                            [NewsStatus],
                            [NewsView] FROM News
                            WHERE NewsTitle LIKE N'%" + searchKeyword + "%'" +
                        @" OR NewsContent LIKE N'%" + searchKeyword + "%'";

            var searchResults = _dapper.LoadData<News>(sql);

            if (searchResults.Any())
            {
                ViewBag.SearchResults = searchResults;
            }
            else
            {
                ViewBag.SearchResults = new List<News>();
            }
            ViewBag.SearchKeyword = searchKeyword;
            return View();
        }

        public IActionResult Category(string category, int? page) 
        {
            int pageSize = 3;
            string sql = @"SELECT * FROM News WHERE NewsCategoryId = " + category;
            var news = _dapper.LoadData<News>(sql).ToPagedList(page ?? 1, pageSize);

            string sqlcategory = @"SELECT 
                                    [NewsCategoryTitle] FROM NewsCategory WHERE NewsCategoryId = " + category;
            var categoryNews = _dapper.LoadData<NewsCategory>(sqlcategory);

            ViewBag.CategoryNews = categoryNews;

            return View(news);
        }
    }
}
