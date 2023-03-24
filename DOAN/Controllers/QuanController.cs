using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class QuanController : Controller
    {
        // GET: Quan
        MyDataContextDB data = new MyDataContextDB();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SpQuan()
        {
            var ListQuan = data.SanPham.Where(x => x.MaLoai == 1002).ToList();
            return View(ListQuan);
        }
    }
}