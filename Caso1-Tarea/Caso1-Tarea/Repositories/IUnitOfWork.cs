

using Caso1_Tarea.Models;
using Caso1_Tarea.Repositories.Interfaces;

namespace Caso1_Tarea.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Producto> Productos { get; }
        IGenericRepository<Proveedore> Proveedores { get; }
        IGenericRepository<Pedidosproveedore> PedidosProveedores { get; }
        IGenericRepository<Detallepedido> DetallePedidos { get; }
        IGenericRepository<Movimientosinventario> MovimientosInventario { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}