using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
        private readonly IOutputCacheStore _cacheStore;

        public OrdenesController(ApplicationDbContext context, IOutputCacheStore cacheStore)
        {
            _context = context;
            _cacheStore = cacheStore;
        }

        // GET: api/ordenes
        [HttpGet]
        [OutputCache(PolicyName = "Ordenes")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrdenes()
        {
            var ordenes = await _context.Ordenes
                .AsNoTracking()
                .Select(o => new
                {
                    o.Id,
                    o.ClienteId,
                    Cliente = new { o.Cliente!.Id, o.Cliente.Nombre, o.Cliente.Email },
                    o.FechaOrden,
                    o.MontoTotal
                })
                .ToListAsync();

            return Ok(ordenes);
        }

        // GET: api/ordenes/5
        [HttpGet("{id}")]
        [OutputCache(PolicyName = "Ordenes")]
        public async Task<ActionResult<object>> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.ClienteId,
                    Cliente = new { o.Cliente!.Id, o.Cliente.Nombre, o.Cliente.Email },
                    o.FechaOrden,
                    o.MontoTotal
                })
                .FirstOrDefaultAsync();

            if (orden == null)
                return NotFound(new { message = "Orden no encontrada" });

            return Ok(orden);
        }

        // GET: api/ordenes/cliente/5
        [HttpGet("cliente/{clienteId}")]
        [OutputCache(PolicyName = "Ordenes")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrdenesPorCliente(int clienteId)
        {
            var ordenes = await _context.Ordenes
                .AsNoTracking()
                .Where(o => o.ClienteId == clienteId)
                .Select(o => new
                {
                    o.Id,
                    o.ClienteId,
                    Cliente = new { o.Cliente!.Id, o.Cliente.Nombre, o.Cliente.Email },
                    o.FechaOrden,
                    o.MontoTotal
                })
                .ToListAsync();

            return Ok(ordenes);
        }

        // POST: api/ordenes
        [HttpPost]
        public async Task<ActionResult<Orden>> PostOrden(Orden orden)
        {
            if (orden.MontoTotal <= 0)
                return BadRequest(new { message = "El monto total debe ser mayor a cero." });

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == orden.ClienteId);
            if (!clienteExiste)
                return BadRequest(new { message = "El ClienteId especificado no existe." });

            orden.FechaOrden = DateTime.UtcNow;
            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            // Los clientes incluyen sus órdenes, por eso se limpian ambas cachés
            await _cacheStore.EvictByTagAsync("ordenes", default);
            await _cacheStore.EvictByTagAsync("clientes", default);

            return CreatedAtAction(nameof(GetOrden), new { id = orden.Id }, orden);
        }

        // DELETE: api/ordenes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrden(int id)
        {
            var orden = await _context.Ordenes.FindAsync(id);
            if (orden == null)
                return NotFound(new { message = "Orden no encontrada" });

            _context.Ordenes.Remove(orden);
            await _context.SaveChangesAsync();

            await _cacheStore.EvictByTagAsync("ordenes", default);
            await _cacheStore.EvictByTagAsync("clientes", default);

            return NoContent();
        }
    }
}