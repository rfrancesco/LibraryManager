using Microsoft.EntityFrameworkCore;

namespace LibraryManager
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly int _defaultPageSize = 20;
        private readonly int _maxPageSize = 100;
        private int ValidatePageSize(int? pageSize)
        {
            if (pageSize is null || pageSize <= 0)
                return _defaultPageSize;
            if (pageSize > _maxPageSize)
                return _maxPageSize;
            return pageSize.Value;
        }
        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(int userId)
        {
            var result = await _db.Users
                .Where(u => u.UserId == userId)
                .Select(u => new
                UserDetailsDto(
                    u.UserId,
                    u.Name
                )).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _db.Users.AnyAsync(u => u.UserId == userId);
        }

        public async Task<List<UserDetailsDto>> SearchUsersAsync(UserQueryDto query)
        {
            var page = query.Page == null ? 1 : query.Page.Value;
            var pageSize = ValidatePageSize(query.PageSize);
            var userQuery = _db.Users.AsQueryable();
            if (query.Name != null)
                userQuery = userQuery.Where(u => u.Name.ToLower().Contains(query.Name.ToLower()));
            var result = await userQuery
                    .Select(u => new UserDetailsDto(u.UserId, u.Name))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            return result;
        }

        public async Task<UserDetailsDto> CreateUserAsync(string name)
        {
            var user = new User { Name = name };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new UserDetailsDto(user.UserId, user.Name);
        }
    }
}