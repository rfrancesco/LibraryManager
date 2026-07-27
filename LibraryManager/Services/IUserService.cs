namespace LibraryManager
{
    public interface IUserService
    {
        Task<UserDetailsDto?> GetUserByIdAsync(int userId);
        Task<bool> UserExistsAsync(int userId);
        Task<List<UserDetailsDto>> SearchUsersAsync(UserQueryDto query);
        Task<UserDetailsDto> CreateUserAsync(string name);
    }
}