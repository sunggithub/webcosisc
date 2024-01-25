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
            throw new Exception("Failed to delete comment!");
        }

        [HttpPost]
        public IActionResult ApplyAction(string action, DateTime? startDate, DateTime? endDate, string selectedIds)
        {
            List<Comment> model = new List<Comment>();
            if (startDate.HasValue && endDate.HasValue)
            {
                string formattedStartDate = startDate.Value.ToString("yyyy-MM-dd");
                string formattedEndDate = endDate.Value.ToString("yyyy-MM-dd");

                string sqlDate = @"SELECT * FROM Comment WHERE CommentDate BETWEEN '" + formattedStartDate
                                    + @"' AND '" + formattedEndDate + "'";
                model = _dapper.LoadData<Comment>(sqlDate).ToList();

                if (!string.IsNullOrEmpty(selectedIds))
                {
                    ProcessAction(action, model, selectedIds);
                }
            }
            else
            {
                model = _dapper.LoadData<Comment>("SELECT * FROM Comment ORDER BY CommentDate DESC").ToList();
            }

            if (!string.IsNullOrEmpty(selectedIds))
            {
                ProcessAction(action, model, selectedIds);
            }
            return View("Index", model);
        }

        private void ProcessAction(string action, List<Comment> comments, string selectedIds)
        {
            List<int> commentIds = selectedIds.Split(',').Select(int.Parse).ToList();

            switch (action)
            {
                case "delete":
                    foreach (int commentId in commentIds)
                    {
                        var commentToDelete = comments.FirstOrDefault(c => c.CommentId == commentId);
                        if (commentToDelete != null)
                        {
                            string sqldelete = @"DELETE FROM Comment WHERE CommentId = " + commentId;
                            _dapper.ExecuteSql(sqldelete);
                        }
                    }
                    break;
                case "approve":
                    foreach (int commentId in commentIds)
                    {
                        var commentToApprove = comments.FirstOrDefault(c => c.CommentId == commentId);
                        if (commentToApprove != null)
                        {
                            string sqlapproved = @"UPDATE Comment SET CommentStatus = 'approved' WHERE CommentId = " + commentId;
                            _dapper.ExecuteSql(sqlapproved);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
