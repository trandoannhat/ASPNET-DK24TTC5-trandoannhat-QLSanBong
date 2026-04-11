using QLSanBong.Domain.Entities;

namespace QLSanBong.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // --- System ---
    IGenericRepository<User> Users { get; }

    // --- Pitch Management ---
    IGenericRepository<Pitch> Pitches { get; }
    IGenericRepository<PitchBooking> PitchBookings { get; }
    IGenericRepository<PitchMaintenance> PitchMaintenances { get; }

    // --- Services & Invoices ---
    IGenericRepository<Service> Services { get; }
    IGenericRepository<BookingService> BookingServices { get; }

    // --- DbContext Actions ---
    Task<int> CompleteAsync();

    // --- Transaction Management ---
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}