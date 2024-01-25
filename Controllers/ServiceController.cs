using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Website.Data;
using Website.Models;
using X.PagedList;

namespace Website.Controllers
{
    public class ServiceController : Controller
    {
        private readonly DataContextDapper _dapper;

        public ServiceController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index(int? page)
        {
            int pageSize = 6; // Số lượng sản phẩm trên mỗi trang

            string sql = @"SELECT * FROM Product";
            var products = _dapper.LoadData<Product>(sql).ToPagedList(page ?? 1, pageSize);

            return View(products);
        }

        public ActionResult Details() 
        {
            if (HttpContext.Request.Query.TryGetValue("product", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sql = @"SELECT [ProductId],
                                    [ProductTitle],
                                    [ProductAuthor],
                                    [ProductImage],
                                    [ProductDescription],
                                    [ProductStatus],
                                    [ProductReleaseDate] FROM Product WHERE ProductId = " + itemId + @" ORDER BY ProductReleaseDate DESC";
                    var product = _dapper.LoadData<Product>(sql);

                    string sqlrelate = @"SELECT TOP 3 * FROM Product ORDER BY ProductReleaseDate DESC";
                    var productrelate = _dapper.LoadData<Product>(sqlrelate);

                    string sqlcategory = @"SELECT 
                                    [NewsCategoryId],
                                    [NewsCategoryTitle] FROM NewsCategory";
                    var categoryNews = _dapper.LoadData<NewsCategory>(sqlcategory);

                    ViewBag.SingleProduct = product;
                    ViewBag.CategoryNews = categoryNews;
                    ViewBag.ProductRelate = productrelate;

                    return View(product);

                }
            }
            throw new Exception("Failed to dislay news!");
        }

        [HttpPost]
        public IActionResult SearchResults(string searchKeyword)
        {
            string sql = @"SELECT [ProductId],
                                [ProductTitle],
                                [ProductAuthor],
                                [ProductImage],
                                [ProductDescription],
                                [ProductStatus],
                                [ProductReleaseDate] FROM Product
                            WHERE ProductTitle LIKE N'%" + searchKeyword + "%'" +
                        @" OR ProductDescription LIKE N'%" + searchKeyword + "%'";

            var searchResults = _dapper.LoadData<Product>(sql);

            if (searchResults.Any())
            {
                ViewBag.SearchResults = searchResults;
            }
            else
            {
                ViewBag.SearchResults = new List<Product>();
            }
            ViewBag.SearchKeyword = searchKeyword;
            return View();
        }
    }
}
