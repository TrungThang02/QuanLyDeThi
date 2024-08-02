using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using PagedList;
using QuanLyDeThi.Models;

namespace QuanLyDeThi.Controllers
{
    [HandleError]

    public class UserController : Controller
    {

        static int idInfo = 0;


        static public int getid()
        {
            return idInfo;
        }
        QuanLyDeThiEntities db = new QuanLyDeThiEntities();
        // GET: User
        public static string Encrypt(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            encrypt = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());

            }
            return encryptdata.ToString();

        }


        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }


        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //var response = Request["g-recaptcha-response"];
            ////string secretKey = "6Let3L0jAAAAAPx3bZRlTYo3gt-0rfbbrmCFIsvk";
            //string secretKey = "6LdPxmAlAAAAAI5URX6z8aUgEKjPUTK5AFn13uuA";
            //var client = new WebClient();

            //var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));

            //var obj = JObject.Parse(result);
            //var status = (bool)obj.SelectToken("success");
            //ViewBag.Message = status ? "Xác thực Google reCaptcha thành công !" : "Bạn chưa xác thực Google reCaptcha";


            if (ModelState.IsValid)
            {
                int state = Convert.ToInt32(Request.QueryString["id"]);
                var sTenDN = collection["TenDN"];
                var sMatKhau = Encrypt(collection["MatKhau"].ToString());

                //var sMatKhau = collection["MatKhau"].ToString();
                if (String.IsNullOrEmpty(sTenDN))
                {
                    ViewData["err"] = "Tài khoản tên không được để trống";
                }
                else if (String.IsNullOrEmpty(sMatKhau))
                {
                    ViewData["err"] = "Mật khẩu không được để trống";
                }
                else
                {
                    Users nguoidung = db.Users.SingleOrDefault(n => n.UserName == sTenDN && n.PassWord == sMatKhau);


                    Users ADMIN = db.Users.SingleOrDefault(n => n.UserName == sTenDN && n.PassWord == sMatKhau && n.Role == 0);

                    if (nguoidung != null)
                    {
                        ViewBag.ThongBao = "Chúc mừng bạn đăng nhập thành công";

                        Session["TaiKhoan"] = nguoidung;

                        Session["TaiKhoan2"] = nguoidung.Name;

                        Session["TaiKhoan3"] = nguoidung.ID;



                        idInfo = nguoidung.ID;


                        if (nguoidung == ADMIN)
                        {
                            Session.Clear();

                            Session["ADMIN"] = ADMIN;
                            return RedirectToAction("Index", "Admin/Homes", "Areas");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");

                        }


                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                        //ViewBag.Message = status ? "Xác thực Google reCaptcha thành công !" : "Bạn chưa xác thực Google reCaptcha";

                    }
                }
            }


            return View();
        }


        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection collection, Users nguoidung)
        {
            //var response = Request["g-recaptcha-response"];
            ////string secretKey = "6Let3L0jAAAAAPx3bZRlTYo3gt-0rfbbrmCFIsvk";
            //string secretKey = "6LdPxmAlAAAAAI5URX6z8aUgEKjPUTK5AFn13uuA";
            //var client = new WebClient();

            //var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));

            //var obj = JObject.Parse(result);
            //var status = (bool)obj.SelectToken("success");
            //ViewBag.Message = status ? "Xác thực Google reCaptcha thành công !" : "Bạn chưa xác thực Google reCaptcha";


            if (ModelState.IsValid)
            {
                var sTen = collection["Ten"];
                var sTenDN = collection["TenDN"];
                var sEmail = collection["Email"];
                var sMatKhau = Encrypt(collection["MatKhau"].ToString());
                var sNhapLaiMatKhau = collection["MatKhauNL"];

                if (String.IsNullOrEmpty(sTenDN))
                {
                    ViewData["err2"] = "Tên đăng nhập không được để trống";
                }
                else if (String.IsNullOrEmpty(sEmail))
                {
                    ViewData["err6"] = "Email không được để trống";
                }
                else if (String.IsNullOrEmpty(sMatKhau))
                {
                    ViewData["err3"] = "Mật khẩu không được để trống";
                }
                else if (String.IsNullOrEmpty(sNhapLaiMatKhau))
                {
                    ViewData["err4"] = "Phải nhập lại mật khẩu";
                }


                else if (db.Users.SingleOrDefault(n => n.UserName == sTenDN) != null)
                {
                    ViewBag.user = "Tên đăng nhập đã tồn tại";
                }
                else if (db.Users.SingleOrDefault(n => n.Email == sEmail) != null)
                {
                    ViewBag.mail = "Email đã được sử dụng";
                }
                else
                {
                    try
                    {
                        nguoidung.Name = sTen;
                        nguoidung.UserName = sTenDN;
                        nguoidung.PassWord = sMatKhau;
                        nguoidung.Email = sEmail;

                        db.Users.Add(nguoidung);
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        Console.WriteLine(e);
                    }
                    return RedirectToAction("Login", "User");
                }

            }
            else
            {
                ViewBag.ThongBao = "Đăng ký không thành công, vui lòng kiểm tra lại thông tin";
                //ViewBag.Message = status ? "Xác thực Google reCaptcha thành công !" : "Bạn chưa xác thực Google reCaptcha";

            }
            return this.SignUp();

        }

        public ActionResult LoginLogOutPartial()
        {

            return PartialView();
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult QuanTriNguoiDung(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("trantrungthang01699516993@gmail.com", "Hệ thống quản lí đề tài báo cáo tốt nghiệp -  TDMU");
            var toEmail = new MailAddress(email);

            string subject = "";
            string body = "";

            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created";
                body = "<br /><br /> We are  <a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "KHÔI PHỤC MẬT KHẨU";
                body = "Chúng tôi xác nhận tài khoản này thuộc về bạn, vui lòng ấn vào <a href='" + link + "'> Khôi phục mật khẩu </a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, "iauopfthcsrhnunj")
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);



        }

        [HttpPost]
        public ActionResult SendEmail(FormCollection form)
        {
            if (Session == null)
            {
                ViewBag.ERR = "Chưa đăng nhập";
            }
            else
            {



                try
                {
                    string to = "trantrungthang01699516993@gmail.com";
                    //string from = Session["Email"].ToString();
                    string from = "thangpy2k2@gmail.com";

                    string subject = "ĐÓNG GÓP Ý KIẾN"; // chủ đề email
                    string mes = form["message"]; // nội dung email

                    // Khởi tạo đối tượng MailMessage
                    // Khởi tạo đối tượng SmtpClient và gửi email
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // SMTP server và cổng của email
                    smtpClient.Credentials = new NetworkCredential("trantrungthang01699516993@gmail.com", "iauopfthcsrhnunj"); // địa chỉ email và mật khẩu
                    smtpClient.EnableSsl = true; // kích hoạt SSL cho kết nối SMTP



                    using (var message = new MailMessage(from, to)
                    {
                        Subject = subject,
                        Body = mes,
                        IsBodyHtml = true
                    })
                        smtpClient.Send(message);
                }
                catch (Exception ex)
                {

                }
            }

            return RedirectToAction("Index", "Documents");
        }



        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            string message = "";
            bool status = false;
            var account = db.Users.Where(a => a.Email == Email).FirstOrDefault();
            if (account != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                account.ResetPasswordCode = resetCode;
                db.SaveChanges();
                message = "Liên kết khôi phục mật khẩu đã được gửi đến email của bạn <3";
            }
            else
            {
                message = "Không tìm thấy email";
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            var user = db.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPassword model = new ResetPassword();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.PassWord = Encrypt(model.NewPassword);
                    user.ResetPasswordCode = "";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Thay đổi mật khẩu thành công";

                }
                return RedirectToAction("Login", "User");
            }
            else
            {
                message = "Lỗi !";
            }
            ViewBag.Message = message;
            return View(model);
        }


        public ActionResult Info(int? id, int? page)
        {
            int iSize = 12;
            int iPageNum = (page ?? 1);

            if (idInfo != id)
            {
                return RedirectToAction("PageNotFound", "Error");

            }

            try
            {
                int totalViews = (int)db.Exam.Where(r => r.ID == id).Sum(r => r.ViewCount);
                ViewBag.totalView = totalViews;

                int totalDowns = (int)db.Exam.Where(r => r.ID == id).Sum(r => r.DownloadCount);
                ViewBag.totalDown = totalDowns;

                int totalReport = db.Exam.Where(r => r.ID == id).Count();
                ViewBag.totalReport = totalReport;

                int pending = db.Exam.Where(r => r.ID == id).Where(r => r.Status == 0).Count();
                ViewBag.pending = pending;

            }
            catch (Exception)
            {

            }



            var bc = from s in db.Exam where s.ID == id select s;
            return View(bc.Where(r => r.Status == 1).OrderBy(s => s.ExamID).ToPagedList(iPageNum, iSize).ToList());
        }


        public ActionResult Pending(int? id)
        {

            if (idInfo != id)
            {
                return RedirectToAction("PageNotFound", "Error");

            }

            try
            {

                int pending = db.Exam.Where(r => r.ID == id).Where(r => r.Status == 0).Count();
                ViewBag.pending = pending;

            }
            catch (Exception)
            {

            }



            var bc = from s in db.Exam where s.ID == id select s;
            return View(bc.Where(r => r.Status == 0).ToList());
        }



        public ActionResult Thaydoithongtin(int? id)
        {
            //
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (idInfo != id)
            {
                return RedirectToAction("PageNotFound", "Error");

            }
          
            return View(users);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Thaydoithongtin([Bind(Include = "ID,Name,Age,Address,UserName,Email,Phone,Password")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                Session.Remove("TaiKhoan2");
                Session["TaiKhoan2"] = users.Name;
                return RedirectToAction("Index", "Home");
            }
           
            return View(users);
        }


    }
}