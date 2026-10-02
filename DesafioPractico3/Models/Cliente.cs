using System;
using System.Collections.Generic;

namespace DesafioPractico3.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Relación: Un cliente tiene múltiples órdenes[cite: 2]
        public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
    }
}