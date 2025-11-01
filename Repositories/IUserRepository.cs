using UserManagementAzFunction.Models;

namespace UserManagementAzFunction.Repositories
{
    /// <summary>
    /// Repository interface for User entity operations.
    /// Provides abstraction over data access layer.
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}
