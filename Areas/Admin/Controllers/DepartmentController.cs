using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Models;

namespace Website.Areas.Admin.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly DataContextDapper _dapper;

        public DepartmentController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }
        public IActionResult Index(Structure structureAdd)
        {
            string sqlSelect = @"SELECT * FROM Structure";
            var structure = _dapper.LoadData<Structure>(sqlSelect);

            ViewBag.Structure = structure;

            if (!string.IsNullOrEmpty(structureAdd.StructureName))
            {
                string sqlAdd = @"INSERT INTO Structure (StructureName)
                                VALUES (N'" + structureAdd.StructureName + "');";
                if (_dapper.ExecuteSql(sqlAdd))
                {
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult DeleteStructure(int structure)
        {
            string sql = "DELETE FROM Structure WHERE StructureId = " + structure;

            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("Index");
            }
            throw new Exception("Failed to delete structure!");
        }

        public IActionResult ViewAllDepartment()
        {
            string sql = @"SELECT [DepartmentId],
                            [DepartmentStrucId],
                            [DepartmentJobTitle],
                            [DepartmentName],
                            [DepartmentPhone],
                            [DepartmentEmail],
                            [DepartmentImage],
                            [DepartmentDescrip] FROM Department";
            var department = _dapper.LoadData<Department>(sql);

            string sqlstructure = @"SELECT 
                                        [StructureId],
                                        [StructureName] FROM Structure";
            var departmentStruc = _dapper.LoadData<Structure>(sqlstructure);

            ViewBag.Department = department;
            ViewBag.DepartmentStruc = departmentStruc;

            return View();
        }

        public IActionResult CreateDepartment()
        {
            string sql = @"SELECT [StructureId],
                            [StructureName] FROM Structure";
            var departmentStruc = _dapper.LoadData<Structure>(sql);
            ViewBag.DepartmentStructure = departmentStruc;
            return View();
        }
        [HttpPost]
        public IActionResult CreateDepartment(Department createDepartment, IFormFile fileImage)
        {
            if (fileImage != null && fileImage.Length > 0)
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
                createDepartment.DepartmentImage = filename;
                string sql = @"
                    INSERT INTO Department(
                        [DepartmentStrucId],
                        [DepartmentJobTitle],
                        [DepartmentName],
                        [DepartmentPhone],
                        [DepartmentEmail],
                        [DepartmentImage],
                        [DepartmentDescrip]) VALUES ("
                    + "'" + createDepartment.DepartmentStrucId
                    + "',N'" + createDepartment.DepartmentJobTitle
                    + "',N'" + createDepartment.DepartmentName
                    + "',N'" + createDepartment.DepartmentPhone
                    + "',N'" + createDepartment.DepartmentEmail
                    + "',N'" + createDepartment.DepartmentImage
                    + "',N'" + createDepartment.DepartmentDescrip +
                "'" + ")";
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("ViewAllDepartment");
                }
                throw new Exception("Failed to create department!");
            }
            throw new Exception("Failed to Upload File!");

        }

        public IActionResult EditDepartment()
        {
            // Logic lấy ID và đối tượng từ query string
            if (HttpContext.Request.Query.TryGetValue("editDepart", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sql = "SELECT * FROM Department WHERE DepartmentId = " + itemId;
                    var department = _dapper.LoadData<Department>(sql);

                    string sqlstruc = @"SELECT [StructureId],
                            [StructureName] FROM Structure";
                    var departmentStructure = _dapper.LoadData<Structure>(sqlstruc);

                    ViewBag.DepartmentStructure = departmentStructure;

                    var firstDepartment = department.FirstOrDefault(); // hoặc SingleOrDefault hoặc First

                    return View(firstDepartment); 
                }
            }
            throw new Exception("Failed to dislay department to edit!");
        }

        [HttpPost]
        public IActionResult EditDepartment(Department editDepartment, IFormFile fileImage)
        {
            if (fileImage != null && fileImage.Length > 0)
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
                editDepartment.DepartmentImage = filename;
                string sql = @"
                UPDATE Department 
                    SET DepartmentStrucId = '" + editDepartment.DepartmentStrucId +
                   "', DepartmentJobTitle = N'" + editDepartment.DepartmentJobTitle +
                   "', DepartmentName = N'" + editDepartment.DepartmentName +
                   "', DepartmentPhone = N'" + editDepartment.DepartmentPhone +
                   "', DepartmentEmail = N'" + editDepartment.DepartmentEmail +
                   "', DepartmentImage = N'" + editDepartment.DepartmentImage +
                   "', DepartmentDescrip = N'" + editDepartment.DepartmentDescrip +
                   @"' WHERE DepartmentId = " + editDepartment.DepartmentId.ToString();
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("ViewAllDepartment");
                }
                throw new Exception("Failed to edit department!");
            }
            else
            {
                string sql = @"
                UPDATE Department 
                    SET DepartmentStrucId = '" + editDepartment.DepartmentStrucId +
                   "', DepartmentJobTitle = N'" + editDepartment.DepartmentJobTitle +
                   "', DepartmentName = N'" + editDepartment.DepartmentName +
                   "', DepartmentPhone = N'" + editDepartment.DepartmentPhone +
                   "', DepartmentEmail = N'" + editDepartment.DepartmentEmail +
                   "', DepartmentDescrip = N'" + editDepartment.DepartmentDescrip +
                   @"' WHERE DepartmentId = " + editDepartment.DepartmentId.ToString();
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("ViewAllDepartment");
                }
                throw new Exception("Failed to edit department!");
            }    
        }

        [HttpPost]
        public IActionResult DeleteDepartment(int delete)
        {
            string sql = "DELETE FROM Department WHERE DepartmentId = " + delete;

            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("ViewAllDepartment");
            }
            throw new Exception("Failed to delete department!");
        }
    }
}
