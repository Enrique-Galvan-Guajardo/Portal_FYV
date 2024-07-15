using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Portal_FYV.Models;

namespace Portal_FYV.Controllers
{
    public class ProductosController : Controller
    {
        private db_model db = new db_model();

        // GET: Productos
        public ActionResult Index()
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            if (rol != "Admin+" && rol != "Admin" && rol != "Proveedores")
            {
                return RedirectToAction("Index", "Home");
            }
            int idProv = Session["Id_Usuario"] != null ? Convert.ToInt32(Session["Id_Usuario"]) : 0;
            // Cargar la lista de embalajes
            if (idProv == 0)
            {
                return View(db.Productos.OrderByDescending(x => x.Descripcion).ToList());
            }
            else
            {
                return View(db.Productos.Where(x => x.Id_Proveedor == idProv).OrderByDescending(x => x.Descripcion).ToList());
            }
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            int idProv = Session["Id_Usuario"] != null ? Convert.ToInt32(Session["Id_Usuario"]) : 0;
            // Cargar la lista de embalajes
            var catalogo = db.CatalogoProductos.ToList();
            var productos = db.Productos.ToList();
            var catalogoProductos = catalogo.Where(x => !productos.Any(y => y.Descripcion == x.Descripcion && y.Id_Proveedor == idProv)).OrderBy(x => x.Descripcion).ToList();

            //var precioProductos = db.CatalogoProductos.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<Producto, SelectList, SelectList>( new Producto(), new SelectList(catalogoProductos, "Descripcion", "Descripcion"), new SelectList(db.Embalajes.OrderBy(x => x.Tipo_Embalaje), "Tipo_Embalaje", "Tipo_Embalaje"));

            return View(modelos);
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Producto,Nombre,Descripcion,Clave_externa,Clave_interna," +
            "Codigo_barras,Imagen_ruta,Embalaje,Unidad,Fecha_Creacion,Fecha_Modificacion,Id_Proveedor,Stock")] Producto producto)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            if (ModelState.IsValid)
            {
                db.Productos.Add(producto);
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Registro de producto creado.", Message_data = "", Message_Classes = "success", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de producto incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(producto);
            }
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            // Cargar la lista de embalajes
            var catalogoProductos = db.CatalogoProductos.ToList();
            //var precioProductos = db.CatalogoProductos.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<Producto, SelectList, SelectList>(producto, new SelectList(catalogoProductos, "Descripcion", "Descripcion"), new SelectList(db.Embalajes, "Tipo_Embalaje", "Tipo_Embalaje"));

            return View(modelos);

            //return View(producto);
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Producto,Nombre,Descripcion,Clave_externa,Clave_interna," +
            "Codigo_barras,Imagen_ruta,Embalaje,Unidad,Fecha_Creacion,Fecha_Modificacion,Id_Proveedor")] Producto producto)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            if (ModelState.IsValid)
            {
                producto.Fecha_Modificacion = DateTime.Now;
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Registro de producto actualizado.", Message_data = "", Message_Classes = "primary", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de producto incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(producto);
            }
        }

        [HttpPost]
        [Route("Productos/actualizarStock")]
        public ActionResult actualizarStock(Producto producto)
        {
            try
            {
                Producto p = db.Productos.Find(producto.Id_Producto);
                p.Stock = producto.Stock;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Success = true, Message = "Stock de producto actualizado.", Message_data = "", Message_Classes = "primary", Message_concat = false });
            }
            catch (Exception)
            {

                return Json(new { Success = false, Message = "Error al actualizar stock de producto.", Message_data = "", Message_Classes = "danger", Message_concat = false });
            }
        }

        // GET: Productos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Productos.Find(id);
            db.Productos.Remove(producto);
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
