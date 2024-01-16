using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Models;

namespace Website.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class CommentController : Controller
    {
        private readonly DataContextDapper _dapper;

        public CommentController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index()
        {
            string sql = @"SELECT [CommentId],
                            [CommentNewsId],
                            [CommentAuthor],
                            [CommentEmail],
                            [CommentStatus],
                            [CommentContent],
                            [CommentDate] FROM Comment ORDER BY CommentDate DESC";

            if (HttpContext.Request.Query.TryGetValue("approve", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sqlApprove = @"UPDATE Comment SET CommentStatus = 'approved' WHERE CommentId = " + itemId ;
                    if (_dapper.ExecuteSql(sqlApprove))
                    {
                        return RedirectToAction("Index");
                    }
                    throw new Exception("Failed to update Status!");
                }
            }

            if (HttpContext.Request.Query.TryGetValue("unapprove", out var id2))
            {
                if (int.TryParse(id2, out int itemId))
                {
                    string sqlApprove = @"UPDATE Comment SET CommentStatus = 'unapprove' WHERE CommentId = " + itemId;
                    if (_dapper.ExecuteSql(sqlApprove))
                    {
                        return RedirectToAction("Index");
                    }
                    throw new Exception("Failed to update Status!");
                }
            }
            return View(_dapper.LoadData<Comment>(sql));
        }
        public IActionResult DeleteComment(int delete)
        {
            string sql = "DELETE FROM Comment WHERE CommentId = " + delete;

            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("Index");
            }
            throw new Exception("Failed to delete product!");
        }
    }
}
