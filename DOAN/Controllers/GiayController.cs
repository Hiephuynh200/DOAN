using DOAN.Models;
using PagedList;
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
        public ActionResult SpGiay()
        {
            var ListGiay = data.SanPham.Where(x => x.MaLoai == 1004).ToList();
            return View(ListGiay);
        }
    }
}