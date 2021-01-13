using LTUDM.Models;
using MongoDB.Driver;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTUDM.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        private IMongoCollection<ProductModel> GetProducts;
        private IMongoCollection<CategoryModel> GetCategory;
        public ProductController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetProducts = db.GetCollection<ProductModel>("Products");
            this.GetCategory = db.GetCollection<CategoryModel>("Category");
        }
        public ActionResult Product(int? page, string categoryId, string sort, string searchString, string currentFilter)
        {
            var category = GetCategory.Find(FilterDefinition<CategoryModel>.Empty).ToList();
            ViewBag.Category = category;
            ViewBag.CurrentSort = sort;
            ViewBag.NameSortParm = sort == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.PriceSortParm = sort == "price_asc" ? "price_desc" : "price_asc";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var model = GetProducts.Find
            (FilterDefinition<ProductModel>.Empty).ToList();
            List<ProductModel> lsPr = new List<ProductModel>();
            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(s => s.ProductName.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            if (categoryId != null)
            {
                foreach (var item in model)
                {
                    if (item.Type.CategoryID == categoryId)
                    {
                        lsPr.Add(item);
                    }
                }
                model = lsPr;
            }
            switch (sort)
            {
                case "name_desc":
                    model = model.OrderByDescending(s => s.ProductName).ToList();
                    break;
                case "name_asc":
                    model = model.OrderBy(s => s.ProductName).ToList();
                    break;
                case "price_desc":
                    model = model.OrderByDescending(s => s.Price).ToList();
                    break;
                case "price_asc":
                    model = model.OrderBy(s => s.Price).ToList();
                    break;
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ProductDetail(string id)
        {           
                ProductModel model = GetProducts.Find(n => n.ProductID == id).FirstOrDefault();
                return View(model);        
        }
    }
}