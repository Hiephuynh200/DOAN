﻿using DOAN.Models;
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
            var all_SanPham = data.SanPham.OrderBy(s => s.TenSP);
            var all_SanPhamTK = data.SanPham.OrderBy(m => m.TenSP).Where(sp => sp.TenSP.ToUpper().Contains(SearchString.ToUpper()));
            page = 1;
            if (SearchString != null)
                return View(all_SanPhamTK.ToPagedList(pageNum, pageSize));
            return View(all_SanPham.ToPagedList(pageNum, pageSize));
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
            return View();
        }
        [HttpPost]
        public ActionResult Create(SanPham sp)
        {
            MyDataContextDB data = new MyDataContextDB();
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
        public ActionResult Edit(int id, FormCollection collection)
        {

            var sp = data.SanPham.First(m => m.MaSP == id);
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