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

            // Cargar la lista de embalajes
            var embalajes = db.Embalajes.ToList();

            // Crear un Tuple que contenga ambos modelos y la lista de embalajes
            var modelos = new Tuple<REQHDR, List<REQDET>, SelectList>(rEQHDR, rEQDETs, new SelectList(embalajes, "Id_Embalaje", "Tipo_Embalaje"));
            
            ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje");

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
