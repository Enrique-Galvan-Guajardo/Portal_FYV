namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrdenCompra_Web")]
    public partial class OrdenCompra_Web
    {
        public OrdenCompra_Web()
        {
            estatus = "0";
            tipo = "MULTIPLE";
            fecha = DateTime.Now;
        }
        [Key]
        public int Id_OrdenCompra { get; set; }

        [StringLength(20)]
        public string num_orden { get; set; }

        [StringLength(15)]
        public string cve_art { get; set; }
        [StringLength(10)]
        public string suc { get; set; }

        public decimal cant { get; set; }
        public decimal descto { get; set; }
        [StringLength(300)]
        public string promocion { get; set; }
        [StringLength(15)]
        public string prv { get; set; }
        public DateTime? fecha { get; set; }
        [StringLength(300)]
        public string estatus { get; set; }
        [StringLength(25)]
        public string tipo { get; set; }
        [StringLength(50)]
        public string sucursal_receptora { get; set; }

    }
}
