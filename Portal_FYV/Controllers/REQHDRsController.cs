using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.Win32;
using Portal_FYV.Models;

namespace Portal_FYV.Controllers
{
    public class REQHDRsController : Controller
    {
        private db_model db = new db_model();

        // GET: REQHDRs
        public ActionResult Index()
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            if (rol != "Admin+" && rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var rEQHDRs = db.REQHDRs.Include(r => r.Usuario).Include(r => r.Usuario1);
            return View(rEQHDRs.OrderByDescending(x => x.Id_REQHDR).ToList());
        }

        // GET: REQHDRs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQHDR rEQHDR = db.REQHDRs.Find(id);
            List<REQDET> rEQDETs = rEQHDR.REQDETs.ToList();
            if (rEQHDR == null)
            {
                return HttpNotFound();
            }

            id = Convert.ToInt32(Session["Id_Usuario"]);
            Usuario usuario = db.Usuarios.Find(id);

            // Cargar la lista de embalajes
            var embalajes = db.Embalajes.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<REQHDR, List<REQDET>, SelectList, Usuario>(rEQHDR, rEQDETs, new SelectList(embalajes, "Id_Embalaje", "Tipo_Embalaje"), usuario);
            
            ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje");

            return View(modelos);
        }

        // GET: REQHDRs/Resumen/5
        public ActionResult Resumen(DateTime searchDate)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            string sucursal = Session["Sucursal"] != null ? Session["Sucursal"].ToString() : "";
            int id = Convert.ToInt32(Session["Id_Usuario"]);

            Usuario usuario = db.Usuarios.Find(id);

            List<REQHDR> rEQHDRs = new List<REQHDR>();
            List<REQDET> rEQDETs = new List<REQDET>();
            List<Producto> productos = new List<Producto>();
            
            string[] descripciones;
            int[] ids_productos;

            // Ajusta la fecha para eliminar la parte de la hora
            DateTime searchDateOnly = searchDate.Date;

            // Realiza la consulta comparando solo las fechas
            int[] selectedIds;

            List<CatalogoProducto> catalogo = new List<CatalogoProducto>();
            List<UsuariosProductos> usuariosProductos = new List<UsuariosProductos>();

            var modelos = new object();

            switch (rol)
            {
                case "Admin+":
                case "Admin":
                case "Compras":
                    selectedIds = db.REQHDRs
                    .Where(x => DbFunctions.TruncateTime(x.Fecha_creacion) == searchDateOnly && x.Id_REQHDR_Parent == 0)
                    .Select(x => x.Id_REQHDR)
                    .ToArray();
                    if (selectedIds == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    rEQHDRs = db.REQHDRs.Where(x => selectedIds.Contains(x.Id_REQHDR) && x.Id_REQHDR_Parent == 0).ToList();

                    /*
                    
                    Verificar si las mismas ordenes de las solicitudes están con estatus CARRITO_PR, para evitar modificaciones mientras están en proceso todas
                    string REQHDRS = String.Join("-", rEQHDRs.Select(x => x.Id_REQHDR).Distinct());
                    
                    var ids_reqhdrs = REQHDRS.Split('-');

                    var ordenCompra_Web = db.OrdenCompras_Web.Where(x => x.REQHDRS == REQHDRS || ids_reqhdrs.Contains(x.REQHDRS)).ToList();

                    if (ordenCompra_Web.Count() == ordenCompra_Web.Where(x => x.Estatus == "CARRITO_PR").Count())
                    {
                        return RedirectToAction("Error", "Home");
                    }
                    
                    */

                    rEQDETs = db.REQDETs.Where(x => selectedIds.Contains(x.Id_REQHDR)).ToList();

                    /*Validar que los reqhdrs de cada reqdet no estén procesandose en ordencompraweb*/
                    /*Estatus | CARRITO = Agregados al carrito, CARRITO_PR = Agregados y posteriormente procesandose, CAMCELADO = Productos cancelados, TERM = Finalizados*/

                    string[] descriptions_reqdets = rEQDETs.Select(x => x.Descripcion.Trim()).Distinct().ToArray();
                    List<OrdenCompra_Web> ocw = db.OrdenCompras_Web.Where(x => descriptions_reqdets.Contains(x.Producto.Trim())).OrderBy(x => x.Id_OrdenCompra).Distinct().ToList();
                    List<string> ids_reqhdrs_temps = selectedIds.Select(x => x.ToString()).ToList();
                    ocw = ocw.Where(x => x.REQHDRS.Split('-').Intersect(ids_reqhdrs_temps).Any()).ToList();
                    ViewBag.ordenCompra = ocw;
                    /*
                    List<REQDET> delete_reqdets = new List<REQDET>();
                    List<REQHDR> delete_reqhdrs = new List<REQHDR>();
                    foreach (var reqdet in rEQDETs)
                    {
                        if (ocw.Any(x => x.Producto.Trim() == reqdet.Descripcion.Trim() && x.REQHDRS.Split('-').Contains(reqdet.Id_REQHDR.ToString())))
                        {
                            //Los que estén a punto de eliminarse se guardarán en una variable de lista temporal, para después eliminarlos de golpe del listado original
                            delete_reqdets.Add(rEQDETs.FirstOrDefault(x => x.Descripcion.Trim() == reqdet.Descripcion.Trim() && x.Id_REQHDR == reqdet.Id_REQHDR));
                            delete_reqhdrs.Add(rEQHDRs.FirstOrDefault(x => x.Id_REQHDR == reqdet.Id_REQHDR));
                        }
                    }

                    rEQDETs = rEQDETs.Where(x => !delete_reqdets.Select(y => y.Id_REQDET).ToArray().Contains(x.Id_REQDET)).ToList();
                    rEQHDRs = rEQHDRs.Where(x => !rEQDETs.Select(y => y.Id_REQHDR).ToArray().Contains(x.Id_REQHDR)).ToList();
                     */

                    descripciones = rEQDETs.Select(x => x.Descripcion.Trim()).ToArray();

                    productos = db.Productos.Where(x => descripciones.Contains(x.Descripcion)).ToList();
                    ViewBag.productos = productos;

                    ids_productos = productos.Select(x => x.Id_Producto).ToArray();

                    catalogo = db.CatalogoProductos.Where(x => descripciones.Contains(x.Descripcion.Trim())).ToList();

                    if (rEQDETs == null)
                    {
                        return RedirectToAction("Error", "Home");
                    }

                    int[] ids_proveedores = db.UsuariosProductos.Where(x => selectedIds.Contains(x.Id_REQHDR) && ids_productos.Contains(x.Id_Producto)).Select(x => x.Id_Usuario).Distinct().ToArray();
                    List<UsuariosProductos> uspr = db.UsuariosProductos.Where(x => selectedIds.Contains(x.Id_REQHDR) && ids_productos.Contains(x.Id_Producto) && ids_proveedores.Contains(x.Id_Usuario)).OrderByDescending(x => x.Id_UsuarioProducto).ToList();
                    foreach (var r in uspr)
                    {
                        if (!usuariosProductos.Any(x => x.Id_Usuario == r.Id_Usuario && x.Id_Producto == r.Id_Producto && x.Id_REQHDR == r.Id_REQHDR))
                        {
                            usuariosProductos.Add(r);
                        }
                    }

                    
                    // Crear un Tuple que contenga ambos modelos y la lista de embalajes
                    modelos = new Tuple<List<REQHDR>, List<REQDET>, List<UsuariosProductos>, List<CatalogoProducto>>(rEQHDRs, rEQDETs, usuariosProductos, catalogo);

                    return View(modelos);

                case "Proveedores":
                    selectedIds = db.REQHDRs
                    .Where(x => DbFunctions.TruncateTime(x.Fecha_validacion) == searchDateOnly && x.Id_REQHDR_Parent == 0)
                    .Select(x => x.Id_REQHDR)
                    .ToArray();
                    if (selectedIds == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    rEQHDRs = db.REQHDRs.Where(x => selectedIds.Contains(x.Id_REQHDR) && x.Id_REQHDR_Parent == 0).ToList();

                    productos = db.Productos.Where(x => x.Id_Proveedor == usuario.Id_Usuario).ToList();
                    ViewBag.productos = productos;

                    descripciones = productos.Select(x => x.Descripcion.Trim()).ToArray();
                    ids_productos = productos.Select(x => x.Id_Producto).ToArray();

                    catalogo = db.CatalogoProductos.Where(x => descripciones.Contains(x.Descripcion.Trim())).ToList();

                    foreach (var r in rEQHDRs)
                    {
                        rEQDETs.AddRange(r.REQDETs.Where(x => selectedIds.Contains(x.Id_REQHDR) && descripciones.Contains(x.Descripcion.Trim())).ToList());
                    }

                    if (rEQDETs == null)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                    //Para que existan resultados, debe haber registros de proveedores con precio hacia el respectivo producto de los REQHDRS en cuestión
                    usuariosProductos = db.UsuariosProductos.Where(x => ids_productos.Contains(x.Id_Producto) && selectedIds.Contains(x.Id_REQHDR) && x.Id_Usuario == usuario.Id_Usuario).ToList();

                    // Crear un Tuple que contenga ambos modelos y la lista de embalajes
                    modelos = new Tuple<List<REQHDR>, List<REQDET>, List<UsuariosProductos>, List<CatalogoProducto>>(rEQHDRs, rEQDETs, usuariosProductos, catalogo);

                    return View(modelos);
                default:
                    return RedirectToAction("Index", "Home");
            }
            
        }

        public ActionResult NewsResumenes(string ids_REQHDRS)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            string sucursal = Session["Sucursal"] != null ? Session["Sucursal"].ToString() : "";
            int id = Convert.ToInt32(Session["Id_Usuario"]);

            Usuario usuario = db.Usuarios.Find(id);

            List<REQHDR> rEQHDRs = new List<REQHDR>();
            List<REQDET> rEQDETs = new List<REQDET>();
            List<Producto> productos = new List<Producto>();

            string[] descripciones;
            int[] ids_productos;

            // Ajusta la fecha para eliminar la parte de la hora
            DateTime searchDateOnly = DateTime.Now;

            // Realiza la consulta comparando solo las fechas
            int[] selectedIds = ids_REQHDRS.Split('-').Select(x => Convert.ToInt32(x)).ToArray();

            List<CatalogoProducto> catalogo = new List<CatalogoProducto>();
            List<UsuariosProductos> usuariosProductos = new List<UsuariosProductos>();

            var modelos = new object();

            if (selectedIds == null)
            {
                return RedirectToAction("Index", "Home");
            }

            rEQHDRs = db.REQHDRs.Where(x => selectedIds.Contains(x.Id_REQHDR)).ToList();


            rEQDETs = db.REQDETs.Where(x => selectedIds.Contains(x.Id_REQHDR)).ToList();

            /*Validar que los reqhdrs de cada reqdet no estén procesandose en ordencompraweb*/
            /*Estatus | CARRITO = Agregados al carrito, CARRITO_PR = Agregados y posteriormente procesandose, CAMCELADO = Productos cancelados, TERM = Finalizados*/

            string[] descriptions_reqdets = rEQDETs.Select(x => x.Descripcion.Trim()).Distinct().ToArray();
            List<OrdenCompra_Web> ocw = db.OrdenCompras_Web.Where(x => descriptions_reqdets.Contains(x.Producto.Trim())).OrderBy(x => x.Id_OrdenCompra).Distinct().ToList();
            List<string> ids_reqhdrs_temps = selectedIds.Select(x => x.ToString()).ToList();
            ocw = ocw.Where(x => x.REQHDRS.Split('-').Intersect(ids_reqhdrs_temps).Any()).ToList();
            ViewBag.ordenCompra = ocw;

            descripciones = rEQDETs.Select(x => x.Descripcion.Trim()).ToArray();

            productos = db.Productos.Where(x => descripciones.Contains(x.Descripcion.Trim())).ToList();
            ViewBag.productos = productos;

            ids_productos = productos.Select(x => x.Id_Producto).ToArray();

            catalogo = db.CatalogoProductos.Where(x => descripciones.Contains(x.Descripcion.Trim())).ToList();

            if (rEQDETs == null)
            {
                return RedirectToAction("Error", "Home");
            }


            int[] ids_proveedores = new List<Int32>().ToArray();
            if (rol == "Proveedores")
            {
                List<UsuariosProductos> usp = db.UsuariosProductos.Where(x => x.Id_Usuario == id).ToList();
                usp = usp.Where(x => ids_productos.Contains(x.Id_Producto)).ToList();
                List<Producto> prods_prov = productos.Where(x => x.Id_Proveedor == id && descripciones.Contains(x.Descripcion.Trim())).ToList();
                int[] ids_prods_prov = prods_prov.Select(x => x.Id_Producto).ToArray();
                string[] ids_prodsDesc_prov = prods_prov.Select(x => x.Descripcion.Trim()).Distinct().ToArray();

                List<REQDET> rEQDETSProv = rEQDETs.Where(x => selectedIds.Contains(x.Id_REQHDR) && ids_prodsDesc_prov.Contains(x.Descripcion.Trim())).ToList();
                
                usp = usp.Where(x => x.Id_Usuario == id && ids_prods_prov.Contains(x.Id_Producto) && selectedIds.Contains(x.Id_REQHDR)).ToList();

                if (usp.Count() != rEQDETSProv.Count())
                {
                    ids_prods_prov = ids_prods_prov.Except(usp.Select(x => x.Id_Producto).ToArray()).ToArray();
                    ids_prodsDesc_prov = prods_prov.Where(x => ids_prods_prov.Contains(x.Id_Producto)).Select(x => x.Descripcion.Trim()).Distinct().ToArray();
                    //string[] names_prec_prod_prov = usp.Where(x => !selectedIds.Contains(x.Id_REQHDR)).Select(x => x.Producto.Descripcion.Trim()).ToArray();
                    rEQDETSProv = ids_prodsDesc_prov == null ? rEQDETSProv : rEQDETSProv.Where(x => ids_prodsDesc_prov.Contains(x.Descripcion.Trim())).ToList();
                    foreach (var item in rEQDETSProv)
                    {
                        UsuariosProductos UsPs = new UsuariosProductos();
                        UsPs.Id_Producto = prods_prov.FirstOrDefault(x => x.Descripcion.Trim() == item.Descripcion.Trim()).Id_Producto;
                        UsPs.Id_Usuario = id;
                        UsPs.Id_REQHDR = item.Id_REQHDR;
                        UsPs.Precio = 0;
                        UsPs.Cantidad_comprada = 0;
                        UsPs.Fecha_Creacion = DateTime.Now;
                        db.UsuariosProductos.Add(UsPs);
                        db.SaveChanges();
                        usuariosProductos.Add(UsPs);
                    }
                }
                ids_proveedores = prods_prov.Select(x => x.Id_Proveedor).ToArray();
            }
            else {
                ids_proveedores = db.UsuariosProductos.Where(x => selectedIds.Contains(x.Id_REQHDR) && ids_productos.Contains(x.Id_Producto)).Select(x => x.Id_Usuario).Distinct().ToArray();
            }            
            
            List<UsuariosProductos> uspr = db.UsuariosProductos.Where(x => selectedIds.Contains(x.Id_REQHDR) && ids_productos.Contains(x.Id_Producto) && ids_proveedores.Contains(x.Id_Usuario)).OrderByDescending(x => x.Id_UsuarioProducto).ToList();
            //List<UsuariosProductos> uspr = usp.Where(x => ids_productos.Contains(x.Id_Producto) && ids_proveedores.Contains(x.Id_Usuario)).OrderByDescending(x => x.Id_UsuarioProducto).ToList();
            foreach (var r in uspr)
            {
                if (!usuariosProductos.Any(x => x.Id_Usuario == r.Id_Usuario && x.Id_Producto == r.Id_Producto && x.Id_REQHDR == r.Id_REQHDR))
                {
                    usuariosProductos.Add(r);
                }
            }

            //Filtrar para saber cuales pertenecen al usuario Proveedor actual
            string[] productsName = usuariosProductos.Select(x => x.Producto.Descripcion.Trim()).ToArray();
            rEQDETs = rEQDETs.Where(x => productsName.Contains(x.Descripcion.Trim())).ToList();
            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            modelos = new Tuple<List<REQHDR>, List<REQDET>, List<UsuariosProductos>, List<CatalogoProducto>>(rEQHDRs, rEQDETs, usuariosProductos, catalogo);

            return View(modelos);
        }
        public ActionResult Compra(string REQHDRS, DateTime date)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            string sucursal = Session["Sucursal"] != null ? Session["Sucursal"].ToString() : "";
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            var modelos = new object();

            try
            {
                Usuario usuario = db.Usuarios.Find(id);

                List<OrdenCompra_Web> ordenCompra_Web = new List<OrdenCompra_Web>();
                // Obtener todas las órdenes de compra en memoria
                var ordenesCompra_Web = db.OrdenCompras_Web.ToList();

                // Filtrar las órdenes de compra que contienen cualquier elemento de ids_reqhdrs
                var ids_reqhdrs = REQHDRS.Split('-');
                
                ordenCompra_Web = ordenesCompra_Web
                    .Where(x => x.REQHDRS.Split('-').Intersect(ids_reqhdrs).Any() && x.Fecha_creacion.ToString("M") == date.ToString("M"))
                    .ToList();

                if (ordenCompra_Web != null)
                {
                    modelos = new Tuple<List<OrdenCompra_Web>, List<REQHDR>>(ordenCompra_Web, db.REQHDRs.Where(x => ids_reqhdrs.Contains(x.Id_REQHDR.ToString())).ToList());
                    return View(modelos);
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult procesarOC(string REQHDRS, int mode)
        {
            try
            {
                List<OrdenCompra_Web> ordenCompra_Web = new List<OrdenCompra_Web>();
                List<OrdenCompra_Web> ordenCompra_Web_COMPRA = db.OrdenCompras_Web.Where(x => x.Estatus == "CARRITO").ToList();

                var ids_reqhdrs = REQHDRS.Split('-');
                ordenCompra_Web = ordenCompra_Web_COMPRA.Where(x => x.REQHDRS.Split('-').Intersect(ids_reqhdrs).Any()).ToList();

                if (ordenCompra_Web == null)
                {
                    return Json(new
                    {
                        Success = false,
                        value = "",
                        Message = "No se han enviado las solicitudes para procesar las ordenes de compra.",
                        Message_data = "",
                        Message_Classes = "warning",
                        Message_concat = false
                    });
                }
                
                foreach (var oc in ordenCompra_Web)
                {
                    switch (mode)
                    {
                        case 1:
                            oc.Estatus = "CARRITO_PR";
                            oc.Fecha_limite = DateTime.Now.AddHours(1);
                            break;
                        case 2:
                            oc.Estatus = "CANCELADO";
                            break;
                        default:
                            break;
                    }
                    db.Entry(oc).State = EntityState.Modified;
                }
                
                db.SaveChanges();
                switch (mode)
                {
                    case 1:
                        // Execute PA | SP
                        db.ExecuteStoredProcedure("pa_Descarga_OC_FYV", null);
                        return Json(new
                        {
                            Success = true,
                            value = "",
                            Message = "Ordenes de compra aprobadas para procesarse.",
                            Message_data = "",
                            Message_Classes = "success",
                            Message_concat = false
                        });
                    case 2:
                        return Json(new
                        {
                            Success = true,
                            value = "",
                            Message = "Ordenes de compra canceladas.",
                            Message_data = "",
                            Message_Classes = "primary",
                            Message_concat = false
                        });
                    default:
                        return Json(new
                        {
                            Success = true,
                            value = "",
                            Message = "Método finalizado con éxito para ordenes de compra sin modo seleccionado.",
                            Message_data = "",
                            Message_Classes = "warning",
                            Message_concat = false
                        });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = false,
                    value = "",
                    Message = "Error al procesar las ordenes de compra.",
                    Message_data = "",
                    Message_Classes = "danger",
                    Message_concat = false
                });
            }
        }

        [HttpPost]
        public ActionResult distribuirCompras(List<UsuariosProductos> usuariosProductos, int[] idsREQHDRS_Resumen, int[] idsUsuariosProductos_Resumen)
        {
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            Usuario usuario = db.Usuarios.Find(id);

            try
            {
                List<OrdenCompra_Web> ordenCW = new List<OrdenCompra_Web>();
                bool isNewRow = false;
                decimal totalPrecio = 0;
                foreach (var item in usuariosProductos)
                {
                    OrdenCompra_Web ordenCompra = new OrdenCompra_Web();
                    
                    UsuariosProductos up = db.UsuariosProductos.Find(item.Id_UsuarioProducto);
                    up.Cantidad_comprada = item.Cantidad_comprada;

                    //Consultas para objeto ordenCompra

                    Producto prd = db.Productos.Find(up.Id_Producto);
                    REQHDR hdr = db.REQHDRs.Find(up.Id_REQHDR);
                    REQDET rdet = db.REQDETs.FirstOrDefault(x => x.Id_REQHDR == hdr.Id_REQHDR && x.Descripcion == prd.Descripcion);

                    int[] ids_reqhdrs = usuariosProductos.Select(x => x.Id_REQHDR).ToArray();
                    int[] ids_reqdets = db.REQDETs.Where(x => ids_reqhdrs.Contains(x.Id_REQHDR) && x.Descripcion == prd.Descripcion).Select(x => x.Id_REQDET).ToArray();

                    ordenCompra.REQHDRS = String.Join("-", idsREQHDRS_Resumen);
                    ordenCompra.Producto = prd.Descripcion;
                    ordenCompra.Proveedor = up.Usuario.Nombre;
                    ordenCompra.Creador = usuario.Correo;

                    //Si existe ya uno en db, meterlo a la lista
                    if (db.OrdenCompras_Web.Any(x => x.REQHDRS == ordenCompra.REQHDRS && x.Producto == ordenCompra.Producto && x.Proveedor == ordenCompra.Proveedor))
                    {
                        ordenCW.Add(db.OrdenCompras_Web.FirstOrDefault(x => x.REQHDRS == ordenCompra.REQHDRS && x.Producto == ordenCompra.Producto && x.Proveedor == ordenCompra.Proveedor));
                    }
                    else
                    {
                        isNewRow = true;
                    }

                    //Si es la 2 vuelta, traerlo de la lista actual
                    if (ordenCW.Any(x => x.REQHDRS == ordenCompra.REQHDRS && x.Producto == ordenCompra.Producto && x.Proveedor == ordenCompra.Proveedor))
                    {
                        ordenCompra = ordenCW.FirstOrDefault(x => x.REQHDRS == ordenCompra.REQHDRS && x.Producto == ordenCompra.Producto && x.Proveedor == ordenCompra.Proveedor);
                        ordenCompra.Creador = usuario.Correo;
                        totalPrecio = up.Precio * up.Cantidad_comprada;
                        ordenCompra = asignarCantidadSucursal(ordenCompra, up);
                        ordenCompra.Precio = Convert.ToString(asignarPrecioSucursal(db.UsuariosProductos.Where(x => idsUsuariosProductos_Resumen.Contains(x.Id_UsuarioProducto) && x.Id_Producto == up.Id_Producto && x.Id_Usuario == up.Id_Usuario).ToList()));
                        
                        if (ordenCompra.Id_OrdenCompra > 0)
                        {
                            db.Entry(ordenCompra).State = EntityState.Modified;
                        }
                        
                    }//Si es la primera vuelta, generarlo en la lista
                    else
                    {
                        ordenCompra.Cantidad_solicitada = db.REQDETs.Where(x => ids_reqdets.Contains(x.Id_REQDET)).Select(x => x.Cantidad_solicitada).Sum().ToString();
                        ordenCompra.Cantidad_validada = db.REQDETs.Where(x => ids_reqdets.Contains(x.Id_REQDET)).Select(x => x.Cantidad_validada).Sum().ToString();
                        totalPrecio = up.Precio * up.Cantidad_comprada;
                        ordenCompra.Precio = totalPrecio.ToString();
                        ordenCompra = asignarCantidadSucursal(ordenCompra, up);
                        ordenCW.Add(ordenCompra);
                    }

                    db.Entry(up).State = EntityState.Modified;
                }

                if (isNewRow)
                {
                    db.OrdenCompras_Web.AddRange(ordenCW);
                }

                db.SaveChanges();

                return Json(new { 
                    Success = true, 
                    value = "", 
                    Message = "Compra agregada correctamente.", 
                    Message_data = "", 
                    Message_Classes = "alert-success", 
                    Message_concat = false 
                });

            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = false,
                    value = "",
                    Message = "Hubo un problema con guardar las compras.",
                    Message_data = "",
                    Message_Classes = "alert-danger",
                    Message_concat = false
                });
            }
        }

        [HttpPost]
        public ActionResult guardarDistribucion(List<OrdenCompra_Web> odc, List<resumenProveedor> usuariosProductos)
        {
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            try
            {

                if (odc == null || usuariosProductos == null)
                {
                    return Json(new
                    {
                        Success = false,
                        value = "",
                        Message = "No se han enviado valores para generar una orden de compra.",
                        Message_data = "",
                        Message_Classes = "warning",
                        Message_concat = false
                    });
                }

                string producto = odc.FirstOrDefault().Producto;
                string[] ids = odc.SelectMany(x => x.REQHDRS.Split('-')).Distinct().ToArray();

                // Obtén los registros de REQHDRs que necesitan ser eliminados
                // Recupera los registros desde la base de datos (sin filtrar por 'ids' todavía)
                var registros = db.OrdenCompras_Web
                                  .Where(x => x.Producto == producto && x.Estatus == "CARRITO")
                                  .ToList();

                // Filtra los registros en memoria usando Split y Contains
                var registrosParaEliminar = registros
                                            .Where(x => x.REQHDRS.Split('-').Intersect(ids).Any())
                                            .ToList();

                /*
                if (registrosParaEliminar != null && registrosParaEliminar.Count == 0)
                {
                    return Json(new
                    {
                        Success = false,
                        value = "",
                        Message = "Ya hay ordenes en proceso para este producto, no puede alterar la selección realizada previamente.",
                        Message_data = "",
                        Message_Classes = "warning",
                        Message_concat = false
                    });
                }
                */

                foreach (var item in usuariosProductos)
                {
                    UsuariosProductos up = new UsuariosProductos();
                    up = db.UsuariosProductos.FirstOrDefault(x => x.Id_Usuario == item.Id_Proveedor && x.Id_REQHDR == item.Id_REQHDR && x.Producto.Descripcion.Trim() == item.Producto);

                    if (up != null)
                    {
                        up.Cantidad_comprada = item.Cantidad_solicitada;
                        db.Entry(up).State = EntityState.Modified;
                    }
                }
                //Actualizo la cantidad solicitada para el proveedor cuyo producto se está solicitando por el comprador, ya que necesito saber cuánto se le está pidiendo y plasmarlo en Resumen

                if (registrosParaEliminar.Count > 0)
                {
                    db.OrdenCompras_Web.RemoveRange(registrosParaEliminar);
                }
                
                db.SaveChanges();

                foreach (var item in odc)
                {
                    OrdenCompra_Web orden = new OrdenCompra_Web();

                    if (db.OrdenCompras_Web.Any(x => x.REQHDRS == item.REQHDRS && x.Proveedor == item.Proveedor && x.Producto == item.Producto))
                    {
                        orden = db.OrdenCompras_Web.FirstOrDefault(x => x.REQHDRS == item.REQHDRS && x.Proveedor == item.Proveedor && x.Producto == item.Producto);
                        orden.Juarez = item.Juarez;
                        orden.Villas = item.Villas;
                        orden.Almaguer = item.Almaguer;
                        orden.Jarachina = item.Jarachina;
                        orden.Balcones = item.Balcones;
                        orden.Cedis = item.Cedis;
                        orden.Guanza = item.Guanza;
                        orden.Ofertas = item.Ofertas;
                        orden.Guanajuato = item.Guanajuato;
                        orden.Cantidad_validada = item.Cantidad_validada;
                        orden.Cantidad_solicitada = recuperarSolicitado(item);
                        orden.Precio = recuperarPrecio(item);
                        db.Entry(orden).State = EntityState.Modified;
                    }
                    else
                    {
                        orden = item;
                        orden.Id_Proveedor_Merksys = db.Usuarios.FirstOrDefault(x => x.Nombre == item.Proveedor).Proveeedor_no_mks;
                        orden.Id_Producto_Merksys = db.Productos.FirstOrDefault(x => x.Descripcion.Trim() == item.Producto).Clave_interna;
                        orden.Creador = db.Usuarios.Find(id).Correo;
                        orden.Cantidad_solicitada = recuperarSolicitado(item);
                        orden.Precio = recuperarPrecio(item);
                        orden.Estatus = "CARRITO";
                        db.OrdenCompras_Web.Add(orden);
                    }

                    db.SaveChanges();
                }
                return Json(new
                {
                    Success = true,
                    value = "",
                    Message = "Orden agregada correctamente.",
                    Message_data = "",
                    Message_Classes = "success",
                    Message_concat = false
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = false,
                    value = "",
                    Message = "Problema en guardar la orden.",
                    Message_data = "",
                    Message_Classes = "danger",
                    Message_concat = false
                });
            }

        }

        [HttpPost]
        public ActionResult cancelarDistribucion(int Id_OrdenCompra)
        {
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            try
            {
                OrdenCompra_Web ordenCompra = new OrdenCompra_Web();
                ordenCompra = db.OrdenCompras_Web.Find(Id_OrdenCompra);
                ordenCompra.Estatus = "CANCELADO";
                db.Entry(ordenCompra).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    Success = true,
                    value = "",
                    Message = "Orden cancelada correctamente.",
                    Message_data = "",
                    Message_Classes = "success",
                    Message_concat = false
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = false,
                    value = "",
                    Message = "Problema al cancelar la orden.",
                    Message_data = "",
                    Message_Classes = "danger",
                    Message_concat = false
                });
            }
        }
        public string recuperarPrecio(OrdenCompra_Web item)
        {
            UsuariosProductos up = new UsuariosProductos();
            string reqhdrsString = item.REQHDRS;  // Suponiendo que item.REQHDRS es el string a convertir
            int[] reqhdrs = reqhdrsString 
                .Split('-')  // Divide el string en subcadenas separadas por '-'
                .Select(int.Parse)  // Convierte cada subcadena en un entero
                .ToArray();  // Convierte el resultado en un arreglo
            up = db.UsuariosProductos.OrderByDescending(x => x.Id_UsuarioProducto).FirstOrDefault(x => x.Producto.Descripcion.Trim() == item.Producto.Trim() && x.Usuario.Nombre.Trim() == item.Proveedor.Trim() && reqhdrs.Contains(x.Id_REQHDR));
            if (up != null)
            {
                return (Math.Round(Convert.ToDecimal(item.Cantidad_validada) * up.Precio, 4)).ToString("F4");
            }
            else
            {
                return "0";
            }
        }

        public string recuperarSolicitado(OrdenCompra_Web item)
        {
            List<string> sucursal = new List<string> { "NA" };

            if (Convert.ToDecimal(item.Juarez) > 0)
            {
                sucursal.Add("JUA");
            }

            if (Convert.ToDecimal(item.Guanza) > 0)
            {
                sucursal.Add("GUA");
            }
            if (Convert.ToDecimal(item.Ofertas) > 0)
            {
                sucursal.Add("OFE");
            }
            if (Convert.ToDecimal(item.Balcones) > 0)
            {
                sucursal.Add("BAL");
            }
            if (Convert.ToDecimal(item.Guanajuato) > 0)
            {
                sucursal.Add("GTO");
            }
            if (Convert.ToDecimal(item.Jarachina) > 0)
            {
                sucursal.Add("JAR");
            }
            if (Convert.ToDecimal(item.Almaguer) > 0)
            {
                sucursal.Add("AMG");
            }

            // Si necesitas convertir la lista de nuevo a un array:
            string[] sucursalArray = sucursal.ToArray();

            string[] idsreqhdrs = item.REQHDRS.Split('-');

            decimal res = db.REQDETs.Any(x => idsreqhdrs.Contains(x.Id_REQHDR.ToString()) && sucursalArray.Contains(x.REQHDR.Sucursal) && x.Descripcion == item.Producto) ? db.REQDETs.Where(x => idsreqhdrs.Contains(x.Id_REQHDR.ToString()) && sucursalArray.Contains(x.REQHDR.Sucursal) && x.Descripcion == item.Producto).Sum(y => y.Cantidad_solicitada) : 0;

            
            return (Math.Round(res).ToString("F4"));
        }

        public OrdenCompra_Web asignarCantidadSucursal(OrdenCompra_Web oc, UsuariosProductos us)
        {
            REQHDR rEQHDR = db.REQHDRs.Find(us.Id_REQHDR);

            switch (rEQHDR.Sucursal)
            {
                case "JUA":
                    oc.Juarez = us.Cantidad_comprada.ToString();
                    break;
                case "GUA":
                    oc.Guanza = us.Cantidad_comprada.ToString();
                    break;
                case "OFE":
                    oc.Ofertas = us.Cantidad_comprada.ToString();
                    break;
                case "BAL":
                    oc.Balcones = us.Cantidad_comprada.ToString();
                    break;
                case "GTO":
                    oc.Guanajuato = us.Cantidad_comprada.ToString();
                    break;
                case "CDI":
                    oc.Cedis = us.Cantidad_comprada.ToString();
                    break;
                case "JAR":
                    oc.Jarachina = us.Cantidad_comprada.ToString();
                    break;
                case "AMG":
                    oc.Almaguer = us.Cantidad_comprada.ToString();
                    break;
                default:
                    break;
            }

            return oc;
        }

        public decimal asignarPrecioSucursal(List<UsuariosProductos> usuariosProductos)
        {
            decimal preJua = 0;
            decimal preGua = 0;
            decimal preOfe = 0;
            decimal preBal = 0;
            decimal preGto = 0;
            decimal preCdi = 0;
            decimal preJar = 0;
            decimal preAmg = 0;

            decimal Total = 0;
            
            foreach (var us in usuariosProductos)
            {
                REQHDR rEQHDR = db.REQHDRs.Find(us.Id_REQHDR);

                switch (rEQHDR.Sucursal)
                {
                    case "JUA":
                        preJua = us.Precio * us.Cantidad_comprada;
                        break;
                    case "GUA":
                        preGua = us.Precio * us.Cantidad_comprada;
                        break;
                    case "OFE":
                        preOfe = us.Precio * us.Cantidad_comprada;
                        break;
                    case "BAL":
                        preBal = us.Precio * us.Cantidad_comprada;
                        break;
                    case "GTO":
                        preGto = us.Precio * us.Cantidad_comprada;
                        break;
                    case "CDI":
                        preCdi = us.Precio * us.Cantidad_comprada;
                        break;
                    case "JAR":
                        preJar = us.Precio * us.Cantidad_comprada;
                        break;
                    case "AMG":
                        preAmg = us.Precio * us.Cantidad_comprada;
                        break;
                    default:
                        break;
                }
            }
            Total = preJua + preGua + preOfe + preBal + preGto + preCdi + preJar + preAmg;
            return Total;
        }

        // POST: REQHDRs/Delete/5
        // Actualiza precio de producto
        [HttpPost]
        public ActionResult guardarPrecio(UsuariosProductos precio, string producto, int[] ids_REQHDRS)
        {
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            try
            {
                Producto productoR = db.Productos.FirstOrDefault( x => x.Descripcion.Trim() == producto && x.Id_Proveedor == id);
                List<UsuariosProductos> usuariosProductos = new List<UsuariosProductos>();
                List<REQDET> reqDETs = db.REQDETs.Where(x => ids_REQHDRS.Contains(x.Id_REQHDR) && x.Descripcion == producto).ToList();

                /*
                List<string> ids_reqhdrs = reqDETs.Select(x => x.Id_REQHDR.ToString()).ToList();
                if (db.OrdenCompras_Web.Where(x => x.REQHDRS.Split('-').Intersect(ids_reqhdrs).Any()).Any())
                {
                    
                }
                */

                bool check = true;
                foreach (var item in reqDETs)
                {
                    DateTime currentDate = DateTime.Now.Date; // Obtener la fecha actual sin la hora
                    DateTime fechaLimiteProveedor = Convert.ToDateTime(item.REQHDR.Fecha_lim_proveedor).Date; // Convertir la fecha límite del proveedor a DateTime sin la hora

                    int diferenciaDias = (fechaLimiteProveedor - currentDate).Days; // Calcular la diferencia en días

                    if (diferenciaDias < 0 && !check)
                    {
                        check = false;
                        break;
                    }
                    else
                    {
                        UsuariosProductos newUP = new UsuariosProductos();

                        newUP.Id_REQHDR = item.Id_REQHDR;
                        newUP.Id_Producto = productoR.Id_Producto;
                        newUP.Precio = precio.Precio;
                        newUP.Id_Usuario = precio.Id_Usuario;

                        usuariosProductos.Add(newUP);
                    }
                }
                
                if (check)
                {
                    db.UsuariosProductos.AddRange(usuariosProductos);
                    db.SaveChanges();
                    return Json(new { Success = true, value = new { precio.Id_Producto, precio.Id_Usuario, precio.Precio}, Message = "Precio agregado correctamente.", Message_data = "", Message_Classes = "success", Message_concat = false });
                }else
                {
                    return Json(new { Success = false, value = "", Message = "Fecha límite para proveedor superada, no se han actualizado los campos.", Message_data = "", Message_Classes = "warning", Message_concat = false });
                }

            }
            catch (Exception)
            {
                return Json(new { Success = false, value = new { precio.Id_Producto, precio.Id_Usuario, precio.Precio }, Message = "Error al registrar precio.", Message_data = "", Message_Classes = "danger", Message_concat = false });
            }
        }

        public ActionResult Consolidation(DateTime startDate, DateTime endDate)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            if (rol == "Eficiencia Operativa")
            {
                endDate = startDate;
            }
            if (startDate == null || endDate == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            endDate = endDate.AddDays(1);

            int[] EstatusIds = rol == "Eficiencia Operativa" ? new int[] { 1, 2 } : new int[] { 1, 2 };
            List<REQHDR> rEQHDRs = db.REQHDRs.Where(x => x.Id_REQHDR_Parent == 0 && (x.Fecha_creacion >= startDate && x.Fecha_creacion < endDate) && EstatusIds.Contains(x.Estatus)).ToList();
            
            int[] req_Array = rEQHDRs.Select(x => x.Id_REQHDR).ToArray();
            
            List<REQDET> rEQDETs = db.REQDETs.Where(x => req_Array.Contains(x.Id_REQHDR)).ToList();
            if (rEQHDRs.Count() == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            int id = Convert.ToInt32(Session["Id_Usuario"]);

            Usuario usuario = db.Usuarios.Find(id);

            var embalajes = db.Embalajes.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<List<REQHDR>, List<REQDET>, Usuario, SelectList>(rEQHDRs, rEQDETs, usuario, new SelectList(embalajes, "Id_Embalaje", "Tipo_Embalaje"));

            ViewBag.Title = "Consolidación de " + startDate.ToString("D");

            return View(modelos);
        }
        public ActionResult DuplicateREQHDR(string ids_REQHDRS)
        {
            try
            {
                string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
                int id = Convert.ToInt32(Session["Id_Usuario"]);
                DateTime creacion = DateTime.Now;
                List<REQHDR> hdrs = new List<REQHDR>();
                foreach (int Id_REQHDR_Parent in ids_REQHDRS.Split('-').Select(x => Convert.ToInt32(x)))
                {
                    REQHDR tempReqHDR = db.REQHDRs.Find(Id_REQHDR_Parent);
                    REQHDR reqHDR = new REQHDR();

                    reqHDR.Estatus = 1;
                    reqHDR.Fecha_creacion = creacion;
                    reqHDR.Fecha_lim_proveedor = tempReqHDR.Fecha_lim_proveedor;
                    reqHDR.Fecha_validacion = creacion;
                    reqHDR.Id_Comprador = id;
                    reqHDR.Id_Creador = tempReqHDR.Id_Creador;
                    reqHDR.Id_REQHDR_Parent = Id_REQHDR_Parent;
                    reqHDR.Id_Validador = tempReqHDR.Id_Validador;
                    reqHDR.Sucursal = tempReqHDR.Sucursal;

                    hdrs.Add(reqHDR);
                }
                db.REQHDRs.AddRange(hdrs);
                db.SaveChanges();
                return Json(new { Success = true, value = ids_REQHDRS, Message = "Nuevos REQHDRs para cotizar generados correctamente.", Message_data = "", Message_Classes = "success", Message_concat = false });
            }
            catch (Exception e)
            {
                return Json(new { Success = false, value = ids_REQHDRS, Message = "Hubo un error al procesar estos REQHDRs.\n" + e.Message.ToString(), Message_data = "", Message_Classes = "danger", Message_concat = false });
            }
        }

        public ActionResult NewsConsolidations(string ids_REQHDRS)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            int[] ids = ids_REQHDRS.Split('-').Select(x => Convert.ToInt32(x)).ToArray();
            if (rol != "Admin" && rol != "Compras")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<REQHDR> rEQHDRs = db.REQHDRs.Where(x => ids.Contains(x.Id_REQHDR)).ToList();

            int[] req_Array = rEQHDRs.Select(x => x.Id_REQHDR).ToArray();

            List<REQDET> rEQDETs = db.REQDETs.Where(x => ids.Contains(x.Id_REQHDR)).ToList();

            if (rEQHDRs.Count() == 0)
            {
                return RedirectToAction("Error", "Home");
            }

            int id = Convert.ToInt32(Session["Id_Usuario"]);

            Usuario usuario = db.Usuarios.Find(id);

            var embalajes = db.Embalajes.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<List<REQHDR>, List<REQDET>, Usuario, SelectList>(rEQHDRs, rEQDETs, usuario, new SelectList(embalajes, "Id_Embalaje", "Tipo_Embalaje"));

            ViewBag.Title = "Consolidación de " + rEQHDRs.OrderByDescending(x => x.Fecha_creacion).FirstOrDefault().Fecha_creacion.ToString("t");

            return View(modelos);
        }
        [HttpPost]
        public ActionResult updateDates(List<REQHDR> REQHDRs)
        {
            try
            {
                foreach (var rh in REQHDRs)
                {
                    REQHDR r = db.REQHDRs.Find(rh.Id_REQHDR);
                    r.Fecha_lim_proveedor = rh.Fecha_lim_proveedor;
                    //r.Fecha_lim_recepcion = rh.Fecha_lim_recepcion;
                    db.Entry(r).State = EntityState.Modified;
                    //db.REQHDRs.Add(r);
                }

                db.SaveChanges();

                return Json(new { Success = true, value = "", Message = "Fechas actualizadas correctamente.", Message_data = "", Message_Classes = "success", Message_concat = false });
            }
            catch (Exception)
            {
                return Json(new { Success = false, value = "", Message = "Error al registrar fechas.", Message_data = "", Message_Classes = "danger", Message_concat = false });
            }
        }

        [HttpPost]
        public ActionResult updateDatesOrden(OrdenCompra_Web orden)
        {
            try
            {
                List<OrdenCompra_Web> ordenes = db.OrdenCompras_Web.Where(x => x.REQHDRS == orden.REQHDRS).ToList();
                foreach (var or in ordenes)
                {
                    or.Fecha_limite = orden.Fecha_limite;
                    db.Entry(or).State = EntityState.Modified;
                }

                db.SaveChanges();

                return Json(new { Success = true, value = "", Message = "Fechas actualizadas correctamente.", Message_data = "", Message_Classes = "success", Message_concat = false });
            }
            catch (Exception)
            {
                return Json(new { Success = false, value = "", Message = "Error al registrar fechas.", Message_data = "", Message_Classes = "danger", Message_concat = false });
            }
        }
        // GET: REQHDRs/Create
        public ActionResult Create()
        {
            ViewBag.Id_Creador = new SelectList(db.Usuarios, "Id_Usuario", "Username");
            ViewBag.Id_Validador = new SelectList(db.Usuarios, "Id_Usuario", "Username");
            return View();
        }

        // POST: REQHDRs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_REQHDR,Sucursal,Fecha_creacion,Id_Creador,Estatus,Fecha_validacion,Id_Validador")] REQHDR rEQHDR)
        {
            if (ModelState.IsValid)
            {
                db.REQHDRs.Add(rEQHDR);
                db.SaveChanges();
            }

            ViewBag.Id_Creador = new SelectList(db.Usuarios, "Id_Usuario", "Username", rEQHDR.Id_Creador);
            ViewBag.Id_Validador = new SelectList(db.Usuarios, "Id_Usuario", "Username", rEQHDR.Id_Validador);
            return View(rEQHDR);
        }

        [HttpPost]
        public ActionResult EditREQHDR(REQHDR rh, List<REQDET> rs, int opt)
        {
            rh = db.REQHDRs.Find(rh.Id_REQHDR);

            int idUser = Convert.ToInt32(Session["Id_Usuario"]);

            Usuario us = db.Usuarios.Find(idUser);
            try
            {

                if (opt == 1)
                {
                    rh.Estatus = 2;
                    rh.Fecha_validacion = DateTime.Now;
                    rh.Id_Validador = us.Id_Usuario;

                    foreach (var rd in rs)
                    {
                        if (db.REQDETs.Any(x => x.Id_REQDET == rd.Id_REQDET))
                        {
                            REQDET rEQDET = new REQDET();
                            rEQDET = db.REQDETs.Find(rd.Id_REQDET);
                            rEQDET.Descripcion = rd.Descripcion == null ? rEQDET.Descripcion : rd.Descripcion;
                            rEQDET.Cantidad_validada = rd.Cantidad_validada;
                            rEQDET.Id_Embalaje_validado = rd.Id_Embalaje_validado;
                            rEQDET.Fecha_validacion = DateTime.Now;
                            rEQDET.Estatus = "2";

                            db.Entry(rEQDET).State = EntityState.Modified;
                        }
                        else
                        {
                            REQDET rEQDET = new REQDET();
                            rEQDET = rd;
                            rEQDET.Fecha_creacion = DateTime.Now;
                            rEQDET.Fecha_validacion = DateTime.Now;
                            rEQDET.Cantidad_solicitada = Convert.ToDecimal(rd.Cantidad_validada);
                            rEQDET.Estatus = "2";
                            db.REQDETs.Add(rEQDET);
                        }
                    }
                }

                if (opt == 0)
                {
                    rh.Estatus = 0;
                    db.Entry(rh).State = EntityState.Modified;
                }

                db.SaveChanges();

                /*
                rh.Sucursal = us.Sucursal;
                rh.Fecha_creacion = DateTime.Now;
                rh.Id_Creador = us.Id_Usuario;
                rh.Estatus = 1;
                db.REQHDRs.Add(rh);
                db.SaveChanges();

                foreach (var item in rs)
                {
                    item.Id_REQHDR = rh.Id_REQHDR;
                    item.Estatus = "1";
                    item.Fecha_creacion = DateTime.Now;
                }
                db.SaveChanges();

                db.REQDETs.AddRange(rs);
                */
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception)
            {

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult ValidarREQHDR(List<REQHDR> rhs, int opt)
        {
            int idUser = Convert.ToInt32(Session["Id_Usuario"]);

            Usuario us = db.Usuarios.Find(idUser);
            
            try
            {
                foreach (var rh in rhs)
                {
                    REQHDR rh_item = db.REQHDRs.Find(rh.Id_REQHDR);
                
                    if (opt == 1)
                    {
                        rh_item.Estatus = 2;
                        rh_item.Fecha_validacion = DateTime.Now;
                        rh_item.Id_Validador = us.Id_Usuario;
                    }

                    if (opt == 0)
                    {
                        rh_item.Estatus = 0;
                    }
                    db.Entry(rh_item).State = EntityState.Modified;

                    db.SaveChanges();

                }

                
                return Json(new { Success = true, value = "", Message = (opt == 1 ? "Aprobados correctamente." : "Cancelados correctamente."), Message_data = "", Message_Classes = (opt == 1 ? " text-success fs-3 fw-bold" : " text-secondary fs-3 fw-bold"), Message_concat = false });
            }
            catch (Exception)
            {
                return Json(new { Success = false, value = "", Message = "Error al validar.", Message_data = "", Message_Classes = " text-danger fs-4 fw-bold", Message_concat = false });
            }

        }
        // GET: REQHDRs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQHDR rEQHDR = db.REQHDRs.Find(id);
            if (rEQHDR == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Creador = new SelectList(db.Usuarios, "Id_Usuario", "Username", rEQHDR.Id_Creador);
            ViewBag.Id_Validador = new SelectList(db.Usuarios, "Id_Usuario", "Username", rEQHDR.Id_Validador);
            return View(rEQHDR);
        }

        // POST: REQHDRs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_REQHDR,Sucursal,Fecha_creacion,Id_Creador,Estatus,Fecha_validacion,Id_Validador")] REQHDR rEQHDR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rEQHDR).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.Id_Creador = new SelectList(db.Usuarios, "Id_Usuario", "Username", rEQHDR.Id_Creador);
            ViewBag.Id_Validador = new SelectList(db.Usuarios, "Id_Usuario", "Username", rEQHDR.Id_Validador);
            return View(rEQHDR);
        }

        // GET: REQHDRs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQHDR rEQHDR = db.REQHDRs.Find(id);
            if (rEQHDR == null)
            {
                return HttpNotFound();
            }
            return View(rEQHDR);
        }

        // POST: REQHDRs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            REQHDR rEQHDR = db.REQHDRs.Find(id);
            db.REQHDRs.Remove(rEQHDR);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public class resumenProveedor
        {
            public resumenProveedor()
            {
                Id_REQHDR = 0;
                Id_Proveedor = 0;
                Proveedor = "NA";
                Cantidad_disponible = 0;
                Cantidad_solicitada = 0;
                Producto = "NA";
            }
            public int id { get; set; }
            public int Id_REQHDR { get; set; }
            public int Id_Proveedor { get; set; }
            public string Proveedor { get; set; }
            public decimal Cantidad_disponible { get; set; }
            public decimal Cantidad_solicitada { get; set; }
            public string Producto { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult> getOrderSearchListAPI(string urlAPI)
        {
            PublicController publicController = new PublicController();
            var result = await publicController.ObtenerDatosDeAPI(urlAPI);

            if (result is JsonResult jsonResult)
            {
                return Json(jsonResult.Data, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(500, "Error al obtener datos de la API desde PublicController.");
        }
    }
}
