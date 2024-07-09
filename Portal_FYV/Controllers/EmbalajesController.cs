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
    public class EmbalajesController : Controller
    {
        private db_model db = new db_model();

        // GET: Embalajes
        public ActionResult Index()
        {
            return View(db.Embalajes.OrderByDescending(x => x.Tipo_Embalaje).ToList());
        }

        // GET: Embalajes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Embalaje embalaje = db.Embalajes.Find(id);
            if (embalaje == null)
            {
                return HttpNotFound();
            }
            return View(embalaje);
        }

        // GET: Embalajes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Embalajes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Embalaje,Tipo_Embalaje,valor_um,um")] Embalaje embalaje)
        {
            if (ModelState.IsValid)
            {
                db.Embalajes.Add(embalaje);
                db.SaveChanges();

                return Json(new { Success = true, Message = "Registro de embalaje creado.", Message_data = "", Message_Classes = "success", Message_concat = false });
            }
            else
            {
                return Json(new { Success = false, Message = "Información de producto incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
        }

        // GET: Embalajes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Embalaje embalaje = db.Embalajes.Find(id);
            if (embalaje == null)
            {
                return HttpNotFound();
            }
            return View(embalaje);
        }

        // POST: Embalajes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Embalaje,Tipo_Embalaje,valor_um,um")] Embalaje embalaje)
        {
            if (ModelState.IsValid)
            {
                db.Entry(embalaje).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Success = true, Message = "Registro de embalaje modificado.", Message_data = "", Message_Classes = "primary", Message_concat = false });
            }
            else
            {
                return Json(new { Success = false, Message = "Información de producto incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
        }

        // GET: Embalajes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Embalaje embalaje = db.Embalajes.Find(id);
            if (embalaje == null)
            {
                return HttpNotFound();
            }
            return View(embalaje);
        }

        // POST: Embalajes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Embalaje embalaje = db.Embalajes.Find(id);
            db.Embalajes.Remove(embalaje);
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
