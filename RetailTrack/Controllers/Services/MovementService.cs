using RetailTrack.Data;
using RetailTrack.Models;

namespace RetailTrack.Services
{
    public class MovementService
    {
        private readonly ApplicationDbContext _context;

        public MovementService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los movimientos
        public List<Movement> GetAllMovements()
        {
            return _context.Movements.ToList() ?? new List<Movement>();
        }

        // Obtener un movimiento por ID
        public Movement GetMovementById(Guid id)
        {
            return _context.Movements.FirstOrDefault(m => m.Id == id);
        }

        // Crear un nuevo movimiento
        public void AddMovement(Movement movement)
        {
            _context.Movements.Add(movement);
            _context.SaveChanges();
        }

        // Actualizar un movimiento existente
        public void UpdateMovement(Movement movement)
        {
            _context.Movements.Update(movement);
            _context.SaveChanges();
        }

        // Eliminar un movimiento
        public void DeleteMovement(Guid id)
        {
            try
            {
                var movement = _context.Movements.FirstOrDefault(m => m.Id == id);
                if (movement != null)
                {
                    _context.Movements.Remove(movement);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log o manejo de errores
                throw new Exception("Error eliminando el movimiento", ex);
            }
        }

    }
}
