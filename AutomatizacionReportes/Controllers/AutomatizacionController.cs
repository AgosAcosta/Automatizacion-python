using AutomatizacionReportes.Infrastructure;
using AutomatizacionReportes.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;

namespace AutomatizacionReportes.Controllers
{
    [ApiController]
    [Route("api/procesos")]
    public class AutomatizacionController : ControllerBase
    {
        private readonly WhatsappProcessService _whatsapp;
        private readonly SmsProcessService _sms;
        private readonly MailProcessService _mail;
        private readonly ExecutionLockService _lock;
        private readonly MailAsignacionProcessService _mailAsignacion;

        public AutomatizacionController(
            WhatsappProcessService whatsapp, SmsProcessService sms, MailProcessService mail, MailAsignacionProcessService mailAsignacion,
            ExecutionLockService executionLock)
        {
            _whatsapp = whatsapp;
            _sms = sms; 
            _mail = mail;
            _mailAsignacion = mailAsignacion;
            _lock = executionLock;
        }

        [HttpPost("whatsapp/ejecutar")]
        public Task<IActionResult> EjecutarWhatsapp()
            => EjecutarProceso(_whatsapp.EjecutarAsync, "WhatsApp");

        [HttpPost("sms/ejecutar")]
        public Task<IActionResult> EjecutarSms()
            => EjecutarProceso(_sms.EjecutarAsync, "SMS");


        [HttpPost("mail/ejecutar")]
        public Task<IActionResult> EjecutarMail()
    => EjecutarProceso(_mail.EjecutarAsync, "Mail");

        [HttpPost("mail-asignacion/ejecutar")]
        public Task<IActionResult> EjecutarMailAsignacion()
    => EjecutarProceso(_mailAsignacion.EjecutarAsync, "Mail Asignación");

        private async Task<IActionResult> EjecutarProceso<T>(
            Func<Task<T>> ejecutar, string nombreProceso)
        {
            if (!_lock.TryStart())
            {
                return Conflict(new
                {
                    success = false,
                    message = $"El proceso {nombreProceso} ya se encuentra en ejecución"
                });
            }

            try
            {
                var result = await ejecutar();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error al ejecutar {nombreProceso}: {ex.Message}"
                });
            }
            finally
            {
                _lock.End();
            }
        }
        
    }
}

