namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("REQHDR")]
    public partial class REQHDR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REQHDR()
        {
            REQDETs = new HashSet<REQDET>();
            Fecha_lim_proveedor = DateTime.Now;
            Id_REQHDR_Parent = 0;
            Id_Comprador = 0;
            //Asignar fecha validación
        }

        [Key]
        public int Id_REQHDR { get; set; }

        [Required]
        [StringLength(3)]
        public string Sucursal { get; set; }

        public DateTime Fecha_creacion { get; set; }

        public int Id_Creador { get; set; }

        public int Estatus { get; set; }

        public DateTime Fecha_validacion { get; set; }

        public int? Id_Validador { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REQDET> REQDETs { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual Usuario Usuario1 { get; set; }
        public DateTime? Fecha_lim_proveedor { get; set; }
        
        public int Id_REQHDR_Parent { get; set; }
        public int Id_Comprador { get; set; }

    }
}
