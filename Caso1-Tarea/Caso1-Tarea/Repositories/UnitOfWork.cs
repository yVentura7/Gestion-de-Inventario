using Microsoft.EntityFrameworkCore.Storage;
using Caso1_Tarea.Models;
using Caso1_Tarea.Repositories.Interfaces;

namespace Caso1_Tarea.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Lab5Context _context;
        private IDbContextTransaction? _transaction;
        
        private IGenericRepository<Producto>? _productos;
        private IGenericRepository<Proveedore>? _proveedores;
        private IGenericRepository<Pedidosproveedore>? _pedidosProveedores;
        private IGenericRepository<Detallepedido>? _detallePedidos;
        private IGenericRepository<Movimientosinventario>? _movimientosInventario;

        public UnitOfWork(Lab5Context context)
        {
            _context = context;
        }

        public IGenericRepository<Producto> Productos => 
            _productos ??= new GenericRepository<Producto>(_context);

        public IGenericRepository<Proveedore> Proveedores => 
            _proveedores ??= new GenericRepository<Proveedore>(_context);

        public IGenericRepository<Pedidosproveedore> PedidosProveedores => 
            _pedidosProveedores ??= new GenericRepository<Pedidosproveedore>(_context);

        public IGenericRepository<Detallepedido> DetallePedidos => 
            _detallePedidos ??= new GenericRepository<Detallepedido>(_context);

        public IGenericRepository<Movimientosinventario> MovimientosInventario => 
            _movimientosInventario ??= new GenericRepository<Movimientosinventario>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}