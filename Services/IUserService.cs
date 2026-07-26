namespace LibraryManager
{
    public interface IUserService
    {
        Task<UserDetailsDto?> GetUserByIdAsync(int userId);
        Task<List<UserDetailsDto>> SearchUsersAsync(UserQueryDto query);
    }
}