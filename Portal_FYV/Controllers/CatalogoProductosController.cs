using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using Portal_FYV.Models;

namespace Portal_FYV.Controllers
{
    public class CatalogoProductosController : Controller
    {
        private db_model db = new db_model();

        // GET: CatalogoProductos
        public ActionResult Index()
        {
            return View(db.CatalogoProductos.ToList());
        }

        // GET: CatalogoProductos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatalogoProducto catalogoProducto = db.CatalogoProductos.Find(id);
            if (catalogoProducto == null)
            {
                return HttpNotFound();
            }
            return View(catalogoProducto);
        }

        // GET: CatalogoProductos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatalogoProductos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Catalogo,Clave_numero,Codigo_barras,Descripcion,Linea,Sublinea,Familia,Subfamilia,Estatus,Unidades")] CatalogoProducto catalogoProducto)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            if (ModelState.IsValid)
            {
                db.CatalogoProductos.Add(catalogoProducto);
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Registro de catalogo creado.", Message_data = "", Message_Classes = "success", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de catalogo incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(catalogoProducto);
            }
        }

        // GET: CatalogoProductos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatalogoProducto catalogoProducto = db.CatalogoProductos.Find(id);
            if (catalogoProducto == null)
            {
                return HttpNotFound();
            }
            return View(catalogoProducto);
        }

        // POST: CatalogoProductos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Catalogo,Clave_numero,Codigo_barras,Descripcion,Linea,Sublinea,Familia,Subfamilia,Estatus,Unidades")] CatalogoProducto catalogoProducto)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            if (ModelState.IsValid)
            {
                db.Entry(catalogoProducto).State = EntityState.Modified;
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Registro de catalogo actualizada.", Message_data = "", Message_Classes = "primary", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de catalogo incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(catalogoProducto);
            }
        }

        // GET: CatalogoProductos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatalogoProducto catalogoProducto = db.CatalogoProductos.Find(id);
            if (catalogoProducto == null)
            {
                return HttpNotFound();
            }
            return View(catalogoProducto);
        }

        // POST: CatalogoProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CatalogoProducto catalogoProducto = db.CatalogoProductos.Find(id);
            db.CatalogoProductos.Remove(catalogoProducto);
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
