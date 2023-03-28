using DOAN.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        MyDataContextDB data = new MyDataContextDB();
        // GET: Admin/HomeAdmin
        public ActionResult Index(int? page, string SearchString)
        {
            int pageSize = 8;
            int pageNum = page ?? 1;
            var SearchAll = data.SanPham.OrderBy(s => s.TenSP);
            var SearchSp = data.SanPham.OrderBy(m => m.TenSP).Where(sp => sp.TenSP.ToUpper().Contains(SearchString.ToUpper()));
            page = 1;
            if (SearchString == null || SearchString == "")
                return View(SearchAll.ToPagedList(pageNum, pageSize));
            else if (SearchSp != null)
                return View(SearchSp.ToPagedList(pageNum, pageSize));
            else
                return View(SearchAll.ToPagedList(pageNum, pageSize));
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }

        public ActionResult listSP()
        {
            var listGiay = data.SanPham.Where(s => s.MaLoai == 1004).ToList();
            return View(listGiay);
        }


        public ActionResult Detail(int id)
        {
            var D_sp = data.SanPham.Where(m => m.MaSP == id).First();
            return View(D_sp);
        }
        public ActionResult Create()
        {
            ViewBag.Ma_SP = new SelectList(data.LoaiSanPham, "MaLoai", "TenLoai");
            return View();
        }
        [HttpPost]
        public ActionResult Create(SanPham sp)
        {
            MyDataContextDB data = new MyDataContextDB();
            NhanVien kh = (NhanVien)Session["FullTaiKhoan"];
            if (sp.SoLuong <= 0)
            {
                ViewData["WrongNumber"] = "số lượng ko được âm";
                return this.Create();
            } 
            if(sp.GiaBan <=0 )
            {
                ViewData["WrongMoney1"] = "giá tiền ko được âm";
                return this.Create();
            }
             if (sp.GiaNhap <= 0)
            {
                ViewData["WrongMoney"] = "giá nhập ko được âm";
                return this.Create();
            }
            data.SanPham.AddOrUpdate(sp);
            data.SaveChanges();
            return RedirectToAction("Create");
        }

        public ActionResult Delete(int id)
        {
            var D_sach = data.SanPham.First(m => m.MaSP == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_sach = data.SanPham.Where(m => m.MaSP == id).First();
            data.SanPham.Remove(D_sach);
            data.SaveChanges();
            return RedirectToAction("listSP", "HomeAdmin");
        }
        public ActionResult Edit(int id)
        {
            try
            {
                var E_Sp = data.SanPham.First(m => m.MaSP == id);
                return View(E_Sp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection, SanPham sanpham)
        {

            var sp = data.SanPham.First(m => m.MaSP == id);

            var E_tensp = collection["TenSP"];
            if (sanpham.SoLuong <= 0)
            {
                ViewData["WrongNumber"] = "số lượng ko được âm";
                return this.Create();
            }
             if (sanpham.GiaBan <= 0)
            {
                ViewData["WrongMoney1"] = "giá tiền ko được âm";
                return this.Create();
            }
             if (sanpham.GiaNhap <= 0)
            {
                ViewData["WrongMoney"] = "Giá nhập ko được âm";
                return this.Create();
            }
            sp.MaSP = id;
            if (string.IsNullOrEmpty(E_tensp))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                sp.TenSP = E_tensp;
                UpdateModel(sp);
                data.SaveChanges();
                return RedirectToAction("listSP", "HomeAdmin");
            }
            return this.Edit(id);
        }
    }
}