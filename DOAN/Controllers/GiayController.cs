using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class GiayController : Controller
    {
        // GET: Giay
        MyDataContextDB data = new MyDataContextDB();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SpGiay()
        {
            var ListGiay = data.SanPham.Where(x => x.MaLoai == 1004).ToList();
            return View(ListGiay);
        }
    }
    }