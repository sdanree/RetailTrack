using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailTrack.Services
{
    public class ProviderService
    {
        private readonly ApplicationDbContext _context;

        public ProviderService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea un nuevo proveedor.
        /// </summary>
        /// <param name="provider">El proveedor a crear.</param>
        public async Task AddProviderAsync(Provider provider)
        {
            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene todos los proveedores.
        /// </summary>
        /// <returns>Lista de proveedores.</returns>
        public async Task<List<Provider>> GetAllProvidersAsync()
        {
            return await _context.Providers.ToListAsync();
        }

        /// <summary>
        /// Obtiene un proveedor por su ID.
        /// </summary>
        /// <param name="id">ID del proveedor.</param>
        /// <returns>El proveedor encontrado o null si no existe.</returns>
        public async Task<Provider?> GetProviderByIdAsync(Guid id)
        {
            return await _context.Providers.FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Actualiza un proveedor existente.
        /// </summary>
        /// <param name="provider">El proveedor con los datos actualizados.</param>
        public async Task UpdateProviderAsync(Provider provider)
        {
            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un proveedor por su ID.
        /// </summary>
        /// <param name="id">ID del proveedor a eliminar.</param>
        public async Task DeleteProviderAsync(Guid id)
        {
            var provider = await GetProviderByIdAsync(id);
            if (provider != null)
            {
                _context.Providers.Remove(provider);
                await _context.SaveChangesAsync();
            }
        }
    }
}
