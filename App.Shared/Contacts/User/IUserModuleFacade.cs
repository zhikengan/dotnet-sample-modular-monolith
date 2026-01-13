namespace App.Shared.Contacts;

public interface IUserModuleFacade
{
    Task<bool> UserExistsAsync(Guid userId);
    Task<string?> GetUserNameAsync(Guid userId);
}
