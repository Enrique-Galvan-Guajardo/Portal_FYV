namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("REQDET")]
    public partial class REQDET
    {
        [Key]
        public int Id_REQDET { get; set; }

        public int Id_REQHDR { get; set; }

        [Required]
        [StringLength(15)]
        public string Clave_articulo { get; set; }

        [Required]
        [StringLength(60)]
        public string Descripcion { get; set; }

        [Column(TypeName = "money")]
        public decimal Cantidad_solicitada { get; set; }

        public DateTime Fecha_creacion { get; set; }

        public int Id_Embalaje { get; set; }

        [Column(TypeName = "money")]
        public decimal? Cantidad_validada { get; set; }

        public DateTime? Fecha_validacion { get; set; }

        public int? Id_Embalaje_validado { get; set; }

        public DateTime? Fecha_ultima_compra { get; set; }

        [Column(TypeName = "money")]
        public decimal? Cantidad_ultima_compra { get; set; }

        [Column(TypeName = "money")]
        public decimal? Ventas_ultima_semana { get; set; }

        public int? Existencia { get; set; }

        [Required]
        [StringLength(50)]
        public string Estatus { get; set; }

        public virtual REQHDR REQHDR { get; set; }
    }
}
