using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        MyDataContextDB data = new MyDataContextDB();
        // GET: Admin/GioHang

        public List<DonDatHang> Laygiohang()
        {
            int maKH = (int)Session["TaiKhoanKH"];
            List<DonDatHang> lstGiohang = Session["GioHang"] as List<DonDatHang>;
            var loadGH = data.DonDatHang.Where(x => x.MaKH == maKH).ToList();
            lstGiohang = loadGH;
            if (lstGiohang == null)
            {
                lstGiohang = new List<DonDatHang>();
                Session["GioHang"] = lstGiohang;
            }
            //else if (lstGiohang == null)
            //{
            //    lstGiohang = new List<GioHang>();
            //    Session["GioHang"] = lstGiohang;
            //}
            else
            {
                foreach (DonDatHang gh in loadGH)
                {
                    gh.SanPham = (SanPham)data.SanPham.FirstOrDefault(x => x.MaSP == gh.MaSP);
                    gh.KhachHang = (KhachHang)data.KhachHang.FirstOrDefault(x => x.MaKH == gh.MaKH);
                    if (lstGiohang == null)
                    {
                        lstGiohang = new List<DonDatHang>();
                        lstGiohang.Add(gh);
                    }
                    else
                    {
                        if (!lstGiohang.Contains(gh))
                            lstGiohang.Add(gh);
                        else Session["GioHang"] = lstGiohang;
                    }
                }
            }
            //Session["GioHang"] = lstGiohang;
            return lstGiohang;

        }
        public ActionResult ThemGioHang(int id, string strURL)
        {
            if (Session["TaiKhoanKH"] == null || Session["TaiKhoanKH"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            int maTK = (int)Session["TaiKhoanKH"];
            List<DonDatHang> lstGiohang = Laygiohang();
            DonDatHang sanpham = lstGiohang.Find(s => s.MaSP == id);
            DonDatHang temp = new DonDatHang(id, maTK);
            if (sanpham == null)
            {
                sanpham = new DonDatHang(id);
                sanpham.MaKH = maTK;
                lstGiohang.Add(sanpham);
                temp.SoLuong = sanpham.SoLuong;
                data.DonDatHang.Add(temp);
                data.SaveChanges();
                return Redirect(strURL);
            }
            else
            {
                sanpham.SoLuong += 1;
                lstGiohang.Add(sanpham);
                temp.SoLuong = sanpham.SoLuong; ;
                data.DonDatHang.AddOrUpdate(temp);
                data.SaveChanges();
                return Redirect(strURL);
            }
        }
        public int? TongSoLuong()
        {

            int? tsl = 0;
            List<DonDatHang> lstGiohang = Session["GioHang"] as List<DonDatHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.SoLuong);
            }
            return tsl;
        }
        public int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<DonDatHang> lstGiohang = Session["GioHang"] as List<DonDatHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }

        private double? TongTien()
        {
            double? tt = 0;
            List<DonDatHang> lstGiohang = Session["GioHang"] as List<DonDatHang>;

            if (lstGiohang != null)
            {
                tt = lstGiohang.Sum(n => n.ThanhTien);
            }
            return tt;
        }

        public ActionResult GioHang()
        {
            List<DonDatHang> lstGiohang = Laygiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.TongSoLuongSanPham = TongSoLuongSanPham();
            return View(lstGiohang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.TongSoLuongSanPham = TongSoLuongSanPham();
            return PartialView();
        }

        public ActionResult XoaGioHang(int id)
        {
            List<DonDatHang> lstGiohang = Laygiohang();
            DonDatHang sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.MaSP == id);
                data.DonDatHang.Remove(sanpham);
                data.SaveChanges();
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapnhatGiohang(int id, FormCollection collection)
        {
            List<DonDatHang> lstGiohang = Laygiohang();
            DonDatHang gioHang = lstGiohang.SingleOrDefault(s => s.MaSP == id);
            SanPham sanPham = data.SanPham.SingleOrDefault(sp => sp.MaSP == id);
            if (gioHang != null)
            {
                int sl = int.Parse(collection["txtSolg"].ToString());
                gioHang.SoLuong = sl;
                data.DonDatHang.AddOrUpdate(gioHang);
                data.SaveChanges();
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGiohang()
        {
            List<DonDatHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoanKH"] == null || Session["TaiKhoanKH"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Sach");
            }
            List<DonDatHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }

        public ActionResult DatHang(FormCollection collection)
        {

            HoaDon dh = new HoaDon();
            KhachHang kh = (KhachHang)Session["FullTaiKhoan"];
            SanPham s = new SanPham();
            List<DonDatHang> lstGiohang = Session["GioHang"] as List<DonDatHang>;
            dh.MaKH = kh.MaKH;
            dh.NgayLapHoaDon = DateTime.Now;
            data.HoaDon.Add(dh);
            data.SaveChanges();
            foreach (var item in lstGiohang)
            {
                var temp = data.DonDatHang.FirstOrDefault(x => x.MaKH == item.MaKH && x.MaSP == item.MaSP);
                data.DonDatHang.Remove(temp);
                ChiTietHoaDon ctdh = new ChiTietHoaDon();
                s = data.SanPham.Find(item.MaSP);
                ctdh.MaSP = item.MaSP;
                ctdh.MaHD = dh.MaHD;
                ctdh.SoLuongSanPham = item.SoLuong;
                s.SoLuong -= item.SoLuong;
                data.ChiTietHoaDon.Add(ctdh);
                data.SanPham.AddOrUpdate(s);
                data.SaveChanges();
            }
            data.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacnhanDonhang", "GioHang");
        }

   
        public ActionResult XacnhanDonhang()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}