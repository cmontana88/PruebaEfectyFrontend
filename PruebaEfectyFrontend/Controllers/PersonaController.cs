using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PruebaEfectyFrontend.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PruebaEfectyFrontend.Controllers
{
    public class PersonaController : Controller
    {
        private readonly IConfiguration _configuration;
        public PersonaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            GetTipoDocumentos();
            ViewBag.MensajeError = "";
            ViewBag.MensajeExitoso = "";
            return View();
        }

        [HttpPost]
        public IActionResult Crear(PersonaViewModel persona)
        {
            try
            {
                string endPointApi = _configuration.GetValue<string>("EndPointApi");
                string crearPersona = _configuration.GetValue<string>("CrearPersona");
                var client = new RestClient(endPointApi + crearPersona);                

                var request = new RestRequest();

                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/pdf");
                request.AddParameter("application/json", JsonConvert.SerializeObject(persona), ParameterType.RequestBody);

                var response = client.Execute(request);
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.MensajeExitoso = "Persona creada con exito";
                    ViewBag.MensajeError = "";
                }
                else
                {
                    ViewBag.MensajeExitoso = "";
                    ViewBag.MensajeError = "Error al crear la persona";
                }

                GetTipoDocumentos();
            }
            catch (Exception)
            {

                throw;
            }

            ModelState.Clear();

            return View("Index");
        }

        public List<TipoDocumentoViewModel> GetTipoDocumentos()
        {
            List<TipoDocumentoViewModel> tipoDocumento = new List<TipoDocumentoViewModel>();
            try
            {                
                string endPointApi = _configuration.GetValue<string>("EndPointApi");
                string obtenerTipoDocumentos = _configuration.GetValue<string>("ObtenerTipoDocumentos");
                var client = new RestClient(endPointApi + obtenerTipoDocumentos);
                var request = new RestRequest();
                request.Method = Method.GET;

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tipoDocumento = JsonConvert.DeserializeObject<List<TipoDocumentoViewModel>>(response.Content);
                }

                ViewBag.TipoDocumentos = new SelectList(tipoDocumento, "Id", "Valor");
            }
            catch (Exception)
            {
                throw;
            }

            return tipoDocumento;
        }

        public IActionResult List()
        {
            List<TipoDocumentoViewModel> tipoDocumento = new List<TipoDocumentoViewModel>();
            List<PersonaViewModel> personas = new List<PersonaViewModel>();
            string endPointApi = _configuration.GetValue<string>("EndPointApi");
            string consultarPersona = _configuration.GetValue<string>("ConsultarPersona");
            var client = new RestClient(endPointApi + consultarPersona);
            var request = new RestRequest();
            request.Method = Method.GET;

            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                personas = JsonConvert.DeserializeObject<List<PersonaViewModel>>(response.Content);
            }

            tipoDocumento = GetTipoDocumentos();

            foreach(var pers in personas)
            {
                pers.TipoDocumentoDescripcion = tipoDocumento.Where(t => t.Id == pers.TipoDocumento).FirstOrDefault().Valor;
            }

            var filtros = new List<(int index, string valor)>
            {
                (1, "Nombre"),
                (2, "Apellidos"),
                (3, "Tipo de documento"),
                (4, "Valor a ganar")
            };

            ViewBag.Filtros = new SelectList(filtros.Select(f => new { Index = f.index, Valor = f.valor }).ToList(), "Index", "Valor");

            return View(personas);
        }

        [HttpPost]
        public IActionResult FiltrarPersonas([FromBody] Filtro filtrosForm)
        {
            List<TipoDocumentoViewModel> tipoDocumento = new List<TipoDocumentoViewModel>();
            List<PersonaViewModel> personas = new List<PersonaViewModel>();
            string endPointApi = _configuration.GetValue<string>("EndPointApi");
            string consultarPersona = _configuration.GetValue<string>("ConsultarPersona");
            var client = new RestClient(endPointApi + consultarPersona);
            var request = new RestRequest();
            request.Method = Method.GET;

            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                personas = JsonConvert.DeserializeObject<List<PersonaViewModel>>(response.Content);
            }

            tipoDocumento = GetTipoDocumentos();

            foreach (var pers in personas)
            {
                pers.TipoDocumentoDescripcion = tipoDocumento.Where(t => t.Id == pers.TipoDocumento).FirstOrDefault().Valor;
            }

            switch (filtrosForm.FiltroId)
            {
                case 1:
                    personas = personas.Where(p => p.Nombres.ToLower().Contains(filtrosForm.Texto.ToLower())).ToList();
                    break;
                case 2:
                    personas = personas.Where(p => p.Apellidos.ToLower().Contains(filtrosForm.Texto.ToLower())).ToList();
                    break;
                case 3:
                    personas = personas.Where(p => p.TipoDocumentoDescripcion.ToLower().Contains(filtrosForm.Texto.ToLower())).ToList();
                    break;
                case 4:
                    personas = personas.Where(p => p.ValoraGanar.ToString().Contains(filtrosForm.Texto)).ToList();
                    break;
                default:
                    break;
            }

            var filtros = new List<(int index, string valor)>
            {
                (1, "Nombre"),
                (2, "Apellidos"),
                (3, "Tipo de documento"),
                (4, "Valor a ganar")
            };

            ViewBag.Filtros = new SelectList(filtros.Select(f => new { Index = f.index, Valor = f.valor }).ToList(), "Index", "Valor");

            return View("List", personas);
        }
    }
}
