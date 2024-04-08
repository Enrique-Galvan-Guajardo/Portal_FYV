using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
        public ActionResult Resumen(List<int> selectedIds)
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

                    foreach (var r in ids_proveedores)
                    {
                        usuariosProductos.Add(db.UsuariosProductos.Where(x => selectedIds.Contains(x.Id_REQHDR) && ids_productos.Contains(x.Id_Producto) && x.Id_Usuario == r).OrderByDescending(x => x.Id_UsuarioProducto).FirstOrDefault());
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

                    usuariosProductos = db.UsuariosProductos.Where(x => ids_productos.Contains(x.Id_Producto) && selectedIds.Contains(x.Id_REQHDR) && x.Id_Usuario == usuario.Id_Usuario).ToList();

                    // Crear un Tuple que contenga ambos modelos y la lista de embalajes
                    modelos = new Tuple<List<REQHDR>, List<REQDET>, List<UsuariosProductos>, List<CatalogoProducto>>(rEQHDRs, rEQDETs, usuariosProductos, catalogo);

                    return View(modelos);
                default:
                    return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpPost]
        public ActionResult distribuirCompras(List<UsuariosProductos> usuariosProductos)
        {
            try
            {
                List<OrdenCompra_Web> ordenCW = new List<OrdenCompra_Web>();
                
                foreach (var item in usuariosProductos)
                {
                    OrdenCompra_Web ordenCompra = new OrdenCompra_Web();
                    
                    UsuariosProductos up = db.UsuariosProductos.Find(item.Id_UsuarioProducto);
                    up.Cantidad_comprada = item.Cantidad_comprada;

                    ordenCompra.num_orden = up.Id_REQHDR.ToString();
                    ordenCompra.cve_art = up.Producto.Clave_externa;
                    ordenCompra.suc = db.REQHDRs.Find(up.Id_REQHDR).Sucursal;
                    ordenCompra.cant = up.Cantidad_comprada;
                    ordenCompra.prv = up.Id_Usuario.ToString();
                    ordenCompra.fecha = DateTime.Now;
                    ordenCompra.estatus = "0";
                    ordenCompra.tipo = "MULTIPLE";

                    ordenCW.Add(ordenCompra);

                    db.Entry(up).State = EntityState.Modified;
                }

                db.OrdenCompras_Web.AddRange(ordenCW);

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

        // POST: REQHDRs/Delete/5
        [HttpPost]
        public ActionResult guardarPrecio(UsuariosProductos precio, string producto, int[] ids_REQHDRS)
        {
            try
            {
                Producto productoR = db.Productos.FirstOrDefault( x => x.Descripcion == producto);
                precio.Id_Producto = productoR.Id_Producto;

                foreach (var item in db.REQDETs.Where(x => ids_REQHDRS.Contains(x.Id_REQHDR) && x.Descripcion == producto))
                {
                    precio.Id_REQHDR = item.Id_REQHDR;
                    db.UsuariosProductos.Add(precio);
                }

                db.SaveChanges();

                return Json(new { Success = true, value = new { precio.Id_Producto, precio.Id_Usuario, precio.Precio}, Message = "Precio agregado correctamente.", Message_data = "", Message_Classes = "alert-success", Message_concat = false });
            }
            catch (Exception)
            {
                return Json(new { Success = false, value = new { precio.Id_Producto, precio.Id_Usuario, precio.Precio }, Message = "Error al registrar precio.", Message_data = "", Message_Classes = "alert-danger", Message_concat = false });
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

            if (rEQHDRs.Count() == 0)
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
