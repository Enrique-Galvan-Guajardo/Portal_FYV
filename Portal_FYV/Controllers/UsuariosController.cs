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
    public class UsuariosController : Controller
    {
        private db_model db = new db_model();

        // GET: Usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios.Include(u => u.Estatus).Include(u => u.Roles);
            return View(usuarios.ToList());
        }
        
        // POST: Usuarios/logIn
        [HttpPost]
        public ActionResult logIn(string user_session, string user_password)
        {
            if (db.Usuarios.Any(x => (x.Username == user_session || x.Correo == user_session) && x.Contrasena == user_password))
            {
                Usuario usuario = db.Usuarios.FirstOrDefault(x => (x.Username == user_session || x.Correo == user_session) && x.Contrasena == user_password);
                
                Session["Id_Usuario"] = usuario.Id_Usuario;
                Session["Sucursal"] = usuario.Sucursal;
                Session["Rol"] = usuario.Roles.Rol;

                if (usuario.Id_Rol == 3)
                {
                    return RedirectToAction("CapturarDetalles", "REQDETs");
                }
                
            }
            return RedirectToAction("Index", "Home");
        }
        
        // GET: Usuarios/LogOut
        [HttpGet]
        [Route("/Usuarios/logout")]
        public ActionResult LogOut()
        {
            //Terminamos la sesión
            Session.Abandon();
            //Redirigimos al Login
            return RedirectToAction("Index", "Home", new { Message = "Sesión finalizada.", Message_Classes = "warning" });
        }

        // POST: Usuarios/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount([Bind(Include = "Contrasena,Correo")] Usuario usuario)
        {
            if (!db.Usuarios.Any(x => x.Nombre == usuario.Correo || x.Correo == usuario.Correo) && ModelState.IsValid)
            {
                usuario.Username = usuario.Correo.Split('@')[0];
                db.Usuarios.Add(usuario);
                db.SaveChanges();

                //Session["Id_Usuario"] = usuario.Id_Usuario;
                return RedirectToAction("Index", "Home", new { Message = "Usuario creado exitosamente, ya puedes iniciar sesión.", Message_Classes = "success" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Usuario/correo ya existente.", Message_Classes = "warning" });
            }
        }
        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            Usuario usuario = new Usuario();

            ViewBag.Id_Estatus = new SelectList(db.Estatus, "Id_Estatus", "Name");
            ViewBag.Id_Rol = new SelectList(db.Roles, "Id_Rol", "Rol");
            return View(usuario);
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Usuario,Username,Nombre,Correo,Contrasena,Sucursal,Id_Estatus,Fecha_Aprov,Id_Rol,Razon_social,RFC,Pais,Estado,Ciudad,Colonia,Localidad,Codigo_postal,Calle,Numero,Proveeedor_no_mks,Contacto_nombre1,Contacto_nombre2,Contacto_nombre3,Contacto_correo1,Contacto_correo2,Contacto_correo3,Contacto_tel1,Contacto_tel2,Contacto_tel3,Prorroga")] Usuario usuario)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            try
            {
                if (!db.Usuarios.Any(x => (x.Nombre == usuario.Correo || x.Correo == usuario.Correo) && x.Contrasena == usuario.Contrasena) && ModelState.IsValid)
                {
                    if (usuario.Id_Estatus == 2)
                    {
                        UsuariosAltas ua = new UsuariosAltas();
                        ua.Id_UsuarioRegistra = usuario.Id_Usuario;
                        ua.Id_UsuarioAprueba = Convert.ToInt32(Session["Id_Usuario"]);
                        ua.Fecha = DateTime.Now;
                        usuario.Fecha_Aprov = ua.Fecha;
                        db.UsuariosAltas.Add(ua);
                    }

                    usuario.Id_Rol = Session["Id_Usuario"] == null ? 5 : usuario.Id_Rol;

                    db.Usuarios.Add(usuario);
                    db.SaveChanges();

                    if (rol == "Admin+")
                    {
                        return Json(new { Success = true, Message = "Usuario creado.", Message_data = "", Message_Classes = "success", Message_concat = false });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { Message = "Usuario creado exitosamente, ya puedes iniciar sesión.", Message_Classes = "success" });
                    }

                }

                ViewBag.Id_Estatus = new SelectList(db.Estatus, "Id_Estatus", "Name", usuario.Id_Estatus);
                ViewBag.Id_Rol = new SelectList(db.Roles, "Id_Rol", "Rol", usuario.Id_Rol);
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Información de usuario incompleta o ya existente.", Message_data = "", Message_Classes = "warning", Message_concat = false });
                }
                else
                {
                    return View(usuario);
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Error al procesar información de usuario.", Message_data = "", Message_Classes = "danger", Message_concat = false });
            }
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Estatus = new SelectList(db.Estatus, "Id_Estatus", "Name", usuario.Id_Estatus);
            ViewBag.Id_Rol = new SelectList(db.Roles, "Id_Rol", "Rol", usuario.Id_Rol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Usuario,Username,Nombre,Correo,Contrasena,Sucursal,Id_Estatus,Fecha_Aprov,Id_Rol,Razon_social,RFC,Pais,Estado,Ciudad,Colonia,Localidad,Codigo_postal,Calle,Numero,Proveeedor_no_mks,Contacto_nombre1,Contacto_nombre2,Contacto_nombre3,Contacto_correo1,Contacto_correo2,Contacto_correo3,Contacto_tel1,Contacto_tel2,Contacto_tel3,permitir_Fru,permitir_Sec,permitir_Veg,Prorroga")] Usuario usuario)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            try
            {

                if (ModelState.IsValid)
                {
                    if (usuario.Id_Estatus == 2)
                    {
                        if (!db.UsuariosAltas.Any(x => x.Id_UsuarioRegistra == usuario.Id_Usuario))
                        {
                            UsuariosAltas ua = new UsuariosAltas();
                            ua.Id_UsuarioRegistra = usuario.Id_Usuario;
                            ua.Id_UsuarioAprueba = Convert.ToInt32(Session["Id_Usuario"]);
                            ua.Fecha = DateTime.Now;
                            usuario.Fecha_Aprov = ua.Fecha;
                            db.UsuariosAltas.Add(ua);
                        }
                    }
                    else if (usuario.Id_Estatus == 1)
                    {
                        usuario.Fecha_Aprov = null;
                    }
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();

                    if (rol == "Admin+")
                    {
                        return Json(new { Success = true, Message = "Información de usuario editada.", Message_data = "", Message_Classes = "primary", Message_concat = false });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                ViewBag.Id_Estatus = new SelectList(db.Estatus, "Id_Estatus", "Name", usuario.Id_Estatus);
                ViewBag.Id_Rol = new SelectList(db.Roles, "Id_Rol", "Rol", usuario.Id_Rol);
                
                if (rol == "Admin+")
                {
                    return Json(new { Success = true, Message = "Información de usuario incompleta.", Message_data = "", Message_Classes = "warning", Message_concat = false });
                }
                else
                {
                    return View(usuario);
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Error al procesar información de usuario.", Message_data = "", Message_Classes = "danger", Message_concat = false });
            }
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            UsuariosAltas usuariosAltas = usuario.UsuariosAltas.FirstOrDefault();
            db.UsuariosAltas.Remove(usuariosAltas);
            db.Usuarios.Remove(usuario);
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
