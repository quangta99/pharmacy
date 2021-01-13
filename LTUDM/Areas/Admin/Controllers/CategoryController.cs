﻿using LTUDM.Areas.Admin.Models;
using LTUDM.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace LTUDM.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Admin/Category
        private IMongoCollection<CategoryModel> GetCategory;

        public CategoryController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetCategory = db.GetCollection<CategoryModel>("Category");

        }

        /// <summary>
        /// Categories page
        /// </summary>
        /// <returns></returns>
        public ActionResult Categories()
        {
            if (Session["admin"] != null)
            {
                var model = GetCategory.Find
            (FilterDefinition<CategoryModel>.Empty).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult CreatCategory()
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

        public ActionResult Update(string ID)
        {
            if (Session["admin"] != null)
            {
                CategoryModel model = GetCategory.Find(n => n.CategoryID == ID).FirstOrDefault();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Update2(FormCollection collection)
        {
            if (Session["admin"] != null)
            {
                string ID = collection["ID"].ToString();
                CategoryModel model = GetCategory.Find(n => n.CategoryID == ID).FirstOrDefault();
                string name = collection["name"].ToString();
                var filter = Builders<CategoryModel>.Filter.Eq("CategoryID", ID);

                if (name != "")
                {
                    var update = Builders<CategoryModel>.Update.Set("CategoryName", name);
                    GetCategory.UpdateOne(filter, update);
                }
                return RedirectToAction("Categories");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="createCat">New category</param>
        /// <returns>nếu đã tồn tài thì không add mới trả về thông báo đã tồn tại, ngược lại add new 1 category va thông bào thành công</returns>
        [HttpPost]
        public ActionResult CreatCategory(CategoryModel createCat)
        {
            if (Session["admin"] != null)
            {

                var cat = GetCategory.Find(m => m.CategoryID == createCat.CategoryID).FirstOrDefault();
                if (cat != null)
                {
                    ViewBag.CreateProError = "Mã sản phẩm đã tồn tại.";
                }
                else
                {
                    GetCategory.InsertOne(createCat);
                    ViewBag.CreateProError = "Thêm sản phẩm thành công.";
                }


                return View("CreatCategory");
            }
            else
            {
                return RedirectToAction("Index","Home");
            }

        }
    }
}