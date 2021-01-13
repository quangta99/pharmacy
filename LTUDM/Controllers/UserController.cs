using LTUDM.Models;
using Microsoft.Ajax.Utilities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTUDM.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private IMongoCollection<UserModel> GetUserModel;
        private IMongoCollection<OrderModel> GetOrderModel;
        private IMongoCollection<OrderDetailModel> GetOrderDetail;
        private IMongoCollection<ProductModel> GetProducts;
        public UserController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetUserModel = db.GetCollection<UserModel>("User");
            this.GetOrderModel = db.GetCollection<OrderModel>("Order");
            this.GetOrderDetail = db.GetCollection<OrderDetailModel>("OrderDetail");
            this.GetProducts = db.GetCollection<ProductModel>("Products");
        }
        [HttpGet]
        public ActionResult Infor()
        {
            UserModel model = (UserModel)Session["user"];
            var order = GetOrderModel.Find(a => a.UserID == model.Id.ToString()).ToList();
            ViewBag.Order = order;
            return View(model);
        }
        public ActionResult ChangePassword(FormCollection collection)
        {
            string message = "";
            string newPass = collection["newPass"].ToString();
            string oldPass = Crypt.Hash(collection["oldPass"].ToString());
            UserModel model = (UserModel)Session["user"];
            if (oldPass != model.Password)
            {
                message = "Mật khẩu cũ không đúng";
            }
            else if (oldPass == model.Password)
            {
                var v = GetUserModel.Find(a => a.Email == model.Email).FirstOrDefault();
                if (v != null)
                {
                    v.Password = Crypt.Hash(newPass);
                    var filter = Builders<UserModel>.Filter.Eq("Email", v.Email);
                    var update = Builders<UserModel>.Update.Set("Password", v.Password);
                    GetUserModel.UpdateOne(filter, update);
                }
            }
            return RedirectToAction("Infor");
        }
        public ActionResult ChangeInfor(FormCollection collection, HttpPostedFileBase img)
        {
            UserModel model = (UserModel)Session["user"];
            string imgName = null;
            if (img != null)
            {
                imgName = Path.GetFileName(img.FileName);
                string directFolder = Server.MapPath("~/Image/" + model.Email + "_" + imgName);
                string oldImg = Server.MapPath("~/Image/" + model.Img);
                if (model.Img != null)
                {
                    System.IO.File.Delete(oldImg);
                }
                img.SaveAs(directFolder);
            }
            string name = collection["name"].ToString();
            string direct = collection["direct"].ToString();
            string phone = collection["phone"].ToString();
            var v = GetUserModel.Find(a => a.Email == model.Email).FirstOrDefault();
            if (v != null)
            {
                if (name != "")
                {
                    v.Name = name;
                }
                else
                {
                    v.Name = model.Name;
                }
                if (phone != "")
                {
                    v.Phone = phone;
                }
                else
                {
                    v.Phone = model.Phone;
                }
                if (direct != "")
                {
                    v.Direct = direct;
                }
                else
                {
                    v.Direct = model.Direct;
                }
                if (img == null)
                {
                    v.Img = model.Img;
                }
                else
                {
                    v.Img = v.Email + "_" + imgName;
                }
                model.Name = v.Name;
                model.Phone = v.Phone;
                model.Direct = v.Direct;
                model.Img = v.Img;
                Session["user"] = model;
                var filter = Builders<UserModel>.Filter.Eq("Email", v.Email);
                var updateName = Builders<UserModel>.Update.Set("Name", v.Name);
                var updateDirect = Builders<UserModel>.Update.Set("Direct", v.Direct);
                var updatePhone = Builders<UserModel>.Update.Set("Phone", v.Phone);
                var updateImg = Builders<UserModel>.Update.Set("Img", v.Img);
                GetUserModel.UpdateOne(filter, updateName);
                GetUserModel.UpdateOne(filter, updateDirect);
                GetUserModel.UpdateOne(filter, updatePhone);
                GetUserModel.UpdateOne(filter, updateImg);
            }
            return RedirectToAction("Infor");
        }
        public ActionResult LogOut()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult OrderDetail(string id)
        {
            var model = GetOrderModel.Find(o => o.OrderID == id).FirstOrDefault();
            var orderDetail = GetOrderDetail.Find(FilterDefinition<OrderDetailModel>.Empty).ToList();
            List<OrderDetailModel> orderDetails = new List<OrderDetailModel>();
            foreach (var item in orderDetail)
            {
                if (item.OrderID == id)
                {
                    orderDetails.Add(item);
                }
            }
            var products = GetProducts.Find(FilterDefinition<ProductModel>.Empty).ToList();
            List<ProductModel> prOrder = new List<ProductModel>();
            foreach (var itemPr in products)
            {
                foreach (var itemOr in orderDetails)
                {
                    if (itemOr.ProductID == itemPr.ProductID)
                    {
                        ProductModel product = new ProductModel();
                        product.img = itemPr.img;
                        product.ProductName = itemPr.ProductName;
                        product.Price = itemOr.SalePrice;
                        product.Quantity = itemOr.Quantity;
                        prOrder.Add(product);
                    };
                }
            }
            ViewBag.ProductOrder = prOrder;
            return View(model);
        }
    }
}