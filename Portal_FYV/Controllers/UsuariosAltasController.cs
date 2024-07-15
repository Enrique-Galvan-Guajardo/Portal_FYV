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
    public class UsuariosAltasController : Controller
    {
        private db_model db = new db_model();

        // GET: UsuariosAltas
        public ActionResult Index()
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            if (rol != "Admin+" && rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var usuariosAltas = db.UsuariosAltas.Include(u => u.Usuario).Include(u => u.Usuario1);
            return View(usuariosAltas.ToList());
        }

        // GET: UsuariosAltas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuariosAltas usuariosAltas = db.UsuariosAltas.Find(id);
            if (usuariosAltas == null)
            {
                return HttpNotFound();
            }
            return View(usuariosAltas);
        }

        // GET: UsuariosAltas/Create
        public ActionResult Create()
        {
            ViewBag.Id_UsuarioRegistra = new SelectList(db.Usuarios, "Id_Usuario", "Username");
            ViewBag.Id_UsuarioAprueba = new SelectList(db.Usuarios, "Id_Usuario", "Username");
            return View();
        }

        // POST: UsuariosAltas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_UsuarioAlta,Id_UsuarioRegistra,Id_UsuarioAprueba,Fecha")] UsuariosAltas usuariosAltas)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            if (ModelState.IsValid)
            {
                db.UsuariosAltas.Add(usuariosAltas);
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Alta de usuario creada.", Message_data = "", Message_Classes = "success", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Id_UsuarioRegistra = new SelectList(db.Usuarios, "Id_Usuario", "Username", usuariosAltas.Id_UsuarioRegistra);
            ViewBag.Id_UsuarioAprueba = new SelectList(db.Usuarios, "Id_Usuario", "Username", usuariosAltas.Id_UsuarioAprueba);
            
            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de validación incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(usuariosAltas);
            }
        }

        // GET: UsuariosAltas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuariosAltas usuariosAltas = db.UsuariosAltas.Find(id);
            if (usuariosAltas == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_UsuarioRegistra = new SelectList(db.Usuarios, "Id_Usuario", "Username", usuariosAltas.Id_UsuarioRegistra);
            ViewBag.Id_UsuarioAprueba = new SelectList(db.Usuarios, "Id_Usuario", "Username", usuariosAltas.Id_UsuarioAprueba);
            return View(usuariosAltas);
        }

        // POST: UsuariosAltas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_UsuarioAlta,Id_UsuarioRegistra,Id_UsuarioAprueba,Fecha")] UsuariosAltas usuariosAltas)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            if (ModelState.IsValid)
            {
                db.Entry(usuariosAltas).State = EntityState.Modified;
                db.SaveChanges();
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Alta de usuario actualizada.", Message_data = "", Message_Classes = "primary", Message_concat = false });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Id_UsuarioRegistra = new SelectList(db.Usuarios, "Id_Usuario", "Username", usuariosAltas.Id_UsuarioRegistra);
            ViewBag.Id_UsuarioAprueba = new SelectList(db.Usuarios, "Id_Usuario", "Username", usuariosAltas.Id_UsuarioAprueba);

            if (rol == "Admin+")
            {
                return Json(new { Success = false, Message = "Información de validación incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
            }
            else
            {
                return View(usuariosAltas);
            }
        }

        // GET: UsuariosAltas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuariosAltas usuariosAltas = db.UsuariosAltas.Find(id);
            if (usuariosAltas == null)
            {
                return HttpNotFound();
            }
            return View(usuariosAltas);
        }

        // POST: UsuariosAltas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuariosAltas usuariosAltas = db.UsuariosAltas.Find(id);
            db.UsuariosAltas.Remove(usuariosAltas);
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
