using Website.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContextDapper _dapper;

        public HomeController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public IActionResult Index()
        {
            string sql = @"SELECT TOP(4) * FROM News ORDER BY NewsDate DESC";
            return View(_dapper.LoadData<News>(sql));
        }
        public IActionResult Test()
        {
            string sql = @"SELECT TOP 1 * FROM News ORDER BY NewsDate DESC";
            var singleNews = _dapper.LoadData<News>(sql);

            string sqlnew = @"SELECT * FROM News 
                                ORDER BY NewsDate DESC 
                                OFFSET 1 ROWS 
                                FETCH NEXT 4 ROWS ONLY ";
            var newNews = _dapper.LoadData<News>(sqlnew);

            string sqlcategory = @"SELECT 
                                    [NewsCategoryId],
                                    [NewsCategoryTitle] FROM NewsCategory";
            var categoryNews = _dapper.LoadData<NewsCategory>(sqlcategory);


            ViewBag.SingleNews = singleNews;
            ViewBag.NewNews = newNews;
            ViewBag.CategoryNews = categoryNews;

            return View();
        }

    }
}
