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
    }
}
