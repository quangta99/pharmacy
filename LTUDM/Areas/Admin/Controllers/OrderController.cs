using LTUDM.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTUDM.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private IMongoCollection<OrderModel> GetOrder;
        private IMongoCollection<OrderDetailModel> GetDetail;

        public OrderController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetOrder = db.GetCollection<OrderModel>("Order");
            this.GetDetail = db.GetCollection<OrderDetailModel>("OrderDetail");

        }
        // GET: Admin/Order
        public ActionResult Index()
        {

            if (Session["admin"] != null)
            {
                var model = GetOrder.Find
            (FilterDefinition<OrderModel>.Empty).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Detail(string ID)
        {
            if (Session["admin"] != null)
            {
                    ViewBag.Order = GetOrder.Find(n => n.OrderID == ID).FirstOrDefault();
                  List<OrderDetailModel> model  = GetDetail.Find(n => n.OrderID == ID).ToList();

                return View(model);
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
               var Order = GetOrder.Find(n => n.OrderID == ID).FirstOrDefault();
                var filter = Builders<OrderModel>.Filter.Eq("OrderID", ID);
               
                if (Order.OrderStatus == "Đang Kiểm tra")
                {
                  var  Update = Builders<OrderModel>.Update.Set("OrderStatus", "Đang vận chuyển");
                    GetOrder.UpdateOne(filter, Update);
                }
                if (Order.OrderStatus == "Đang vận chuyển")
                {
                    var Update = Builders<OrderModel>.Update.Set("OrderStatus", "Đã giao hàng");
                    GetOrder.UpdateOne(filter, Update);
                }

                return RedirectToAction("Detail", new {ID = Order.OrderID });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}