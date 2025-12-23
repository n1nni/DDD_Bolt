using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using Bolt.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Bolt.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Passenger?> GetPassengerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OfType<Passenger>()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Driver?> GetDriverByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OfType<Driver>()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Driver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OfType<Driver>()
            .Where(d => d.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    // Get all users
    public async Task<IEnumerable<User>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // Get all drivers
    public async Task<IEnumerable<Driver>> GetAllDriversAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .OfType<Driver>()
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // Get all passengers
    public async Task<IEnumerable<Passenger>> GetAllPassengersAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .OfType<Passenger>()
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}