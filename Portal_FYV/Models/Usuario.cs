namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            permitir_Fru = false;
            permitir_Sec = false;
            permitir_Veg = false;
            Id_Estatus = 1;
            Id_Rol = 3;
            Prorroga = "0";
            REQHDRs = new HashSet<REQHDR>();
            REQHDRs1 = new HashSet<REQHDR>();
            UsuariosAltas = new HashSet<UsuariosAltas>();
            UsuariosAltas1 = new HashSet<UsuariosAltas>();
            UsuariosProductos = new HashSet<UsuariosProductos>();
        }

        [Key]
        public int Id_Usuario { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        public string Nombre { get; set; }

        public string Correo { get; set; }

        [Required]
        [StringLength(50)]
        public string Contrasena { get; set; }

        public int Id_Estatus { get; set; }

        public DateTime? Fecha_Aprov { get; set; }

        public int Id_Rol { get; set; }

        [StringLength(50)]
        public string Sucursal { get; set; }

        public string Razon_social { get; set; }

        [StringLength(50)]
        public string RFC { get; set; }

        [StringLength(3)]
        public string Pais { get; set; }

        [StringLength(3)]
        public string Estado { get; set; }

        [StringLength(3)]
        public string Ciudad { get; set; }

        [StringLength(4)]
        public string Colonia { get; set; }

        [StringLength(2)]
        public string Localidad { get; set; }

        [StringLength(5)]
        public string Codigo_postal { get; set; }

        public string Calle { get; set; }

        public int? Numero { get; set; }

        [StringLength(9)]
        public string Proveeedor_no_mks { get; set; }

        [StringLength(50)]
        public string Contacto_nombre1 { get; set; }

        [StringLength(50)]
        public string Contacto_nombre2 { get; set; }

        [StringLength(50)]
        public string Contacto_nombre3 { get; set; }

        [StringLength(50)]
        public string Contacto_correo1 { get; set; }

        [StringLength(50)]
        public string Contacto_correo2 { get; set; }

        [StringLength(50)]
        public string Contacto_correo3 { get; set; }

        [StringLength(50)]
        public string Contacto_tel1 { get; set; }

        [StringLength(50)]
        public string Contacto_tel2 { get; set; }

        [StringLength(50)]
        public string Contacto_tel3 { get; set; }

        public bool permitir_Fru { get; set; }
        public bool permitir_Sec { get; set; }
        public bool permitir_Veg { get; set; }
        public string Prorroga { get; set; }

        public virtual Estatus Estatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REQHDR> REQHDRs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REQHDR> REQHDRs1 { get; set; }

        public virtual Roles Roles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuariosAltas> UsuariosAltas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuariosAltas> UsuariosAltas1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuariosProductos> UsuariosProductos { get; set; }
    }
}
