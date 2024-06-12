using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Portal_FYV.Models;

namespace Portal_FYV.Controllers
{
    public class REQHDRsController : Controller
    {
        private db_model db = new db_model();

        // GET: REQHDRs
        public ActionResult Index()
        {
            var rEQHDRs = db.REQHDRs.Include(r => r.Usuario).Include(r => r.Usuario1);
            return View(rEQHDRs.ToList());
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
            int[] selectedIds = db.REQHDRs
                .Where(x => DbFunctions.TruncateTime(x.Fecha_validacion) == searchDateOnly)
                .Select(x => x.Id_REQHDR)
                .ToArray();

            List<CatalogoProducto> catalogo = new List<CatalogoProducto>();
            List<UsuariosProductos> usuariosProductos = new List<UsuariosProductos>();

            var modelos = new object();

            switch (rol)
            {
                case "Admin+":
                case "Admin":
                case "Compras":

                    if (selectedIds == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    rEQHDRs = db.REQHDRs.Where(x => selectedIds.Contains(x.Id_REQHDR)).ToList();

                    rEQDETs = db.REQDETs.Where(x => selectedIds.Contains(x.Id_REQHDR)).ToList();

                    descripciones = rEQDETs.Select(x => x.Descripcion).ToArray();

                    productos = db.Productos.Where(x => descripciones.Contains(x.Descripcion)).ToList();

                    ids_productos = productos.Select(x => x.Id_Producto).ToArray();

                    catalogo = db.CatalogoProductos.Where(x => descripciones.Contains(x.Descripcion.Trim())).ToList();

                    if (rEQDETs == null)
                    {
                        return HttpNotFound();
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
                    if (selectedIds == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    rEQHDRs = db.REQHDRs.Where(x => selectedIds.Contains(x.Id_REQHDR)).ToList();

                    productos = db.Productos.Where(x => x.Id_Proveedor == usuario.Id_Usuario).ToList();

                    descripciones = productos.Select(x => x.Descripcion).ToArray();
                    ids_productos = productos.Select(x => x.Id_Producto).ToArray();

                    catalogo = db.CatalogoProductos.Where(x => descripciones.Contains(x.Descripcion)).ToList();

                    foreach (var r in rEQHDRs)
                    {
                        rEQDETs.AddRange(r.REQDETs.Where(x => selectedIds.Contains(x.Id_REQHDR) && descripciones.Contains(x.Descripcion)).ToList());
                    }

                    if (rEQDETs == null)
                    {
                        return HttpNotFound();
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

        public ActionResult Compra(string REQHDRS)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            string sucursal = Session["Sucursal"] != null ? Session["Sucursal"].ToString() : "";
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            var modelos = new object();

            try
            {
                Usuario usuario = db.Usuarios.Find(id);

                List<OrdenCompra_Web> ordenCompra_Web = new List<OrdenCompra_Web>();

                ordenCompra_Web = db.OrdenCompras_Web.Where(x => x.REQHDRS == REQHDRS).ToList();

                var ids_reqhdrs = REQHDRS.Split('-');
                if (ordenCompra_Web != null)
                {
                    modelos = new Tuple<List<OrdenCompra_Web>, List<REQHDR>>(ordenCompra_Web, db.REQHDRs.Where(x => ids_reqhdrs.Contains(x.Id_REQHDR.ToString())).ToList());
                    return View(modelos);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
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
        public ActionResult guardarDistribucion(List<OrdenCompra_Web> proveedores)
        {
            try
            {
                foreach (var item in proveedores)
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
                        orden.Creador = db.Usuarios.FirstOrDefault(x => x.Nombre == item.Proveedor).Correo;
                        orden.Precio = recuperarPrecio(item);
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

        public string recuperarPrecio(OrdenCompra_Web item)
        {
            UsuariosProductos up = new UsuariosProductos();
            string reqhdrsString = item.REQHDRS;  // Suponiendo que item.REQHDRS es el string a convertir
            int[] reqhdrs = reqhdrsString
                .Split('-')  // Divide el string en subcadenas separadas por '-'
                .Select(int.Parse)  // Convierte cada subcadena en un entero
                .ToArray();  // Convierte el resultado en un arreglo
            up = db.UsuariosProductos.FirstOrDefault(x => x.Producto.Descripcion == item.Producto && x.Usuario.Nombre == item.Proveedor && reqhdrs.Contains(x.Id_REQHDR));
            return (Math.Round(Convert.ToDecimal(item.Cantidad_validada) * up.Precio, 4)).ToString("F4");
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
        [HttpPost]
        public ActionResult guardarPrecio(UsuariosProductos precio, string producto, int[] ids_REQHDRS)
        {
            try
            {
                Producto productoR = db.Productos.FirstOrDefault( x => x.Descripcion == producto);
                List<UsuariosProductos> usuariosProductos = new List<UsuariosProductos>();
                
                bool check = true;
                foreach (var item in db.REQDETs.Where(x => ids_REQHDRS.Contains(x.Id_REQHDR) && x.Descripcion == producto))
                {
                    
                    if (Convert.ToDateTime(DateTime.Now).Date > Convert.ToDateTime(item.REQHDR.Fecha_lim_proveedor).Date && check)
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
            if (startDate == null || endDate == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            endDate = endDate.AddDays(1);
            List<REQHDR> rEQHDRs = db.REQHDRs.Where(x => x.Fecha_creacion >= startDate && x.Fecha_creacion < endDate).ToList();
            
            int[] req_Array = rEQHDRs.Select(x => x.Id_REQHDR).ToArray();
            
            List<REQDET> rEQDETs = db.REQDETs.Where(x => req_Array.Contains(x.Id_REQHDR)).ToList();

            if (rEQHDRs.Count() == 0 || rEQHDRs.Count() == rEQHDRs.Where(x => x.Estatus == 2).Count())
            {
                return HttpNotFound();
            }

            int id = Convert.ToInt32(Session["Id_Usuario"]);

            Usuario usuario = db.Usuarios.Find(id);

            var embalajes = db.Embalajes.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<List<REQHDR>, List<REQDET>, Usuario, SelectList>(rEQHDRs, rEQDETs, usuario, new SelectList(embalajes, "Id_Embalaje", "Tipo_Embalaje"));

            ViewBag.Title = "Consolidación de " + startDate.ToString("D");

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
    }
}
