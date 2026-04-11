using Microsoft.EntityFrameworkCore.Storage;
using QLSanBong.Domain.Entities;
using QLSanBong.Domain.Interfaces;
using QLSanBong.Infrastructure.Data;

namespace QLSanBong.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly QLSanBongDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    private IGenericRepository<User>? _users;
    private IGenericRepository<Pitch>? _pitches;
    private IGenericRepository<PitchBooking>? _pitchBookings;
    private IGenericRepository<Service>? _services;
    private IGenericRepository<BookingService>? _bookingServices;
    private IGenericRepository<PitchMaintenance>? _pitchMaintenances;

    public UnitOfWork(QLSanBongDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<User> Users => _users ??= new GenericRepository<User>(_context);
    public IGenericRepository<Pitch> Pitches => _pitches ??= new GenericRepository<Pitch>(_context);
    public IGenericRepository<PitchBooking> PitchBookings => _pitchBookings ??= new GenericRepository<PitchBooking>(_context);
    public IGenericRepository<Service> Services => _services ??= new GenericRepository<Service>(_context);
    public IGenericRepository<BookingService> BookingServices => _bookingServices ??= new GenericRepository<BookingService>(_context);
    public IGenericRepository<PitchMaintenance> PitchMaintenances => _pitchMaintenances ??= new GenericRepository<PitchMaintenance>(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_currentTransaction != null) await _currentTransaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}