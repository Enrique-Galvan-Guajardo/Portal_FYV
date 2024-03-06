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
        [Key]
        public int Id_UsuarioProducto { get; set; }

        public int Id_Usuario { get; set; }

        public int Id_Producto { get; set; }

        public virtual Producto Producto { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
