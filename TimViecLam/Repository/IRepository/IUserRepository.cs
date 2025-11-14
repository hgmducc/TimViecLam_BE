namespace TimViecLam.Repository.IRepository
{
    public interface IUserRepository //các truy vấn liên quan đến user
    {
        Task<bool> EmailExitAsync(string email);
        Task<bool> PhoneExitAsync(string phoneNumber);

    }
}
