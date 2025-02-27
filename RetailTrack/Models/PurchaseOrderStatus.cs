using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetailTrack.Models;

namespace RetailTrack.Models
{
        public enum PurchaseOrderStatus
    {
        Pending,       // Pendiente de aprobación
        Approved,      // Aprobada
        PartiallyReceived, // Parcialmente recibida (algunas facturas asociadas)
        Completed,     // Completada (todos los materiales recibidos)
        Canceled       // Cancelada
    }
}    