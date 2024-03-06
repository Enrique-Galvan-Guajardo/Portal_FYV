namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Productos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Productos()
        {
            UsuariosProductos = new HashSet<UsuariosProductos>();
        }

        [Key]
        public int Id_Producto { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Clave_externa { get; set; }

        [Required]
        public string Clave_interna { get; set; }

        [Required]
        public string Codigo_barras { get; set; }

        public string Imagen_ruta { get; set; }

        [Required]
        public string Embalaje { get; set; }

        [Required]
        public string Unidad { get; set; }

        public DateTime Fecha_Creacion { get; set; }

        public DateTime Fecha_Modificacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuariosProductos> UsuariosProductos { get; set; }
    }
}
