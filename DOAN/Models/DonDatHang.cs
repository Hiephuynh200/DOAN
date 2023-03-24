namespace DOAN.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("DonDatHang")]
    public partial class DonDatHang
    {
        MyDataContextDB data = new MyDataContextDB();
        public DonDatHang() { }

        [Key]
        [Column(Order = 0)]
        public int MaSP { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaKH { get; set; }

        public int? SoLuong { get; set; }
        public Double? ThanhTien
        {
            get { return SoLuong * SanPham.GiaBan; }
        }
        public virtual KhachHang KhachHang { get; set; }

        public virtual SanPham SanPham { get; set; }
        public DonDatHang(int idSP)
        {
            MaSP = idSP;
            SanPham = data.SanPham.Single(s => s.MaSP == MaSP);
            //MaTK = user;
            SoLuong = 1;
        }
        public DonDatHang(int idSP, int maTK)
        {
            this.MaSP = idSP;
            this.MaKH = maTK;
        }
    }
}
