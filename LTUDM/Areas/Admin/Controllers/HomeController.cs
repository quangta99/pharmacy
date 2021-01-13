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
    public class HomeController : Controller
    {

        private IMongoCollection<ProductModel> GetProducts;
        private IMongoCollection<CategoryModel> GetCategory;
        private IMongoCollection<AdminModel> GetAdminName;
        private IMongoCollection<UserModel> GetUserName;
        private IMongoCollection<BannerModels> GetBanner;

        public HomeController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetProducts = db.GetCollection<ProductModel>("Products");
            this.GetCategory = db.GetCollection<CategoryModel>("Category");
            this.GetAdminName = db.GetCollection<AdminModel>("Admin");
            this.GetUserName = db.GetCollection<UserModel>("User");
            this.GetBanner = db.GetCollection<BannerModels>("Banner");
        }
        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewBag.Message = "";
            return View();
        }
        //homepage admin
        public ActionResult AdminPage()
        {
            if (Session["admin"] != null)
            { return View(); }
            else
            {
                return RedirectToAction("Index");
            } 
        }



        /// <summary>
        /// User Page
        /// </summary>
        /// <returns></returns>
        public ActionResult UserPage()
        {
            if (Session["admin"] != null)
            {
                var model = GetUserName.Find
            (FilterDefinition<UserModel>.Empty).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult DetailUser(string id)
        {
            if (Session["admin"] != null)
            {
                ObjectId ay = new ObjectId(id);
                UserModel model = GetUserName.Find(n =>  n.Id == ay).FirstOrDefault();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult LoginAdmin(FormCollection collection)
        {
            string TaiKhoan = collection["txtUser"].ToString();
            string  MatKhau = collection["txtPass"].ToString();
            var a = crypt.Hash(MatKhau);
            AdminModel result = GetAdminName.Find(n => n.AdminName == TaiKhoan && n.Password == a).FirstOrDefault();
            if (result != null)
            {
                Session["admin"] = result;
                return RedirectToAction("AdminPage");

            }
            return View("Index");
        }

        public ActionResult LogOutAdmin()
        {
            if (Session["admin"] != null)
            {
                Session["admin"] = null;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        public ActionResult Banner()
        {
            if (Session["admin"] != null)
            {
                var model = GetBanner.Find
          (FilterDefinition<BannerModels>.Empty).ToList();
                ViewBag.IMG0 = model[0].Img;
                ViewBag.Name0 = model[0].BannerName;
                ViewBag.IMG1 = model[1].Img;
                ViewBag.Name1 = model[1].BannerName;
                ViewBag.IMG2 = model[2].Img;
                ViewBag.Name2 = model[2].BannerName;


                return View();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Banner1(FormCollection collection, HttpPostedFileBase img)
        {
            string name = collection["name"].ToString();

            var model = GetBanner.Find
            (FilterDefinition<BannerModels>.Empty).ToList();
         
            string imgName = null;
            if (img != null)
            {
                imgName = Path.GetFileName(img.FileName);
                string directFolder = Server.MapPath("~/Banner/" + name+ "_" + imgName);
                string oldImg = Server.MapPath("~/Banner/" + model[0].Img);
                if (imgName != null)
                {
                    System.IO.File.Delete(oldImg);
                }
                img.SaveAs(directFolder);
            }
            if (name != "")
            {
                if (model != null)
                {
                    var filter = Builders<BannerModels>.Filter.Eq("Id", model[0].Id);
                    var updateName = Builders<BannerModels>.Update.Set("BannerName", name);
                    GetBanner.UpdateOne(filter, updateName);
                    var Updateimg = Builders<BannerModels>.Update.Set("Img", name + "_" + imgName);
                    GetBanner.UpdateOne(filter, Updateimg);
                }
                {
                    BannerModels bn = new BannerModels();
                    bn.BannerName = name;
                    bn.Img = name + "_" + imgName;

                    GetBanner.InsertOne(bn);
                }

            }

            return RedirectToAction("Banner");
        }

        public ActionResult Banner2(FormCollection collection, HttpPostedFileBase img)
        {
            string name = collection["name"].ToString();

            var model = GetBanner.Find
            (FilterDefinition<BannerModels>.Empty).ToList();

            string imgName = null;
            if (img != null)
            {
                imgName = Path.GetFileName(img.FileName);
                string directFolder = Server.MapPath("~/Banner/" + name + "_" + imgName);
                string oldImg = Server.MapPath("~/Banner/" + model[1].Img);
                if (imgName != null)
                {
                    System.IO.File.Delete(oldImg);
                }
                img.SaveAs(directFolder);
            }
            if (name != "")
            {
                if (model != null)
                {
                    var filter = Builders<BannerModels>.Filter.Eq("Id", model[1].Id);
                    var updateName = Builders<BannerModels>.Update.Set("BannerName", name);
                    GetBanner.UpdateOne(filter, updateName);
                    var Updateimg = Builders<BannerModels>.Update.Set("Img", name + "_" + imgName);
                    GetBanner.UpdateOne(filter, Updateimg);
                }
                

            }

            return RedirectToAction("Banner");
        }

        public ActionResult Banner3(FormCollection collection, HttpPostedFileBase img)
        {
            string name = collection["name"].ToString();

            var model = GetBanner.Find
            (FilterDefinition<BannerModels>.Empty).ToList();

            string imgName = null;
            if (img != null)
            {
                imgName = Path.GetFileName(img.FileName);
                string directFolder = Server.MapPath("~/Banner/" + name + "_" + imgName);
                string oldImg = Server.MapPath("~/Banner/" + model[2].Img);
                if (imgName != null)
                {
                    System.IO.File.Delete(oldImg);
                }
                img.SaveAs(directFolder);
            }
            if (name != "")
            {
                if (model != null)
                {
                    var filter = Builders<BannerModels>.Filter.Eq("Id", model[2].Id);
                    var updateName = Builders<BannerModels>.Update.Set("BannerName", name);
                    GetBanner.UpdateOne(filter, updateName);
                    var Updateimg = Builders<BannerModels>.Update.Set("Img", name + "_" + imgName);
                    GetBanner.UpdateOne(filter, Updateimg);
                }
                {
                    BannerModels bn = new BannerModels();
                    bn.BannerName = name;
                    bn.Img = name + "_" + imgName;

                    GetBanner.InsertOne(bn);
                }

            }

            return RedirectToAction("Banner");
        }

    }
}