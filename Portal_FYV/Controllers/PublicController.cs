using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal_FYV.Controllers
{
    public class PublicController : Controller
    {
        // GET: Public
        public ActionResult Index()
        {
            return View();
        }

        // GET: Public/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Public/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Public/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Public/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Public/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Public/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Public/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerDatosDeAPI(string urlAPI = "")
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(urlAPI);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta JSON en una lista de objetos
                        //List<ObjetoAPI> listaObjetos = JsonConvert.DeserializeObject<List<ObjetoAPI>>(jsonResponse);

                        // Retornar la lista en formato JSON
                        return Json(jsonResponse, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(response.StatusCode, "Error al obtener datos de la API.");
                    }
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(500, "Error en la solicitud a la API: " + ex.Message);
                }
            }
        }

    }
}
