using QLSanBong.Domain.Entities;

namespace QLSanBong.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
   
    IGenericRepository<User> Users { get; }
    
    

    //===========sân bóng=========
    IGenericRepository<Pitch> Pitches { get; }
    IGenericRepository<PitchBooking> PitchBookings { get; }
    
    // --- dịch vụ và tính tiền ---
    IGenericRepository<Service> Services { get; }
    IGenericRepository<BookingService> BookingServices { get; }
    IGenericRepository<PitchMaintenance> PitchMaintenances { get; }

    // Sau này thêm các repo khác nếu phát sinh 
    Task<int> CompleteAsync(); // SaveChanges

    // --- Quản lý Transaction ---
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
