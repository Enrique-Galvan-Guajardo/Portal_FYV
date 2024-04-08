namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UsuariosProductos")]
    public partial class UsuariosProductos
    {
        public UsuariosProductos() {
            Id_REQHDR = 0;
            Cantidad_comprada = 0;
        }
        [Key]
        public int Id_UsuarioProducto { get; set; }

        public int Id_Usuario { get; set; }

        public int Id_Producto { get; set; }

        public decimal Precio { get; set; }
        public int Id_REQHDR { get; set; }
        public decimal Cantidad_comprada { get; set; }

        public virtual Producto Producto { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
