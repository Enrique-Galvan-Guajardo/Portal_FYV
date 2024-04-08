using Portal_FYV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_FYV.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["Id_Usuario"] != null)
            {
                db_model db = new db_model();

                string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
                string sucursal = Session["Sucursal"] != null ? Session["Sucursal"].ToString() : "";
                int id = Convert.ToInt32(Session["Id_Usuario"]);
                
                Usuario usuario = db.Usuarios.Find(id);
                
                // Obtener la fecha actual del servidor
                DateTime fechaActual = DateTime.Now;
                // Calcular la fecha y hora límite para que hayan pasado 1.5 días (36 horas)
                DateTime fechaLimite = fechaActual.AddHours(-Convert.ToInt32(usuario.Prorroga)); // 1.5 días = 36 horas
                
                List<REQHDR> rEQHDRs = new List<REQHDR>();
                List<REQDET> rEQDETs = new List<REQDET>();

                List<Producto> productosProveedor = new List<Producto>();
                List<UsuariosProductos> usuarioPrecios = new List<UsuariosProductos>();

                string[] productosNombres;
                int[] productosIds;
                int[] ids_reqhdrs;
                
                switch (rol)
                {
                    case "Admin+":
                    case "Admin":
                    case "Compras":

                        // Filtrar los registros que todavía no han pasado 1.5 días desde su creación
                        rEQHDRs = db.REQHDRs.Where(r => r.Fecha_creacion >= fechaLimite && r.Sucursal == sucursal).ToList();
                        ids_reqhdrs = rEQHDRs.Select(r => r.Id_REQHDR).ToArray();

                        rEQDETs = db.REQDETs.Where(r => ids_reqhdrs.Contains(r.Id_REQHDR)).ToList();

                        ViewBag.totalRegistrosFiltrados = rEQHDRs;
                        ViewBag.totalReqdets = rEQDETs;

                        productosNombres = rEQDETs.Select(x => x.Descripcion).ToArray();
                        productosProveedor = db.Productos.Where(x => productosNombres.Contains(x.Descripcion)).ToList();
                        productosIds = productosProveedor.Select(x => x.Id_Producto).ToArray();

                        int[] productosProvIds = productosProveedor.Select(x => x.Id_Proveedor).ToArray();

                        usuarioPrecios = db.UsuariosProductos.Where(x => productosIds.Contains(x.Id_Producto) && ids_reqhdrs.Contains(x.Id_REQHDR) && productosProvIds.Contains(x.Id_Usuario)).ToList();
                        
                        ViewBag.totalProductos = productosProveedor;
                        ViewBag.totalPrecios = usuarioPrecios;

                        break;
                    case "Proveedores":
                        
                        rEQHDRs = db.REQHDRs.Where(r => r.Fecha_creacion >= fechaLimite && r.Estatus == 2).ToList();
                        ids_reqhdrs = rEQHDRs.Select(r => r.Id_REQHDR).ToArray();

                        rEQDETs = db.REQDETs.Where(r => ids_reqhdrs.Contains(r.Id_REQHDR)).ToList();

                        ViewBag.totalRegistrosFiltrados = rEQHDRs;
                        ViewBag.totalReqdets = rEQDETs;

                        productosProveedor = db.Productos.Where(x => x.Id_Proveedor == id).ToList();
                        productosNombres = productosProveedor.Count() > 0 ? productosProveedor.Select(x => x.Descripcion).ToArray() : new string[0];
                        productosIds = productosProveedor.Count() > 0 ? productosProveedor.Select(x => x.Id_Producto).ToArray() : new int[0];

                        usuarioPrecios = db.UsuariosProductos.Where(x => productosIds.Contains(x.Id_Producto) && ids_reqhdrs.Contains(x.Id_REQHDR) && x.Id_Usuario == usuario.Id_Usuario).ToList();

                        ViewBag.totalProductos = productosProveedor;
                        ViewBag.totalPrecios = usuarioPrecios;

                        break;
                    default:
                        break;
                }
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}