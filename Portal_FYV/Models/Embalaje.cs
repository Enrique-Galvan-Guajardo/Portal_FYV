namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Embalaje
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Embalaje()
        {
            REQDETs = new HashSet<REQDET>();
        }

        [Key]
        public int Id_Embalaje { get; set; }

        [Required]
        public string Tipo_Embalaje { get; set; }

        [Column(TypeName = "money")]
        public decimal valor_um { get; set; }
        
        [Required]
        [StringLength(3)]
        public string um { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REQDET> REQDETs { get; set; }
    }
}
