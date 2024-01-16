using Microsoft.AspNetCore.Mvc;
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
                        FROM News";
            var news = _dapper.LoadData<News>(sql).ToPagedList(page ?? 1, pageSize);
            return View(news);
        }

        public IActionResult BlogDetail()
        {
            if (HttpContext.Request.Query.TryGetValue("blog", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sql = @"SELECT * FROM News WHERE NewsId = " + itemId;
                    var blog = _dapper.LoadData<News>(sql);

                    string sqlnew = @"SELECT TOP 3 * FROM News ORDER BY NewsDate DESC";
                    var newNews = _dapper.LoadData<News>(sqlnew);

                    string sqlcomment = @"SELECT * FROM Comment WHERE CommentNewsId = " + itemId + @"AND CommentStatus = 'approved' ORDER BY CommentId DESC" ;
                    var comment = _dapper.LoadData<Comment>(sqlcomment);

                    ViewBag.SingleBlog = blog;
                    ViewBag.NewNews = newNews;
                    ViewBag.Comment = comment;

                    return View();

                }
            }
            throw new Exception("Failed to dislay news!");
        }

        [HttpPost]
        public IActionResult BlogDetail(Comment createComment, string name, string email, string msg)
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
                        return RedirectToAction("BlogDetail", "Blog" , new { blog = itemId , area = ""});
                    }
                    return View();

                }
            }
            throw new Exception("Failed to dislay news!");
        }

    }
}
