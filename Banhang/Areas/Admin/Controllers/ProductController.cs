using Microsoft.AspNetCore.Mvc;
using Banhang.Data;
using Banhang.Models;

namespace Banhang.Areas.Admin.Controllers
{
    public class ProductController : AdminBaseController
    {
        private readonly ProductDAO _productDao;
        private readonly CategoryDAO _categoryDao;
        public ProductController(ProductDAO p, CategoryDAO c) { _productDao = p; _categoryDao = c; }

        public IActionResult Index()
        {
            var guard = GuardAdminOrEmployee(); if (guard != null) return guard;
            var list = _productDao.GetAllProducts();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var guard = GuardAdminOrEmployee(); if (guard != null) return guard;
            ViewBag.Categories = _categoryDao.GetAllCategories();
            return View(new Product { IsActive = true, Stock = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product p)
        {
            var guard = GuardAdminOrEmployee(); if (guard != null) return guard;
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryDao.GetAllCategories();
                return View(p);
            }
            var id = _productDao.InsertProduct(p);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var guard = GuardAdminOrEmployee(); if (guard != null) return guard;
            var p = _productDao.GetProductByID(id);
            if (p == null) return NotFound();
            ViewBag.Categories = _categoryDao.GetAllCategories();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product p)
        {
            var guard = GuardAdminOrEmployee(); if (guard != null) return guard;
            if (!_productDao.UpdateProduct(p))
            {
                ViewBag.Categories = _categoryDao.GetAllCategories();
                ViewBag.Error = "Cập nhật thất bại";
                return View(p);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var guard = GuardAdminOrEmployee(); if (guard != null) return guard;
            _productDao.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}
