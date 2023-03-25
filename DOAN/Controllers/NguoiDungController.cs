using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        // GET: NguoiDung
        // GET: Admin/User
        MyDataContextDB db = new MyDataContextDB();
        public int OTP = 0;
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult SaveData(KhachHang kh)
        {
            string result = "NotExist";
            string adminMail = "huynhhiepvan1998@gmail.com";
            KhachHang checkmail = db.KhachHang.Where(x => x.Email == kh.Email).FirstOrDefault();
            if (checkmail == null)
            {
                kh.IsValid = false;
                db.KhachHang.Add(kh);
                BuildEmailTemplate(kh.MaKH);
            }
            else if (checkmail.Email != null)
            {
                result = "Exits";
            }
            else if (kh.Email == adminMail)
            {
                result = "notAdmin";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult XacTHuc(int idKH)
        {
            ViewBag.idKH = idKH;
            return View();
        }

        public void BuildEmailTemplate(int idKH)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("/EmailTemplate/") + "Text" + ".cshtml");
            var regInfo = db.KhachHang.Where(x => x.MaKH == idKH).FirstOrDefault();
            var url = "https://localhost:44343/" + "Admin/LoginAdmin/DangKyAdmin?idKH" + idKH;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Tai khoan cua ban duoc tao thanh cong", body, regInfo.Email);
        }
        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "huynhhiepvan1998@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendMail(mail);
        }

        private static void SendMail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("huynhhiepvan1998@gmail.com", "");
            try
            {
                client.Send(mail);
            }
            catch (Exception ex) { throw ex; }
        }

        public JsonResult kiemTraTk(KhachHang model)
        {
            string result = "Fail";
            var DataItem = db.KhachHang.Where(x => x.Email == model.Email && x.PassKH == model.PassKH).FirstOrDefault();
            if (DataItem != null)
            {
                if (DataItem.Email == model.Email && DataItem.IsValid == true)
                {
                    Session["UserId"] = DataItem.MaKH.ToString();
                    Session["UserName"] = DataItem.TenKH.ToString();
                    result = "Success";
                }
                var isValid = DataItem.IsValid;
                if (isValid == false)
                {
                    result = "notValid";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AfterLogin()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        public JsonResult SendOTP(KhachHang kh)
        {
            bool valid = false;
            KhachHang checkEmail = db.KhachHang.SingleOrDefault(x => x.Email == kh.Email);
            if (checkEmail != null)
            {
                if (checkEmail.IsValid == false)
                {
                    return Json(valid, JsonRequestBehavior.AllowGet);
                }
                valid = true;
                Random random = new Random();
                OTP = random.Next(100000, 1000000);
                Session["OTP"] = OTP.ToString();
                var fromAddress = new MailAddress("huynhhiepvan1998@gmail.com");
                var toAddress = new MailAddress(kh.Email);
                const string fromPass = "gnugexpocjwbxvcu";
                const string subject = "OTP code";
                string body = OTP.ToString();
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPass),
                    Timeout = 20000
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                })
                {
                    smtp.Send(message);
                }
            }
            return Json(valid, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ResetPass(KhachHang kh, FormCollection collection)
        {
            var otp = collection["OtP"];
            string kq = "NotValid";
            var checkMail = db.KhachHang.SingleOrDefault(x => x.Email == kh.Email);
            if (Session["OTP"] == null)
            {
                kq = "WrongOTP";
            }
            else if (checkMail != null && otp == Session["OTP"].ToString())
            {
                kq = "Valid";
                checkMail.PassKH = kh.PassKH;
                db.SaveChanges();
                Session.Clear();
                Session.Abandon();
            }
            else if (otp != Session["OTP"].ToString())
            {
                kq = "WrongOTP";
            }
            return Json(kq, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["TenKH"];
            var dienthoai = collection["SDT"].ToString();
            var email = collection["Email"];
            var diachi = collection["DiaChi"];
            var tendangnhap = collection["UserKH"];
            var matkhau = collection["PassKH"];
            var MatKhauXacNhan = collection["MatKhauXacNhan"];
            string temp = dienthoai;
            char check = temp[0];
            Regex regexMail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match matchMail = regexMail.Match(email);
            Regex regexPhone = new Regex(@"^(84|0[3|5|7|8|9])+([0-9]{8})\b");
            Match matchPhone = regexPhone.Match(dienthoai);
            Regex regexPass = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*?[0-9])(?=.*?[@!#%])[A-Za-z0-9!#%]{8,32}$");
            Match matchPassword = regexPass.Match(matkhau);
            var checkEmail = db.KhachHang.FirstOrDefault(x => x.Email == email);
            if (checkEmail != null)
            {
                ViewData["UserExist"] = "email đã tồn tại, vui long nha email khac";
                return this.DangKy();
            }
            if ((int)check != 0 && !matchPhone.Success)
            {
                ViewData["NumWrong"] = "Num phải đúng định dạng";
                return this.DangKy();
            }
            if (!matchMail.Success)
            {
                ViewData["EmailWrong"] = "Email phải đúng định dạng";
                return this.DangKy();
            }
            if (!matchPassword.Success)
            {
                ViewData["PassWrong"] = "phải có chữ viết hoa, viết thường, ký tự đặc biệt, phải có số, nhập đủ 8 đến 32 ký tự";
                return this.DangKy();
            }
            if (String.IsNullOrEmpty(MatKhauXacNhan))
            {
                ViewData["NhapMKXN"] = "Phải nhập mật khẩu xác nhận";
            }
            else
            {
                if (!matkhau.Equals(MatKhauXacNhan))
                {
                    ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận khong giong nhau";
                }
                else
                {
                    kh.TenKH = hoten;
                    kh.SDT = dienthoai;
                    kh.Email = email;
                    kh.DiaChi = diachi;
                    kh.UserKH = tendangnhap;
                    kh.PassKH = matkhau;
                    db.KhachHang.Add(kh);
                    db.SaveChanges();
                    return RedirectToAction("DangNhap", "NguoiDung");
                }
            }
            return RedirectToAction("DangNhap", "NguoiDung");
        }
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendangnhap = collection["Email"];
            var matkhau = collection["PassKH"];
            KhachHang khachhang = db.KhachHang.FirstOrDefault(x => x.Email == tendangnhap  && x.PassKH == matkhau);
            if (khachhang != null)
            {
                ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                Session["User"] = khachhang.TenKH;
                Session["TaiKhoanKH"] = khachhang.MaKH;
                Session["User"] = khachhang.UserKH;
                Session["FullTaiKhoan"] = khachhang;

            }
            else if (khachhang == null)
            {
                ViewData["ErrorAccount"] = "sai mật khẩu hoặc Tên đăng nhập không tồn tại vui lòng nhập lại";
                return this.DangNhap();
            }
            else
            {
                ViewData["ErrorPass"] = "Mật khẩu không đúng";
                return this.DangNhap();
            }
            return RedirectToAction("Index", "MainPage");
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("DangNhap", "NguoiDung");
        }
    }
}