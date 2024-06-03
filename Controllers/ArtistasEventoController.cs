using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Services;

namespace MvcCoreProyectoSejo.Controllers
{
    public class ArtistasEventoController : Controller
    {
        private ServiceEventos service;
        private ServiceStorageAWS serviceStorageAWS;

        public ArtistasEventoController(ServiceEventos service, ServiceStorageAWS serviceStorageAWS)
        {
            this.service = service;
            this.serviceStorageAWS = serviceStorageAWS;
        }
        public async Task<IActionResult> _AddArtistaToEvento(int idevento)
        {
            EventoDetalles evento = await this.service.FindEventoAsync(idevento);
            List<UsuarioDetalles> artistas = await this.service.GetAllArtistasAsync();

            ViewData["Evento"] = evento;

            return PartialView("_AddArtistaToEvento", artistas);
        }

        [HttpPost]
        public async Task<IActionResult> _AddArtistaToEvento(int idevento, int idartista)
        {
            try
            {
                await this.service.AddArtistaEventoAsync(idevento, idartista);
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
            catch (Exception ex)
            {
                TempData["ERROR"] = "El Artista ya esta en este Evento";
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CrearArtista(Artista artista, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string fileName = file.FileName;
                string folderName = "usuarios/"; // Especifica la carpeta aquí

                using (Stream stream = file.OpenReadStream())
                {
                    bool uploadSuccess = await this.serviceStorageAWS.UploadFileAsync(folderName, fileName, stream);
                    if (!uploadSuccess)
                    {
                        return StatusCode(500, "No se pudo subir la imagen");
                    }
                }

                artista.Foto = fileName;
            }

            await this.service.CrearArtistaAsync(artista);
            return RedirectToAction("Details", "Eventos", new { id = artista.IdEvento });
        }

        [Authorize]
        [HttpPost("DeleteArtistaEvento")]
        public async Task<IActionResult> DeleteArtistaEvento(int idevento, int idartista)
        {
            bool success = await this.service.DeleteArtistaEventoAsync(idevento, idartista);
            if (success)
            {
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
            else
            {
                ViewData["Error"] = "Error";
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
        }

        [Authorize]
        [HttpPost("DeleteArtistaTemp")]
        public async Task<IActionResult> DeleteArtistaTemp(int idevento, int idartista)
        {
            bool success = await this.service.DeleteArtistaTempAsync(idevento, idartista);
            if (success)
            {
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
            else
            {
                ViewData["Error"] = "Error";
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
        }
    }
}
