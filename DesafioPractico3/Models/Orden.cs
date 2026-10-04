using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DesafioPractico3.Models
{
    public class Orden
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }

        // Relación: Una orden pertenece a un solo cliente
        public Cliente? Cliente { get; set; }

        public DateTime FechaOrden { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoTotal { get; set; }
    }
}