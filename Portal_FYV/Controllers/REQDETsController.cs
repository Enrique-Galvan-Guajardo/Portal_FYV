using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
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
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            if (rol != "Admin+" && rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var rEQDETs = db.REQDETs.Include(r => r.REQHDR).Include(r => r.Embalaje);
            return View(rEQDETs.OrderByDescending(x => x.Id_REQDET).ToList());
        }

        // GET: REQDETs
        public ActionResult CapturarDetalles()
        {
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            switch (rol)
            {
                case "Admin+":
                case "Admin":
                case "Compras": 
                case "Requisiciones":
                    Usuario usuario = db.Usuarios.Find(id);
                    ViewBag.Id_Embalaje = new SelectList(db.Embalajes, "Id_Embalaje", "Tipo_Embalaje");
                    return View(usuario);
                default:
                    return RedirectToAction("Error", "Home");

            }
        }

        [HttpGet]
        public async Task<ActionResult> getProductListAPI(string urlAPI)
        {
            PublicController publicController = new PublicController();
            var result = await publicController.ObtenerDatosDeAPI(urlAPI);

            if (result is JsonResult jsonResult)
            {
                return Json(jsonResult.Data, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(500, "Error al obtener datos de la API desde PublicController.");
        }

        [HttpPost]
        public async Task<ActionResult> SendProductDataAPI(string urlAPI, string arts_html, string usuario, string sucursal)
        {
            var payload = new
            {
                arts_html,
                usuario,
                sucursal
            };

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Configurar encabezados si es necesario
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Convertir el objeto a JSON
                    var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

                    // Realizar la solicitud POST
                    var response = await httpClient.PostAsync(urlAPI, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        return Content(responseData, "application/json");
                    }
                    else
                    {
                        return new HttpStatusCodeResult((int)response.StatusCode, "Error al enviar datos.");
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(500, $"Error: {ex.Message}");
                }
            }
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
        public ActionResult CreateREQDETS(List<REQDET> rs, string sucursal, int Id_rh)
        {
            Usuario us = db.Usuarios.Find(Convert.ToInt32(Session["Id_Usuario"]));
            try
            {
                // Obtener la fecha actual
                DateTime fechaActual = DateTime.Now.Date;

                // Consultar la base de datos para verificar si existe un REQHDR con la fecha de hoy
                bool existeRegistroHoy = Id_rh > 0 ? false : db.REQHDRs.Any(r => DbFunctions.TruncateTime(r.Fecha_creacion) == fechaActual && r.Sucursal == sucursal);
                if (rs == null)
                {
                    return Json(new { Success = false, Message = "La solicitud no se ha podido guardar debido a que no se han seleccionado productos.", Message_data = "", Message_Classes = "warning", Message_concat = false });
                }
                //Si la prórroga es de -1, es para habilitar al usuario poder crear más solicitudes al día
                //Si Id_rh es mayor que 0, significa que es una lista enviada por lotes
                if (!existeRegistroHoy || us.Prorroga == "-1") {
                    REQHDR rh = new REQHDR();
                    //Si existe el Id_rh, no hay necesidad de volverlo a guardar
                    if (Id_rh > 0)
                    {
                        rh = db.REQHDRs.Find(Id_rh);
                    }
                    else
                    {
                        rh.Sucursal = sucursal;
                        rh.Fecha_creacion = DateTime.Now;
                        rh.Fecha_validacion = DateTime.Now.AddDays(1);
                        rh.Id_Creador = us.Id_Usuario;
                        rh.Estatus = 1;
                        db.REQHDRs.Add(rh);
                        db.SaveChanges();
                    }

                    foreach (var item in rs)
                    {
                        item.Id_REQHDR = rh.Id_REQHDR;
                        item.Cantidad_validada = item.Cantidad_solicitada;
                        item.Id_Embalaje_validado = item.Id_Embalaje;
                        item.Estatus = "1";
                        item.Fecha_creacion = DateTime.Now;
                    }

                    db.REQDETs.AddRange(rs);
                    try
                    {
                        db.SaveChanges();
                        //RedirectToAction("LogOut", "Usuarios");
                        return Json(new { Success = true, Value = rh.Id_REQHDR, Message = (Id_rh > 0 ? "Elementos adjuntos. Los elementos de la lista se han enviado por lotes y han sido capturados a la solicitud existente." : "Solicitud generada. Los elementos de la lista han sido capturados."), Message_data = "", Message_Classes = "success", Message_concat = false });
                    }
                    catch (Exception)
                    {
                        return Json(new { Success = false, Message = "Solicitud no capturada. Comprueba que la sesión este activa y vuelve a intentar.", Message_data = "", Message_Classes = "warning", Message_concat = false });
                    }

                }
                else
                {
                    return Json(new { Success = false, Message = "Ya se ha capturado una solicitud para el día de hoy, no se permite añadir otra.", Message_data = "", Message_Classes = "warning", Message_concat = false });
                }
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "Error al guardar la solicitud.", Message_data = ("Método: " + e.TargetSite + ".<br> Situación: " + e.Message.ToString() + ".<br> Código de error: " + e.HResult+ ".<br> Tipo: " + e.StackTrace).ToString(), Message_Classes = "warning", Message_concat = false });
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

        [HttpPost]
        public ActionResult guardarREQDET([Bind(Include = "Id_REQDET,Id_REQHDR,Clave_articulo,Descripcion,Cantidad_solicitada,Id_Embalaje,Cantidad_validada,Id_Embalaje_validado")] REQDET rEQDET)
        {
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";
            int id = Convert.ToInt32(Session["Id_Usuario"]);
            if (rol == "Compras")
            {
                rEQDET.Id_Creator = id;
            }
            //rEQDET.Id_REQDET = 0;
            rEQDET.Fecha_creacion = DateTime.Now;
            rEQDET.Fecha_validacion = DateTime.Now;
            rEQDET.Estatus = "2";

            if (!db.REQDETs.Any(x => x.Descripcion == rEQDET.Descripcion && x.Id_REQHDR == rEQDET.Id_REQHDR) && rEQDET != null && rEQDET.Cantidad_validada != null && rEQDET.Id_Embalaje_validado != null)
            {
                try
                {
                    db.REQDETs.Add(rEQDET);
                    db.SaveChanges();
                    // Generar el token anti-falsificación
                    var token = AntiForgery.GetHtml().ToString();
                    // Crear un objeto anónimo con las propiedades necesarias de rEQDET
                    // Convertir el objeto rEQDET en un objeto JSON
                    string reqdetJson = JsonConvert.SerializeObject(rEQDET, new JsonSerializerSettings
                    {
                        // Ignorar las referencias circulares
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        // Incluir solo propiedades que tengan un valor
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    return Json(new { Success = true, value = reqdetJson, Message = "Registro agregado correctamente.", Message_data = new { sucursal = db.REQHDRs.Find(rEQDET.Id_REQHDR).Sucursal, embalaje = db.Embalajes.Find(rEQDET.Id_Embalaje).Tipo_Embalaje, token}, Message_Classes = "alert-success", Message_concat = false });
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult EditarCantidadValidada([Bind(Include = "Id_REQDET,Cantidad_validada,Id_Embalaje_validado")] REQDET rEQDET)
        {
            
            REQDET rd = db.REQDETs.Find(rEQDET.Id_REQDET);
            if (rEQDET.Cantidad_validada != null)
                rd.Cantidad_validada = rEQDET.Cantidad_validada;
            if (rEQDET.Id_Embalaje_validado != null)
                rd.Id_Embalaje_validado = rEQDET.Id_Embalaje_validado;


            if (rd != null)
            {
                try
                {
                    db.Entry(rd).State = EntityState.Modified;
                    db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

        // POST: REQDETs/Delete/5
        [HttpPost, ActionName("DeleteREQDET")]
        public ActionResult DeleteREQDET(int id)
        {
            REQDET rEQDET = db.REQDETs.Find(id);
            db.REQDETs.Remove(rEQDET);
            db.SaveChanges();
            return Json(new { Success = true, value = "", Message = "Registro eliminado correctamente.", Message_data = "", Message_Classes = "alert-success", Message_concat = false });
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
