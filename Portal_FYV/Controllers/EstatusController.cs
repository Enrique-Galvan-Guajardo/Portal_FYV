using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Portal_FYV.Models;

namespace Portal_FYV.Controllers
{
    public class EstatusController : Controller
    {
        private db_model db = new db_model();

        // GET: Estatus
        public ActionResult Index()
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            if (rol != "Admin+" && rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.Estatus.ToList());
        }

        // GET: Estatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estatus estatus = db.Estatus.Find(id);
            if (estatus == null)
            {
                return HttpNotFound();
            }
            return View(estatus);
        }

        // GET: Estatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Estatus/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Estatus,Name")] Estatus estatus)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            if (ModelState.IsValid)
            {
                db.Estatus.Add(estatus);
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Registro de estatus creado.", Message_data = "", Message_Classes = "success", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de estatus incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(estatus);
            }
        }

        // GET: Estatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estatus estatus = db.Estatus.Find(id);
            if (estatus == null)
            {
                return HttpNotFound();
            }
            return View(estatus);
        }

        // POST: Estatus/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Estatus,Name")] Estatus estatus)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            if (ModelState.IsValid)
            {
                db.Entry(estatus).State = EntityState.Modified;
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Registro de estatus actualizado.", Message_data = "", Message_Classes = "primary", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de estatus incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(estatus);
            }
        }

        // GET: Estatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estatus estatus = db.Estatus.Find(id);
            if (estatus == null)
            {
                return HttpNotFound();
            }
            return View(estatus);
        }

        // POST: Estatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estatus estatus = db.Estatus.Find(id);
            db.Estatus.Remove(estatus);
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
