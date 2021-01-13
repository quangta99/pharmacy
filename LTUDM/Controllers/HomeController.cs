using LTUDM.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTUDM.Controllers
{
    public class HomeController : Controller
    {

        private IMongoCollection<ProductModel> GetProducts;
        private IMongoCollection<CategoryModel> GetCategory;
        private IMongoCollection<BannerModels> GetBanner;
        public HomeController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetProducts = db.GetCollection<ProductModel>("Products");
            this.GetCategory = db.GetCollection<CategoryModel>("Category");
            this.GetBanner = db.GetCollection<BannerModels>("Banner");
        }
        public ActionResult Index()
        {
            var model = GetProducts.Find
            (FilterDefinition<ProductModel>.Empty).ToList();


            ViewBag.Banner = GetBanner.Find
            (FilterDefinition<BannerModels>.Empty).ToList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }       
    }
}