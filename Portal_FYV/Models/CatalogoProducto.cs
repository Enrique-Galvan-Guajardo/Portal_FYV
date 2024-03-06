namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CatalogoProductos")]
    public partial class CatalogoProducto
    {
        [Key]
        public int Id_Catalogo { get; set; }

        public int Clave_numero { get; set; }

        [Required]
        public string Codigo_barras { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Linea { get; set; }

        [Required]
        public string Sublinea { get; set; }

        [Required]
        public string Familia { get; set; }

        [Required]
        public string Subfamilia { get; set; }

        public bool Estatus { get; set; }
    }
}
