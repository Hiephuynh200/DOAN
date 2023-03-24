using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class AoController : Controller
    {
        // GET: Ao
        MyDataContextDB data = new MyDataContextDB();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SpAo()
        {
            var ListAo = data.SanPham.Where(x => x.MaLoai == 1001).ToList();
            return View(ListAo);
        }
        public ActionResult Detail(int id)
        {
            var D_sach = data.SanPham.Where(m => m.MaSP == id).First(); 
            return View(D_sach);
        }
    }
}