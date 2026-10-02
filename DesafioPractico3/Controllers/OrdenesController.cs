using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DesafioPractico3.Data;
using DesafioPractico3.Models;

namespace DesafioPractico3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdenesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ordenes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orden>>> GetOrdenes()
        {
            return await _context.Ordenes.Include(o => o.Cliente).ToListAsync();
        }

        // GET: api/ordenes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Orden>> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
            {
                return NotFound(new { message = "Orden no encontrada" });
            }

            return orden;
        }

        // GET: api/ordenes/cliente/5
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Orden>>> GetOrdenesPorCliente(int clienteId)
        {
            var ordenes = await _context.Ordenes
                .Where(o => o.ClienteId == clienteId)
                .Include(o => o.Cliente)
                .ToListAsync();

            return ordenes;
        }

        // POST: api/ordenes
        [HttpPost]
        public async Task<ActionResult<Orden>> PostOrden(Orden orden)
        {
            // Validar que el cliente exista antes de crear la orden
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == orden.ClienteId);
            if (!clienteExiste)
            {
                return BadRequest(new { message = "El ClienteId especificado no existe." });
            }

            orden.FechaOrden = DateTime.Now;
            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrden), new { id = orden.Id }, orden);
        }

        // DELETE: api/ordenes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrden(int id)
        {
            var orden = await _context.Ordenes.FindAsync(id);
            if (orden == null)
            {
                return NotFound();
            }

            _context.Ordenes.Remove(orden);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}