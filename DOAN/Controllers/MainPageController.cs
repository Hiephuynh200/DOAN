using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class MainPageController : Controller
    {
        // GET: MainPage
        MyDataContextDB data = new MyDataContextDB();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListSP()
        {
            var allSp = from ss in data.SanPham select ss;
            return View(allSp);
        }

        public ActionResult Buy()
        {
            return View();
        }
    }
}