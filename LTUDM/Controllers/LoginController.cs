using LTUDM.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LTUDM.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private IMongoCollection<UserModel> GetUserModel;
        private IMongoCollection<BagModel> GetCartModel;
        public LoginController()
        {
            var client = new MongoClient("mongodb+srv://Pharmacy:010558@cluster0-merwd.mongodb.net/Phamarcy?retryWrites=true&w=majority");
            IMongoDatabase db = client.GetDatabase("Phamarcy");
            this.GetUserModel = db.GetCollection<UserModel>("User");
            this.GetCartModel = db.GetCollection<BagModel>("Cart");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collections)
        {
            string account = collections["email"].ToString();
            string password = collections["password"].ToString();
            UserModel isCompare = GetUserModel.Find(a => a.Email == account).FirstOrDefault();
            if (isCompare != null)
            {
                if (!isCompare.IsEmailVerfied)
                {
                    ViewBag.MessageLogin = "Vui lòng xác thực tài khoản của bạn";
                    return View();
                }
                if (isCompare.Password == Crypt.Hash(password))
                {
                   
                    Session["user"] = isCompare;

                    List<CartModel> cart = (List<CartModel>)Session["cart"];
                    if(cart == null) { return RedirectToAction("Index", "Home"); }
                    else
                    {
                        var model = GetCartModel.Find
                        (FilterDefinition<BagModel>.Empty).ToList();
                        //// danh sách san phẩm trong giỏ hàng của session khi chua đang nhập


                        var check = 1;
                            /// tìm xem user này có giỏ hàng trong Database ko
                            BagModel BagOfUser = GetCartModel.Find(n => n.UserID == isCompare.Id.ToString()).FirstOrDefault();
                            /// nếu có thì ktra sản phẩm có trong giỏ hàng trong database chưa
                            if (BagOfUser != null)
                            {
                                foreach (var item in cart)
                                {
                                    foreach (var i in BagOfUser.ListCart)
                                    {
                                        if (item.ProductID == i.ProductID)
                                        {
                                            ///nếu rồi thì cộng số lượng lên
                                            var a = i.Quantity + item.Quantity;
                                            check = 0;
                                            var filter = Builders<BagModel>.Filter.And(
                                                Builders<BagModel>.Filter.Eq(x=>x.UserID, isCompare.Id.ToString()),
                                                Builders<BagModel>.Filter.ElemMatch(x => x.ListCart, x => x.ProductID == item.ProductID));
                                            var update = Builders<BagModel>.Update.Set(x=>x.ListCart[-1].Quantity, a);
                                            a = 0;
                                            GetCartModel.UpdateOne(filter, update);
                                        }
                                    }
                                var Bag2 = BagOfUser.ListCart.Find(n => n.ProductID == item.ProductID);
                                    if (Bag2 == null)
                                    {
                                    item.UserID = isCompare.Id.ToString();
                                       var filter = Builders<BagModel>.Filter.Eq(x => x.UserID, isCompare.Id.ToString());
                                    GetCartModel.UpdateOne(filter, Builders<BagModel>.Update.AddToSet(u => u.ListCart, item));
                                        check = 1;
                                    }
                                }
                            }
                            else
                            {
                                BagModel bag = new BagModel();
                                bag.UserID = isCompare.Id.ToString();
                                foreach(var i in cart)
                                {
                                    i.UserID = isCompare.Id.ToString();
                                }
                                bag.ListCart = cart;
                                
                                GetCartModel.InsertOne(bag);
                            }


                            /// kiểm tra User có giỏ hàng trong database chưa
                           
                      

                        
                        
                    }
                    Session["cart"]=null;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.MessageLogin = "Mật khẩu không đúng";
                }

            }
            else
            {
                ViewBag.MessageLogin = "Tài khoản hoặc mật khẩu không đúng";
            }
            
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            UserModel user = new UserModel();
            bool Status = false;
            string message = "";
            if (ModelState.IsValid)
            {
                var isEmailExist = GetUserModel.Find(a => a.Email == collection["email"].ToString()).FirstOrDefault();
                if (isEmailExist != null)
                {
                    message = "Email đã tồn tại";
                    ViewBag.Message = message;
                    return View(user);
                }
                user.Email = collection["email"].ToString();
                user.Img = null;
                user.Name = collection["name"].ToString();
                user.Phone = collection["phone"].ToString();
                user.Direct = null;
                user.Password = Crypt.Hash(collection["password"].ToString());
                user.ActivationCode = Guid.NewGuid();
                user.IsEmailVerfied = false;
                GetUserModel.InsertOneAsync(user);
                message = "Tạo tài khoản thành công";
                SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());
                message = "Tạo tài khoản thành công. Liên kết kích hoạt tài khoản " +
                    " đã được gửi tới email của bạn:" + user.Email;
                Status = true;
            }
            else
            {
                message = "Đã xảy ra lỗi";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/Login/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("mailxacnhan99@gmail.com", "Tạo tài khoản");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Xacnhan99";
            string subject = "Tài khoản của bạn đã được tạo thành công!";

            string body = "<br/><br/>Tài khoản của bạn " +
                " đã được tạo thành công. Vui lòng nhấn vào liên kết bên dưới để xác thực tài khoản của bạn" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
                var v = GetUserModel.Find(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                v.IsEmailVerfied = true;
                var filter = Builders<UserModel>.Filter.Eq("Email", v.Email);
                var update = Builders<UserModel>.Update.Set("IsEmailVerfied", v.IsEmailVerfied);
                GetUserModel.UpdateOne(filter, update);
                Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            ViewBag.Status = Status;
            return View();
        }
        public ActionResult Forgett()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgett(FormCollection collection)
        {
            string message = "";
            string email = collection["email"].ToString();
            UserModel user = GetUserModel.Find(a => a.Email == email).FirstOrDefault();
            if(user == null)
            {
                message = "Tài khoản của bạn không tồn tại";
            }
            else
            {
                ResetPassword(email, user.ActivationCode.ToString());
                message = "Vui lòng kiểm tra mail để đổi lại mật khẩu";
            }
            ViewBag.Message = message;
            return View();
        }
        [NonAction]
        public void ResetPassword(string emailID, string ActiveCode)
        {
            var verifyUrl = "/Login/Reset/" + ActiveCode ;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("mailxacnhan99@gmail.com", "Đổi mật khẩu");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Xacnhan99";
            string subject = "Thay đổi mật khẩu!";

            string body = "<br/><br/>Tài khoản của bạn " +
                "đã được yêu cầu đổi lại mật khẩu. Vui lòng nhấn vào liên kết bên dưới để đổi mật khẩu" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        [HttpGet]
        public ActionResult Reset(string id)
        {
            bool status = false;
            var user = GetUserModel.Find(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
            if(user != null)
            {
                ViewBag.User = user.Email;
                status = true;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Reset(FormCollection collection, string email)
        {
            string message = "";
            string newPass = collection["password"].ToString();
            string reNewPass = collection["repassword"].ToString();
            if (newPass != reNewPass)
            {
                message = "Mật khẩu mới không khớp";
                ViewBag.Message = message;
            }
            else
            {
                var v = GetUserModel.Find(a => a.Email == email).FirstOrDefault();
                if (v != null)
                {
                    v.Password = Crypt.Hash(newPass);
                    var filter = Builders<UserModel>.Filter.Eq("Email", v.Email);
                    var update = Builders<UserModel>.Update.Set("Password", v.Password);
                    GetUserModel.UpdateOne(filter, update);
                    message = "Thay đổi mật khẩu thành công";
                }
                else
                {
                    message = "Đã có lỗi xảy ra";
                }
            }
            ViewBag.Message = message;
            return View();
        }
    }
}