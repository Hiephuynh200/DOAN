using DOAN.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class MainPageController : Controller
    {
        MyDataContextDB data = new MyDataContextDB();
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
        public ActionResult ListSP()
        {
            var allSp = from ss in data.SanPham select ss;
            return View(allSp);
        }
        public ActionResult Detail(int id)
        {
            var D_SP = data.SanPham.Where(m => m.MaSP == id).First(); 
            return View(D_SP);
        }
        public ActionResult Buy()
        {
            return View();
        }
    }
}