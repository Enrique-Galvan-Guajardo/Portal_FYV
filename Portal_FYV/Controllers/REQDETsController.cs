using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Portal_FYV.Models;

namespace Portal_FYV.Controllers
{
    public class REQDETsController : Controller
    {
        private db_model db = new db_model();

        // GET: REQDETs
        public ActionResult Index()
        {
            var rEQDETs = db.REQDETs.Include(r => r.REQHDR).Include(r => r.Embalaje);
            return View(rEQDETs.ToList());
        }

        // GET: REQDETs
        public ActionResult CapturarDetalles()
        {
            ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje");
            return View();
        }

        

        // GET: REQDETs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQDET rEQDET = db.REQDETs.Find(id);
            if (rEQDET == null)
            {
                return HttpNotFound();
            }
            return View(rEQDET);
        }

        // GET: REQDETs/Create
        public ActionResult Create()
        {
            ViewBag.Id_REQHDR = new SelectList(db.REQHDRs, "Id_REQHDR", "Sucursal");
            ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje");
            return View();
        }

        // POST: REQDETs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_REQDET,Id_REQHDR,Clave_articulo,Descripcion,Cantidad_solicitada,Fecha_creacion,Id_Embalaje,Cantidad_validada,Fecha_validacion,Id_Embalaje_validado,Fecha_ultima_compra,Cantidad_ultima_compra,Ventas_ultima_semana,Existencia,Estatus")] REQDET rEQDET)
        {
            if (ModelState.IsValid)
            {
                db.REQDETs.Add(rEQDET);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_REQHDR = new SelectList(db.REQHDRs, "Id_REQHDR", "Sucursal", rEQDET.Id_REQHDR);
            ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje", rEQDET.Id_Embalaje);
            return View(rEQDET);
        }

        [HttpPost]
        public ActionResult CreateREQDETS(List<REQDET> rs)
        {
            Usuario us = db.Usuarios.Find(Convert.ToInt32(Session["Id_Usuario"]));
            try
            {
                REQHDR rh = new REQHDR();

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

                db.REQDETs.AddRange(rs);
                db.SaveChanges();

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception)
            {

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: REQDETs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQDET rEQDET = db.REQDETs.Find(id);
            if (rEQDET == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_REQHDR = new SelectList(db.REQHDRs, "Id_REQHDR", "Sucursal", rEQDET.Id_REQHDR);
            ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje", rEQDET.Id_Embalaje);
            return View(rEQDET);
        }

        // POST: REQDETs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_REQDET,Id_REQHDR,Clave_articulo,Descripcion,Cantidad_solicitada,Fecha_creacion,Id_Embalaje,Cantidad_validada,Fecha_validacion,Id_Embalaje_validado,Fecha_ultima_compra,Cantidad_ultima_compra,Ventas_ultima_semana,Existencia,Estatus")] REQDET rEQDET)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rEQDET).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_REQHDR = new SelectList(db.REQHDRs, "Id_REQHDR", "Sucursal", rEQDET.Id_REQHDR);
            ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje", rEQDET.Id_Embalaje);
            return View(rEQDET);
        }

        // GET: REQDETs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQDET rEQDET = db.REQDETs.Find(id);
            if (rEQDET == null)
            {
                return HttpNotFound();
            }
            return View(rEQDET);
        }

        // POST: REQDETs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            REQDET rEQDET = db.REQDETs.Find(id);
            db.REQDETs.Remove(rEQDET);
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
