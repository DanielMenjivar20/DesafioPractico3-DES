using System;

namespace DesafioPractico3.Models
{
    public class Orden
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }

        // Relación: Una orden pertenece a un solo cliente[cite: 2]
        public Cliente Cliente { get; set; }

        public DateTime FechaOrden { get; set; }
        public decimal MontoTotal { get; set; }
    }
}