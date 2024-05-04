namespace Domain.Entities.Users;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    public User(string email, string password)
    {
        SetCreate(email);
        Email = email;
        Password = password;
    }

    public void Deactivate(string requestedBy) => SetInactive(requestedBy);

    public void UpdatePassword(string password, string requestedBy)
    {
        SetUpdate(requestedBy);
        Password = password;
    }
}