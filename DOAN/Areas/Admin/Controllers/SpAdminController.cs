using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Areas.Admin.Controllers
{
    public class SpAdminController : Controller
    {
        // GET: Admin/SpAdmin
        // GET: Admin/SpAdmin
        MyDataContextDB data = new MyDataContextDB();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SpAo()
        {
            var listAo = data.SanPham.Where(x => x.MaLoai == 1001).ToList();
            return View(listAo);
        }
        public ActionResult Detail(int id)
        {
            var D_sp = data.SanPham.Where(m => m.MaSP == id).First();
            return View(D_sp);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(SanPham sp)
        {
            MyDataContextDB data = new MyDataContextDB();
            if (sp.SoLuong <= 0)
            {
                ViewData["WrongNumber"] = "số lượng ko được âm";
                return this.Create();
            }
            else if (sp.GiaBan <= 0)
            {
                ViewData["WrongMoney1"] = "giá bán ko được âm";
                return this.Create();
            }
            else if (sp.GiaNhap <= 0)
            {
                ViewData["WrongMoney"] = "giá nhập ko được âm";
                return this.Create();
            }
            data.SanPham.AddOrUpdate(sp);
            data.SaveChanges();
            return RedirectToAction("Create");
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
            if (sanpham.SoLuong <= 0)
            {
                ViewData["WrongNumber"] = "số lượng ko được âm";
                return this.Create();
            }
            else if (sanpham.GiaBan <= 0)
            {
                ViewData["WrongMoney1"] = "giá bán ko được âm";
                return this.Create();
            }
            else if (sanpham.GiaNhap <= 0)
            {
                ViewData["WrongMoney"] = "giá nhập ko được âm";
                return this.Create();
            }
            var E_tensp = collection["TenSP"];
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