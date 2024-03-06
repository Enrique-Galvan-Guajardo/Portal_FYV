namespace Portal_FYV.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UsuariosAltas
    {
        [Key]
        public int Id_UsuarioAlta { get; set; }

        public int Id_UsuarioRegistra { get; set; }

        public int? Id_UsuarioAprueba { get; set; }


        public DateTime Fecha { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual Usuario Usuario1 { get; set; }

    }
}
