﻿using LTUDM.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTUDM.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private IMongoCollection<ProductModel> GetProducts;
        private IMongoCollection<CategoryModel> GetCategory;
        private IMongoCollection<OrderModel> GetOrder;
      
        private IMongoCollection<OrderDetailModel> GetDetail;


        private IMongoCollection<BagModel> GetBagOfUser;
        decimal total;
        public CartController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetProducts = db.GetCollection<ProductModel>("Products");
            this.GetCategory = db.GetCollection<CategoryModel>("Category");
            this.GetOrder = db.GetCollection<OrderModel>("Order");
            this.GetDetail = db.GetCollection<OrderDetailModel>("OrderDetail");
        
            this.GetBagOfUser = db.GetCollection<BagModel>("Cart");

        }
        public ActionResult Cart()
        {
            UserModel User = (UserModel)Session["user"];
            List<ProductModel> productsList = new List<ProductModel>();
            var model = GetProducts.Find
            (FilterDefinition<ProductModel>.Empty).ToList();
            ViewBag.prDatabase = model;
            if (User == null) {
                List<CartModel> cartList = (List<CartModel>)Session["cart"];
                if (cartList != null)
                {
                    foreach (var item in cartList)
                    {
                        foreach (var i in model)
                        {
                            if (i.ProductID == item.ProductID)
                            {
                                ProductModel product = new ProductModel();
                                product.ProductID = i.ProductID;
                                product.ProductName = i.ProductName;
                                product.img = i.img;
                                if (i.discount != 0)
                                {
                                    product.Price =i.Price- i.Price * i.discount / 100;
                                }
                                else
                                {
                                    product.Price = i.Price;
                                }
                                product.Quantity = item.Quantity;
                                productsList.Add(product);
                            }
                        }
                    }
                    ViewBag.tongsoluong = TotalProduct();
                    //Tinh tien
                    decimal Money = 0;
                    foreach(var i in productsList)
                    {
                        ProductModel Product = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                        if(Product.Quantity >= i.Quantity && Product.status == true)
                        {
                            Money =Money + i.Price * i.Quantity;
                        }
                    }
                    ViewBag.Total = Money;
                    total = ViewBag.Total;
                    ViewBag.prCart = productsList;
                }
            }
            else
            {
                BagModel Bag = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                if (Bag != null)
                {
                    foreach (var item in Bag.ListCart)
                    {
                        foreach (var i in model)
                        {
                            if (i.ProductID == item.ProductID)
                            {
                                ProductModel product = new ProductModel();
                                product.ProductID = i.ProductID;
                                product.ProductName = i.ProductName;
                                product.img = i.img;
                                if (i.discount != 0)
                                {
                                    product.Price =i.Price - i.Price * i.discount / 100;
                                }
                                else
                                {
                                    product.Price = i.Price;
                                }
                                product.Quantity = item.Quantity;
                                productsList.Add(product);
                            }
                        }
                    }
                    ViewBag.tongsoluong = TotalProduct();
                    decimal tl = 0;
                    foreach(var i in productsList)
                    {
                        ProductModel Product = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                        if(Product.Quantity >= i.Quantity && Product.status == true)
                        {
                            tl = tl + i.Price * i.Quantity;
                        }
                     
                    }
                    ViewBag.Total = tl;
                    total = ViewBag.Total;
                    ViewBag.prCart = productsList;
                    ViewBag.lenghtPrCart = productsList.Count();
                }

            }
            return View();
        }
        public ActionResult Buy(string id, string strUrl)
        {

            var model = GetProducts.Find
            (FilterDefinition<ProductModel>.Empty).ToList();
            CartModel cart = new CartModel();
            foreach (var I in model)
            {
              
                if (I.ProductID == id)
                {
                    cart.ProductID = I.ProductID;
                    cart.Quantity = 1;
                }

            }
            UserModel User = (UserModel)Session["user"];
            if (User == null)
            {
                if (Session["cart"] == null)
                {
                    List<CartModel> cartList = new List<CartModel>();
                    cartList.Add(cart);
                    Session["cart"] = cartList;
                }
                else
                {
                    cart.Quantity = 1;
                    List<CartModel> cartList = (List<CartModel>)Session["cart"];
                    int index = isExist(id);
                    if (index != -1)
                    {
                        cartList[index].Quantity++;
                    }
                    else
                    {
                        cartList.Add(cart);
                    }
                    Session["cart"] = cartList;
                }
            }
            else
            {
                BagModel BagOfUser = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                CartModel item = new CartModel();
                item.ProductID = id;
                item.UserID = User.Id.ToString();
                item.Quantity = 1;
                /// nếu có thì ktra sản phẩm có trong giỏ hàng trong database chưa
                if (BagOfUser != null)
                {
                    
                        foreach (var i in BagOfUser.ListCart)
                        {
                            if (item.ProductID == i.ProductID)
                            {
                                ///nếu rồi thì cộng số lượng lên
                                var a = i.Quantity + 1;
                                
                                var filter = Builders<BagModel>.Filter.And(
                                    Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString()),
                                    Builders<BagModel>.Filter.ElemMatch(x => x.ListCart, x => x.ProductID == item.ProductID));
                                var update = Builders<BagModel>.Update.Set(x => x.ListCart[-1].Quantity, a);
                                a = 0;
                                GetBagOfUser.UpdateOne(filter, update);
                            break;
                            }
                        }
                        var Bag2 = BagOfUser.ListCart.Find(n => n.ProductID == item.ProductID);
                        if (Bag2 == null)
                        {
                           
                            var filter = Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString());
                            GetBagOfUser.UpdateOne(filter, Builders<BagModel>.Update.AddToSet(u => u.ListCart, item));
                           
                       }
                    
                }
                else
                {
                    BagModel bag = new BagModel();
                    bag.UserID = User.Id.ToString();
                    bag.ListCart = new List<CartModel>();
                    bag.ListCart.Add(item);
                    GetBagOfUser.InsertOne(bag);
                }
            }
            return Redirect(strUrl);
        }
        public ActionResult Remove(string id)
        {
            UserModel User = (UserModel)Session["user"];
            if (User == null)
            {
                List<CartModel> cartList = (List<CartModel>)Session["cart"];
                int index = isExist(id);
                cartList.RemoveAt(index);
                Session["cart"] = cartList;
                if (TotalProduct() == 0)
                {
                    Session["cart"] = null;
                }
            }
            else
            {
                BagModel BagOfUser = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                CartModel item = new CartModel();
                item.ProductID = id;


                foreach (var i in BagOfUser.ListCart)
                {
                    if (item.ProductID == i.ProductID)
                    {

                        ////
                        ///

                        if (BagOfUser.ListCart.Count() > 1)
                        {

                            ///lấy các phần tử trong giỏ hàng ra
                            List<CartModel> NewList = new List<CartModel>(BagOfUser.ListCart);
                            NewList.RemoveAll(x => x.ProductID == id);

                            var filter = Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString());
                            var update = Builders<BagModel>.Update.Set(u => u.ListCart, NewList);
                            GetBagOfUser.UpdateOne(filter, update);
                            break;

                        }
                        else
                        {
                            var filter = Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString());
                            GetBagOfUser.DeleteOne(filter);
                            break;
                        }

                    }
                }
            }
            return RedirectToAction("Cart");
        }
        public ActionResult Add(string id)
        {
            UserModel User = (UserModel)Session["user"];
            if (User == null)
            {
                if (Session["cart"] != null)
                {
                    List<CartModel> cartList = (List<CartModel>)Session["cart"];
                    int index = isExist(id);
                    if (index != -1)
                    {
                        cartList[index].Quantity++;
                    }
                    Session["cart"] = cartList;
                }
            }
            else
            {
                BagModel BagOfUser = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                CartModel item = new CartModel();
                item.ProductID = id;
                item.UserID = User.Id.ToString();
                item.Quantity = 1;

                if (BagOfUser != null)
                {

                    foreach (var i in BagOfUser.ListCart)
                    {
                        if (item.ProductID == i.ProductID)
                        {
                            ///nếu rồi thì cộng số lượng lên
                            var a = i.Quantity + 1;

                            var filter = Builders<BagModel>.Filter.And(
                                Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString()),
                                Builders<BagModel>.Filter.ElemMatch(x => x.ListCart, x => x.ProductID == item.ProductID));
                            var update = Builders<BagModel>.Update.Set(x => x.ListCart[-1].Quantity, a);
                            a = 0;
                            GetBagOfUser.UpdateOne(filter, update);
                            break;
                        }
                    }


                }
            }
            return RedirectToAction("Cart");
        }
        public ActionResult Subtract(string id)
        {
            UserModel User = (UserModel)Session["user"];
            if (User == null)
            {
                if (Session["cart"] != null)
                {
                    List<CartModel> cartList = (List<CartModel>)Session["cart"];
                    int index = isExist(id);
                    if (index != -1)
                    {
                        if (cartList[index].Quantity > 1)
                        {
                            cartList[index].Quantity--;
                        }
                        else
                        {
                            Remove(cartList[index].ProductID);
                            return Redirect("Cart");
                        }
                    }
                    Session["cart"] = cartList;
                }
            }
            else
            {
                BagModel BagOfUser = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                CartModel item = new CartModel();
                item.ProductID = id;
                item.UserID = User.Id.ToString();
                item.Quantity = 1;

                if (BagOfUser != null)
                {

                    foreach (var i in BagOfUser.ListCart)
                    {
                        if (item.ProductID == i.ProductID)
                        {
                            ///nếu rồi thì trừ số lượng lên
                            if (i.Quantity > 1)
                            {
                                var a = i.Quantity - 1;

                                var filter = Builders<BagModel>.Filter.And(
                                    Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString()),
                                    Builders<BagModel>.Filter.ElemMatch(x => x.ListCart, x => x.ProductID == item.ProductID));
                                var update = Builders<BagModel>.Update.Set(x => x.ListCart[-1].Quantity, a);
                                a = 0;
                                GetBagOfUser.UpdateOne(filter, update);
                                break;
                            }
                            else if (i.Quantity == 1)
                            {
                                ////
                                ///

                                if (BagOfUser.ListCart.Count() > 1)
                                {

                                    ///lấy các phần tử trong giỏ hàng ra
                                    List<CartModel> NewList = new List<CartModel>(BagOfUser.ListCart);
                                    NewList.RemoveAll(x => x.ProductID == id);

                                    var filter = Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString());
                                    var update = Builders<BagModel>.Update.Set(u => u.ListCart, NewList);
                                    GetBagOfUser.UpdateOne(filter, update);
                                    break;

                                }
                                else
                                {
                                    var filter = Builders<BagModel>.Filter.Eq(x => x.UserID, User.Id.ToString());
                                    GetBagOfUser.DeleteOne(filter);
                                    break;
                                }


                               


                            }
                        }
                    }
                }
            }
            return RedirectToAction("Cart");
        }
        /// <summary>
        /// Trả về số lượng sản phẩm trong giỏ hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public int TotalProduct()
        {
            //kiểm tra xem User đã đăng nhập chưa
            UserModel User = (UserModel)Session["user"];
            if (User == null)
            {
                ///Nếu chưa đăng nhập thì coi trong Session có hàng nào ko
                List<CartModel> cartList = (List<CartModel>)Session["cart"];
               
                if (cartList == null)
                {
                    return 0;
                }
                var TotalQuantity = 0;
                foreach(var i in cartList)
                {
                    ProductModel Product = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                    if(Product.Quantity >= i.Quantity && Product.status == true)
                    {
                        TotalQuantity = TotalQuantity + i.Quantity;
                    }
                }
                return TotalQuantity;
            }
            else
            {
                /// Lấy trong database giỏ hàng của User
                BagModel Bag = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                if (Bag == null)
                {
                    return 0;
                }
                else
                {
                    var a = 0;
                    foreach(var i in Bag.ListCart)
                    {
                        ProductModel Product = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                        if (Product.ProductID == i.ProductID)
                        {
                            if (Product.Quantity >= i.Quantity && Product.status == true)
                                a = a + i.Quantity;

                        }
                    }
                    return a;
                }
            }
        }
        private int isExist(string id)
        {
            List<CartModel> cart = (List<CartModel>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].ProductID.Equals(id))
                    return i;
            return -1;
        }

        public ActionResult Payment()
        {
            UserModel User = (UserModel)Session["user"];
            List<ProductModel> productsList = new List<ProductModel>();
            var model = GetProducts.Find
            (FilterDefinition<ProductModel>.Empty).ToList();
            ViewBag.prDatabase = model;
            if (User == null)
            {
                List<CartModel> cartList = (List<CartModel>)Session["cart"];
                if (cartList != null)
                {
                    foreach (var item in cartList)
                    {
                        foreach (var i in model)
                        {
                            if (i.ProductID == item.ProductID)
                            {
                                ProductModel product = new ProductModel();
                                product.ProductID = i.ProductID;
                                product.ProductName = i.ProductName;
                                product.img = i.img;
                                if (i.discount != 0)
                                {
                                    product.Price = i.Price - i.Price * i.discount / 100;
                                }
                                else
                                {
                                    product.Price = i.Price;
                                }
                                product.Quantity = item.Quantity;
                                productsList.Add(product);
                            }
                        }
                    }

                    if (TotalProduct() == 0)
                    {
                        ViewBag.tongsoluong = 0;
                    }
                    else
                    {
                        ViewBag.tongsoluong = TotalProduct();
                    }
                    decimal Money = 0;
                    foreach (var i in productsList)
                    {
                        ProductModel Product = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                        if (Product.Quantity >= i.Quantity && Product.status == true)
                        {
                            Money = Money + i.Price * i.Quantity;
                        }
                    }
                    ViewBag.Total = Money;
                  
                }
            }
            else
            {
                BagModel Bag = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                if (Bag != null)
                {
                    foreach (var item in Bag.ListCart)
                    {
                        foreach (var i in model)
                        {
                            if (i.ProductID == item.ProductID)
                            {
                                ProductModel product = new ProductModel();
                                product.ProductID = i.ProductID;
                                product.ProductName = i.ProductName;
                                product.img = i.img;
                                if (i.discount != 0)
                                {
                                    product.Price = i.Price - i.Price * i.discount / 100;
                                }
                                else
                                {
                                    product.Price = i.Price;
                                }
                                product.Quantity = item.Quantity;
                                productsList.Add(product);
                            }
                        }
                    }
                    if (TotalProduct() == 0)
                    {
                        ViewBag.tongsoluong = 0;
                    }
                    else
                    {
                        ViewBag.tongsoluong = TotalProduct();
                    }
                    decimal tl = 0;
                    foreach (var i in productsList)
                    {
                        ProductModel Product = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                        if (Product.Quantity >= i.Quantity && Product.status == true)
                        {
                            tl = tl + i.Price * i.Quantity;
                        }
                    }
                    ViewBag.Total = tl;
                    total = ViewBag.Total;
                    ViewBag.prCart = productsList;
                }
            }
            return View();
        }




        public ActionResult AddOrder(OrderModel OrderSession)
        {

            UserModel user = (UserModel)Session["User"];
            OrderModel Order = new OrderModel();
            //kiểm tra user đã đăng nhập chưa
            if (user != null)
            {
                /// Lưu các trường dữ liệu vào  models của order
                /// 
                Order.OrderDate = DateTime.Now.ToString();
                Order.OrderID = ("OrderD_" + DateTime.Now.Ticks).ToString();
                Order.UserID = user.Id.ToString();
                Order.Name = user.Name;
                Order.Email = user.Email;
                Order.Phone = OrderSession.Phone;
                Order.Notes = OrderSession.Notes;
                Order.Address = OrderSession.Address;
                Order.OrderStatus = "Đang Kiểm tra";



                ///List các sản phẩm trong giỏ hàng lưu trong DATABASE \
                ///Goi API model của danh sách sản phẩm tron giỏ hàng ra sau đó add thông tin
                ///hiện tại chưa có database chua add dc
                ///

                decimal tol = 0;


                BagModel Bag = GetBagOfUser.Find(n => n.UserID == user.Id.ToString()).FirstOrDefault();
                List<string> ListRemove = new List<string>();
                foreach (var i in Bag.ListCart)
                {
                    if (i.UserID == user.Id.ToString())
                    {

                        OrderDetailModel Detail = new OrderDetailModel();


                        ProductModel Product = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                        ///check quantity of Product
                        ///Neu so luong sản phẩm trong kho lớn hơn thì mua bình thường neu1 không qua else
                        ///

                        if (Product.Quantity >= i.Quantity && Product.status == true)
                        {
                            Detail.OrderID = Order.OrderID;
                            Detail.ProductID = i.ProductID;
                            Detail.Quantity = i.Quantity;
                            ProductModel oneProduct = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                            if (oneProduct.discount != 0)
                            {
                                Detail.SalePrice = oneProduct.Price * i.Quantity - (oneProduct.Price * i.Quantity) * oneProduct.discount / 100;
                                tol = tol + Detail.SalePrice;
                            }
                            else
                            {
                                Detail.SalePrice = oneProduct.Price * i.Quantity;
                                tol = tol + Detail.SalePrice;
                            }
                            ListRemove.Add(i.ProductID);
                            GetDetail.InsertOne(Detail);
                            var b = Product.Quantity - i.Quantity;
                            var filter = Builders<ProductModel>.Filter.Eq(x => x.ProductID, i.ProductID);
                            var update = Builders<ProductModel>.Update.Set(u => u.Quantity, b);
                            GetProducts.UpdateOne(filter, update);
                        }

                    }
                }
                Order.TotalPrice = tol;
                if (tol != 0) { GetOrder.InsertOne(Order); }


                List<CartModel> NewList = new List<CartModel>(Bag.ListCart);
                foreach (var i in ListRemove)
                {
                    if (Bag.ListCart.Count() == 1 && Bag.ListCart[0].ProductID == i)
                    {
                        GetBagOfUser.DeleteOne(n => n.UserID == user.Id.ToString());
                    }
                    else
                    {
                        NewList.RemoveAll(x => x.ProductID == i);
                        var filter = Builders<BagModel>.Filter.Eq(x => x.UserID, user.Id.ToString());
                        var update = Builders<BagModel>.Update.Set(u => u.ListCart, NewList);
                        GetBagOfUser.UpdateOne(filter, update);
                    }


                }

                /// Lưu các Detail sản phẩm vào trong model detail

            }

            /// nếu user không đăng nhập cho user nhập các ô cần thiết 
            else
            {
                /// Lưu các trường dữ liệu vào  models của order
                Order.OrderDate = DateTime.Now.ToString();
                Order.OrderID = ("OrderD_" + DateTime.Now.Ticks).ToString();
                Order.Name = OrderSession.Name;
                Order.Email = OrderSession.Email;
                Order.Notes = OrderSession.Notes;
                Order.Phone = OrderSession.Phone;
                Order.Address = OrderSession.Address;
                Order.OrderStatus = "Đang Kiểm tra";

                decimal tol = 0;
                ///List các sản phẩm trong giỏ hàng lưu trong Session khi chua đang nhập
                List<CartModel> cart = (List<CartModel>)Session["cart"];
                List<string> ListRemove = new List<string>();
                /// Lưu detail order vào database
                foreach (var i in cart)
                {
                    OrderDetailModel Detail = new OrderDetailModel();

                    ProductModel oneProduct = GetProducts.Find(n => n.ProductID == i.ProductID).FirstOrDefault();
                    if (oneProduct.Quantity >= i.Quantity && oneProduct.status == true)
                    {
                        Detail.OrderID = Order.OrderID;
                        Detail.ProductID = i.ProductID;
                        Detail.Quantity = i.Quantity;
                        if (oneProduct.discount != 0)
                        {
                            Detail.SalePrice = oneProduct.Price * i.Quantity - (oneProduct.Price * i.Quantity) * oneProduct.discount / 100;
                            tol = tol + Detail.SalePrice;
                        }
                        else
                        {
                            Detail.SalePrice = oneProduct.Price * i.Quantity;
                            tol = tol + Detail.SalePrice;
                        }



                        ListRemove.Add(i.ProductID);
                        GetDetail.InsertOne(Detail);
                        var a = oneProduct.Quantity - i.Quantity;
                        var filter = Builders<ProductModel>.Filter.Eq(x => x.ProductID, i.ProductID);
                        var update = Builders<ProductModel>.Update.Set(u => u.Quantity, a);
                        GetProducts.UpdateOne(filter, update);
                    }


                }
                Order.TotalPrice = tol;
                if (tol != 0) { GetOrder.InsertOne(Order); }
                List<CartModel> NewList = new List<CartModel>(cart);
                foreach (var i in ListRemove)
                {
                    if (cart.Count() == 1 && cart[0].ProductID == i)
                    {
                        Session["cart"] = null;
                    }
                    else
                    {
                        NewList.RemoveAll(x => x.ProductID == i);

                        Session["cart"] = NewList;
                    }


                }

            }
            return RedirectToAction("Success", "Cart");
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult CartPartial()
        {
            var TotalQuantity = 0;
            //kiểm tra xem User đã đăng nhập chưa
            UserModel User = (UserModel)Session["user"];
            if (User == null)
            {
                ///Nếu chưa đăng nhập thì coi trong Session có hàng nào ko
                List<CartModel> cartList = (List<CartModel>)Session["cart"];
                if (cartList == null)
                {
                    TotalQuantity = 0;
                }
                else
                {
                    TotalQuantity = cartList.Sum(i => i.Quantity);
                }
            }
            else
            {
                /// Lấy trong database giỏ hàng của User
                BagModel Bag = GetBagOfUser.Find(n => n.UserID == User.Id.ToString()).FirstOrDefault();
                if (Bag == null)
                {
                    TotalQuantity = 0;
                }
                else
                {
                    var a = 0;
                    foreach (var i in Bag.ListCart)
                    {
                        a = a + i.Quantity;
                    }
                    TotalQuantity = a;
                }
            }

                ViewBag.tongsoluong = TotalQuantity;
                return PartialView();
           
        }
    }
}