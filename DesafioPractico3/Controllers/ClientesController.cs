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
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOutputCacheStore _cacheStore;

        public ClientesController(ApplicationDbContext context, IOutputCacheStore cacheStore)
        {
            _context = context;
            _cacheStore = cacheStore;
        }

        // GET: api/clientes
        [HttpGet]
        [OutputCache(PolicyName = "Clientes")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Ordenes)
                .ToListAsync();
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        [OutputCache(PolicyName = "Clientes")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Ordenes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            return cliente;
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            cliente.FechaRegistro = DateTime.UtcNow;
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            await _cacheStore.EvictByTagAsync("clientes", default);

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest(new { message = "El id de la URL no coincide con el del cuerpo" });

            var existente = await _context.Clientes.FindAsync(id);
            if (existente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            // Solo se actualizan los campos editables (FechaRegistro se conserva)
            existente.Nombre = cliente.Nombre;
            existente.Email = cliente.Email;

            await _context.SaveChangesAsync();
            await _cacheStore.EvictByTagAsync("clientes", default);

            return NoContent();
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            await _cacheStore.EvictByTagAsync("clientes", default);

            return NoContent();
        }
    }
}