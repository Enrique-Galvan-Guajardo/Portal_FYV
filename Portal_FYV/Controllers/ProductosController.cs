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
    public class ProductosController : Controller
    {
        private db_model db = new db_model();

        // GET: Productos
        public ActionResult Index()
        {
            int idProv = Session["Id_Usuario"] != null ? Convert.ToInt32(Session["Id_Usuario"]) : 0;
            // Cargar la lista de embalajes
            if (idProv == 0)
            {
                return View(db.Productos.ToList());
            }
            else
            {
                return View(db.Productos.Where(x => x.Id_Proveedor == idProv).ToList());
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
            var catalogoProductos = catalogo.Where(x => !productos.Any(y => y.Descripcion == x.Descripcion && y.Id_Proveedor == idProv)).ToList();

            //var precioProductos = db.CatalogoProductos.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<Producto, SelectList, SelectList>( new Producto(), new SelectList(catalogoProductos, "Descripcion", "Descripcion"), new SelectList(db.Embalajes, "Tipo_Embalaje", "Tipo_Embalaje"));

            return View(modelos);
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Producto,Nombre,Descripcion,Clave_externa,Clave_interna," +
            "Codigo_barras,Imagen_ruta,Embalaje,Unidad,Fecha_Creacion,Fecha_Modificacion,Id_Proveedor")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Productos.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
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

            if (ModelState.IsValid)
            {
                producto.Fecha_Modificacion = DateTime.Now;
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(producto);
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
