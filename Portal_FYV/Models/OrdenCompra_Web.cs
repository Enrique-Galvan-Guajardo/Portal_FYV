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
            Fecha_creacion = DateTime.Now;
            Fecha_limite = Convert.ToDateTime(Fecha_creacion);
            Juarez = "0";
            Villas = "0";
            Almaguer = "0";
            Jarachina = "0";
            Balcones = "0";
            Cedis = "0";
            Guanza = "0";
            Ofertas = "0";
            Guanajuato = "0";
        }
        [Key]
        public int Id_OrdenCompra { get; set; }

        public string REQHDRS { get; set; }
        [StringLength(100)]
        public string Creador { get; set; }
        [StringLength(100)]
        public string Proveedor { get; set; }
        [StringLength(100)]
        public string Id_Merksys { get; set; }
        [StringLength(100)]
        public string Producto { get; set; }

        [StringLength(100)]
        public string Cantidad_solicitada { get; set; }
        [StringLength(100)]
        public string Precio { get; set; }
        [StringLength(100)]
        public string Cantidad_validada { get; set; }
        public DateTime Fecha_creacion { get; set; }
        [StringLength(45)]
        public string Juarez { get; set; }
        [StringLength(45)]
        public string Villas { get; set; }
        [StringLength(45)]
        public string Almaguer { get; set; }
        [StringLength(45)]
        public string Jarachina { get; set; }
        [StringLength(45)]
        public string Balcones { get; set; }
        [StringLength(45)]
        public string Cedis { get; set; }
        [StringLength(45)]
        public string Guanza { get; set; }
        [StringLength(45)]
        public string Ofertas { get; set; }
        [StringLength(45)]
        public string Guanajuato { get; set; }
        public DateTime Fecha_limite { get; set; }
        [StringLength(45)]
        public string Id_Proveedor_Merksys { get; set; }
        [StringLength(9)]
        public string Id_Producto_Merksys { get; set; }
        [StringLength(10)]
        public string Estatus { get; set; }
    }
}
