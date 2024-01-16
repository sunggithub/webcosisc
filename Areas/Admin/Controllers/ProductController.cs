using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Helpers;
using Website.Models;

namespace Website.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public ProductController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }
        public IActionResult Index()
        {
            string sql = @"SELECT * FROM Product";
            return View(_dapper.LoadData<Product>(sql));
        }

        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateProduct(Product createProduct, IFormFile fileImage)
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
                createProduct.ProductImage = filename;

                string sql = @"
                INSERT INTO Product(
                    [ProductTitle],
                    [ProductAuthor],
                    [ProductImage],
                    [ProductDescription],
                    [ProductStatus],
                    [ProductReleaseDate]) VALUES ("
                    + "N'" + createProduct.ProductTitle
                    + "',N'" + createProduct.ProductAuthor
                    + "',N'" + createProduct.ProductImage
                    + "',N'" + createProduct.ProductDescription
                    + "',N'" + createProduct.ProductStatus
                    + "', GETDATE() )";
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("Failed to create Product!");
            }
            throw new Exception("Failed to Upload File!");
        }

        public IActionResult EditProduct()
        {
            // Logic lấy ID và đối tượng từ query string
            if (HttpContext.Request.Query.TryGetValue("editProduct", out var id))
            {
                if (int.TryParse(id, out int itemId))
                {
                    string sql = "SELECT * FROM Product WHERE ProductId = " + itemId;
                    var product = _dapper.LoadData<Product>(sql);
                    var firstProduct = product.FirstOrDefault(); // hoặc SingleOrDefault hoặc First

                    return View(firstProduct);
                }
            }
            throw new Exception("Failed to dislay product to edit!");
        }

        [HttpPost]
        public IActionResult EditProduct(Product editProduct, IFormFile fileImage)
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
                editProduct.ProductImage = filename;
                string sql = @"
                UPDATE Product 
                    SET ProductTitle = '" + editProduct.ProductTitle +
                   "', ProductAuthor = N'" + editProduct.ProductAuthor +
                   "', ProductImage = N'" + editProduct.ProductImage +
                   "', ProductDescription = N'" + editProduct.ProductDescription +
                   "', ProductStatus = N'" + editProduct.ProductStatus +
                   "', ProductReleaseDate = N'" + editProduct.ProductReleaseDate +
                   @"' WHERE ProductId = " + editProduct.ProductId.ToString();
                        if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("Failed to edit Product!");
            }
            else
            {
                string sql = @"
                UPDATE Product 
                    SET ProductTitle = '" + editProduct.ProductTitle +
                   "', ProductAuthor = N'" + editProduct.ProductAuthor +
                   "', ProductDescription = N'" + editProduct.ProductDescription +
                   "', ProductStatus = N'" + editProduct.ProductStatus +
                   "', ProductReleaseDate = N'" + editProduct.ProductReleaseDate +
                   @"' WHERE ProductId = " + editProduct.ProductId.ToString();
                if (_dapper.ExecuteSql(sql))
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("Failed to edit Product!");
            }
        }

        public IActionResult DeleteProduct(int delete)
        {
            string sql = "DELETE FROM Product WHERE ProductId = " + delete;

            if (_dapper.ExecuteSql(sql))
            {
                return RedirectToAction("Index");
            }
            throw new Exception("Failed to delete product!");
        }
    }
}
