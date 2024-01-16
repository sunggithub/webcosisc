using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Models;

namespace Website.Controllers
{
    public class AboutController : Controller
    {
        private readonly DataContextDapper _dapper;

        public AboutController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Introduction()
        {
            return View();
        }
        public IActionResult Structure()
        {
            string sql = @"SELECT 
                                [DepartmentId],
                                [DepartmentStrucId],
                                [DepartmentJobTitle],
                                [DepartmentName],
                                [DepartmentPhone],
                                [DepartmentEmail],
                                [DepartmentImage],
                                [DepartmentDescrip]
                        FROM Department";
            return View(_dapper.LoadData<Department>(sql));
        }
    }
}
