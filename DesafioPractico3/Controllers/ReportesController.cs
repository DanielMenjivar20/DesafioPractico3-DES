using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioPractico3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Solo los usuarios con rol Admin pueden acceder a estos reportes
    public class ReportesController : ControllerBase
    {
        // GET: api/reportes/clientes-activos
        [HttpGet("clientes-activos")]
        public IActionResult GetReporteClientesActivos()
        {
            // Aquí puedes retornar la URL del reporte publicado en SSRS o un mensaje de éxito
            return Ok(new
            {
                reporte = "Clientes Activos",
                url = "http://localhost/ReportServer/Pages/ReportViewer.aspx?/CompanyManagement/ClientesActivos"
            });
        }

        // GET: api/reportes/ingresos-clientes
        [HttpGet("ingresos-clientes")]
        public IActionResult GetReporteIngresosClientes()
        {
            return Ok(new
            {
                reporte = "Ingresos por Cliente",
                url = "http://localhost/ReportServer/Pages/ReportViewer.aspx?/CompanyManagement/IngresosClientes"
            });
        }

        // GET: api/reportes/clientes-inactivos
        [HttpGet("clientes-inactivos")]
        public IActionResult GetReporteClientesInactivos()
        {
            return Ok(new
            {
                reporte = "Clientes Inactivos",
                url = "http://localhost/ReportServer/Pages/ReportViewer.aspx?/CompanyManagement/ClientesInactivos"
            });
        }
    }
}