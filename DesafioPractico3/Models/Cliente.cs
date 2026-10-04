using System;
using System.Collections.Generic;

namespace DesafioPractico3.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }

        // Relación: Un cliente tiene múltiples órdenes
        public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
    }
}