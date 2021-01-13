using LTUDM.Areas.Admin.Models;
using LTUDM.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTUDM.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private IMongoCollection<ProductModel> GetProducts;
        private IMongoCollection<CategoryModel> GetCategory;
       
        public ProductController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetProducts = db.GetCollection<ProductModel>("Products");
            this.GetCategory = db.GetCollection<CategoryModel>("Category");
          
        }
        // GET: Admin/Product

        public ActionResult Product()
        {
            if (Session["admin"] != null)
            {
                var model = GetProducts.Find
            (FilterDefinition<ProductModel>.Empty).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Sản phẩm ngừng kinhh doanh
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Productdel()
        {
            if (Session["admin"] != null)
            {
                var model = GetProducts.Find
            (FilterDefinition<ProductModel>.Empty).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult SingleProduct(string ID)
        {
            if (Session["admin"] != null)
            {

                ProductModel model = GetProducts.Find(n => n.ProductID == ID).FirstOrDefault();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult CreatProduct()
        {
            if (Session["admin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UpdateProduct(string ID)
        {

            if (Session["admin"] != null)
            {
                ProductModel model = GetProducts.Find(n => n.ProductID == ID).FirstOrDefault();
              
                return View(model);

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UpdateProduct2(FormCollection collection)
        {

            if (Session["admin"] != null)
            {

                string ID = collection["ID"].ToString();

                ProductModel model = GetProducts.Find(n => n.ProductID == ID).FirstOrDefault();
                string name = collection["name"].ToString();
                decimal price = 0;
                if (collection["price"] != "")
                {
                    price = decimal.Parse(collection["price"]);
                }
                int discount = 0;
                if (collection["discount"] != "")
                {
                   discount = int.Parse(collection["discount"]);
                }
                int quantity = 0;
                if (collection["quantity"] != "")
                {
                    quantity = int.Parse(collection["quantity"]);
                }

              
                 

                string description = collection["description"].ToString();
                string Producer = collection["Producer"].ToString();
                string img1 = collection["img1"].ToString();
                string img2 = collection["img2"].ToString();
                string img3 = collection["img3"].ToString();
                var filter = Builders<ProductModel>.Filter.Eq("ProductID", ID);

                if(name != "")
                {
                    var update = Builders<ProductModel>.Update.Set("ProductName", name);
                    GetProducts.UpdateOne(filter, update);
                }
                if (price != 0)
                {
                    var update = Builders<ProductModel>.Update.Set("Price", price);
                    GetProducts.UpdateOne(filter, update);
                }
                if (discount != 0)
                {
                    var update = Builders<ProductModel>.Update.Set("discount", discount);
                    GetProducts.UpdateOne(filter, update);
                }
                if (description != "")
                {
                    var update = Builders<ProductModel>.Update.Set("Description", description);
                    GetProducts.UpdateOne(filter, update);
                }
                if (quantity != 0)
                {
                    var update = Builders<ProductModel>.Update.Set("Quantity", quantity);
                    GetProducts.UpdateOne(filter, update);
                }
                if (Producer != "")
                {
                    var update = Builders<ProductModel>.Update.Set("Producer", Producer);
                    GetProducts.UpdateOne(filter, update);
                }
                if (img1 != "")
                {
                    var update = Builders<ProductModel>.Update.Set(x => x.img[0], img1);
                    GetProducts.UpdateOne(filter, update);
                }
                if (img2 != "")
                {
                    var update = Builders<ProductModel>.Update.Set(x => x.img[1], img2);
                    GetProducts.UpdateOne(filter, update);
                }
                if (img3 != "")
                {
                    var update = Builders<ProductModel>.Update.Set(x => x.img[2], img3);
                    GetProducts.UpdateOne(filter, update);
                }

             

                return RedirectToAction("Product");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CreatProduct(ProductModel createPro)
        {

            if (Session["admin"] != null)
            {
                createPro.status = true;
                var cat = GetCategory.Find(m => m.CategoryID == createPro.Type.CategoryID).FirstOrDefault();
                if (cat != null)
                {
                    createPro.Type.CategoryName = cat.CategoryName;

                    var pro = GetProducts.Find(n => n.ProductID == createPro.ProductID).FirstOrDefault();

                    if (pro != null)
                    {
                        ViewBag.CreateProError = "Mã sản phẩm đã tồn tại.";
                    }
                    else
                    {
                        GetProducts.InsertOne(createPro);
                        ViewBag.CreateProError = "Thêm sản phẩm thành công.";
                    }
                }
                else
                {
                    ViewBag.CreateProError = "Mã loại sản phẩm không đúng.";
                }


                return View("CreatProduct");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }


        public ActionResult Delete(string id)
        {
  
            var v = GetProducts.Find(n => n.ProductID == id).FirstOrDefault();
            if (v != null)
            {
                var filter = Builders<ProductModel>.Filter.Eq("ProductID", v.ProductID);
               
                var Update = Builders<ProductModel>.Update.Set("status", false);
                GetProducts.UpdateOne(filter, Update);
            }
       
            ViewBag.Message = "Đã chuyển sản phẩm thành mặt hàng ngừng kinh doanh";
            return RedirectToAction("Product");
        }

        public ActionResult Return(string id)
        {

            var v = GetProducts.Find(n => n.ProductID == id).FirstOrDefault();
            if (v != null)
            {
                var filter = Builders<ProductModel>.Filter.Eq("ProductID", v.ProductID);

                var Update = Builders<ProductModel>.Update.Set("status", true);
                GetProducts.UpdateOne(filter, Update);
            }

            ViewBag.Message = "Đã thêm vào mặt hàng kinh doanh!";
            return RedirectToAction("Productdel");
        }

    }
}