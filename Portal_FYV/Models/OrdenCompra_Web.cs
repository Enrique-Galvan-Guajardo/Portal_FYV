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
        }
        [Key]
        public int Id_OrdenCompra { get; set; }

        [StringLength(6)]
        public string Id_REQDET { get; set; }
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
        public string Embalaje { get; set; }
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
        public string Guanza { get; set; }
        [StringLength(45)]
        public string Ofertas { get; set; }
        [StringLength(45)]
        public string Guanajuato { get; set; }
        public DateTime Fecha_limite { get; set; }
        [StringLength(45)]
        public string Id_Proveedor_Merksys { get; set; }
    }
}
