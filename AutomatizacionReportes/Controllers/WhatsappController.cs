using AutomatizacionReportes.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutomatizacionReportes.Controllers
{
    [ApiController]
    [Route("api/procesos/whatsapp")]
    public class WhatsappController : ControllerBase
    {
        private readonly WhatsappProcessService _service;
        private readonly ExecutionLockService _lock;

        public WhatsappController(
            WhatsappProcessService service,
            ExecutionLockService executionLock)
        {
            _service = service;
            _lock = executionLock;
        }

        [HttpPost("ejecutar")]
        public async Task<IActionResult> Ejecutar()
        {
            if (!_lock.TryStart())
            {
                return Conflict(new
                {
                    success = false,
                    message = "El proceso ya se encuentra en ejecución"
                });
            }

            try
            {
                var result = await _service.EjecutarAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
            finally
            {
                _lock.End();
            }
        }
    }
}
