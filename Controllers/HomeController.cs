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
            string sqllistNews = @"SELECT TOP 2 * FROM News WHERE NewsCategoryId = 4 ORDER BY NewsDate";
            var listNews = _dapper.LoadData<News>(sqllistNews);

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

            string sqlproduct = @"SELECT TOP 3 * FROM Product ORDER BY ProductReleaseDate DESC";
            var products = _dapper.LoadData<Product>(sqlproduct);

            ViewBag.ListNews = listNews;
            ViewBag.SingleNews = singleNews;
            ViewBag.NewNews = newNews;
            ViewBag.CategoryNews = categoryNews;
            ViewBag.ListProduct = products;

            return View();
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
