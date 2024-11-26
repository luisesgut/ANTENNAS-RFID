using Backend_Final.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly MainService _mainService;

        public MainController(MainService mainService)
        {
            _mainService = mainService;
        }
        [HttpPost("start")]
        public IActionResult Start()
        {
            try
            {
                _mainService.Start();
                return Ok("Proceso iniciado. Lector configurado y leyendo etiquetas.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al iniciar el proceso: {ex.Message}");
            }
        }

        [HttpPost("stop")]
        public IActionResult Stop()
        {
            try
            {
                _mainService.Stop();
                return Ok("Proceso detenido. Lector desconectado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al detener el proceso: {ex.Message}");
            }
        }
    }
}