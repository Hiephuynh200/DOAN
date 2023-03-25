using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Areas.Admin.Controllers
{
    public class LoginAdminController : Controller
    {
        // GET: Admin/LoginAdmin
        MyDataContextDB data = new MyDataContextDB();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendangnhap = collection["Email"];
            var matkhau = collection["Pass"];
            NhanVien nv = data.NhanVien.FirstOrDefault(x => x.Email == tendangnhap && x.Pass == matkhau);
            //KhachHang khachhang = data.KhachHang.FirstOrDefault(x => x.Email == tendangnhap && x.PassKH == matkhau);
            if (nv != null)
            {
                ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                Session["User"] = nv.TenNV;
                Session["CHucVU"] = nv.ChucVu.MaCV;
                Session["TaiKhoan"] = nv.MaNV;
                Session["User"] = nv.User;
                Session["Account"] = nv.MaCV;
                Session["FullTaiKhoan"] = nv;

            }
            else if (nv == null)
            {
                ViewData["ErrorAccount"] = "sai mật khẩu hoặc Tên đăng nhập không tồn tại vui lòng nhập lại";
                return this.DangNhap();
            }
            else
            {
                ViewData["ErrorPass"] = "Mật khẩu không đúng";
                return this.DangNhap();
            }
            return RedirectToAction("Index", "HomeAdmin");
        }

        [HttpGet]
        public ActionResult DangKyAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKyAdmin(FormCollection collection, NhanVien tk)
        {
            var hoten = collection["TenNV"];
            var dienthoai = collection["SDT"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var User = collection["User"];
            var Pass = collection["Pass"];
            var MatKhauXacNhan = collection["MatKhauXacNhan"];
            var chucvu = int.Parse(collection["MaCV"]);
            string temp = dienthoai;
            char check = temp[0];
            Regex regexMail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match matchMail = regexMail.Match(email);
            Regex regexPhone = new Regex(@"^(84|0[3|5|7|8|9])+([0-9]{8})\b");
            Match matchPhone = regexPhone.Match(dienthoai);
            Regex regexPass = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*?[0-9])(?=.*?[@!#%])[A-Za-z0-9!#%]{8,32}$");
            Match matchPassword = regexPass.Match(Pass);
            var checkEmail = data.NhanVien.FirstOrDefault(x => x.Email == email);
            if (checkEmail != null)
            {
                ViewData["UserExist"] = "email đăng nhập đã tồn tại";
                return this.DangKyAdmin();
            }
            if (temp[0] != 0 && !matchPhone.Success)
            {
                ViewData["NumWrong"] = "số điện thoải phải đúng định dạng";
                return this.DangKyAdmin();
            }
            if (!matchMail.Success)
            {
                ViewData["EmailWrong"] = "Email phải đúng định dạng";
                return this.DangKyAdmin();
            }
            if (!matchPassword.Success)
            {
                ViewData["PassWordWrong"] = "phải có chữ viết hoa, viết thường, ký tự đặc biệt, phải có số, nhập đủ 8 đến 32 ký tự";
                return this.DangKyAdmin();
            }
            if (String.IsNullOrEmpty(MatKhauXacNhan))
            {
                ViewData["NhapXNMK"] = "Phải nhập mật khẩu xác nhận!";
            }
            else
            {
                if (!Pass.Equals(MatKhauXacNhan))
                {
                    ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                }
                else
                {
                    tk.TenNV = hoten;
                    tk.SDT = dienthoai;
                    tk.DiaChi = diachi;
                    tk.Email = email;
                    tk.SDT = dienthoai;
                    tk.MaCV = chucvu;

                    data.NhanVien.Add(tk);
                    data.SaveChanges();

                    return RedirectToAction("DangNhap");
                }
            }
            return this.DangKyAdmin();
        }

        public ActionResult LstNhanVien()
        {
            var listNv = from ss in data.NhanVien select ss;
            return View(listNv);
        }

        public ActionResult Detail(int id)
        {
            var listNv = data.NhanVien.Where(m => m.MaNV == id).First(); 
            return View(listNv);
        }
        public ActionResult Edit(int id)
        {
            var listNv = data.NhanVien.First(m => m.MaNV == id);
            return View(listNv);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var ENv = data.NhanVien.First(m => m.MaNV == id);
            var E_tenNV = collection["TenNV"];
            var E_SDT = collection["SDT"];
            var E_DiaChi = collection["DiaChi"];
            var E_Email = collection["Email"];
            var User = collection["User"];
            var Pass = collection["Pass"];
            ENv.MaNV = id;
            if (string.IsNullOrEmpty(E_tenNV))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                ENv.TenNV = E_tenNV;
                ENv.SDT = E_SDT;
                ENv.DiaChi = E_DiaChi;
                ENv.Email = E_Email;
                ENv.User = User;
                ENv.Pass = Pass;
                UpdateModel(ENv);
                data.SaveChanges();
                return RedirectToAction("LstNhanVien");
            }
            return this.Edit(id);
        }
        public ActionResult Delete(int id)
        {
            var listNv = data.NhanVien.First(m => m.MaNV == id);
            return View(listNv);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var listNv = data.NhanVien.Where(m => m.MaNV == id).First(); 
            data.NhanVien.Remove(listNv);
            data.SaveChanges();
            return RedirectToAction("LstNhanVien");
        }
        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("DangNhap", "LoginAdmin");
        }
    }
}