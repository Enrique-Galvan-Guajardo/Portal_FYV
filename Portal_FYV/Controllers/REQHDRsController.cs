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
            if (rEQHDR == null)
            {
                return HttpNotFound();
            }
            return View(rEQHDR);
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
