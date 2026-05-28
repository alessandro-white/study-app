using StudyApp.Models;

namespace StudyApp.Services;

public class AuthService
{
    public static AuthService Instance { get; } = new();

    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    private AuthService() { }

    public (bool success, string error) Register(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username)) return (false, "Username is required.");
        if (string.IsNullOrWhiteSpace(email)) return (false, "Email is required.");
        if (string.IsNullOrWhiteSpace(password)) return (false, "Password is required.");
        if (password.Length < 6) return (false, "Password must be at least 6 characters.");

        if (DatabaseService.Instance.GetUserByUsername(username) != null)
            return (false, "Username already taken.");
        if (DatabaseService.Instance.GetUserByEmail(email) != null)
            return (false, "Email already registered.");

        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        CurrentUser = DatabaseService.Instance.CreateUser(username, email, hash);
        return (true, "");
    }

    public (bool success, string error) Login(string usernameOrEmail, string password)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail)) return (false, "Username or email is required.");
        if (string.IsNullOrWhiteSpace(password)) return (false, "Password is required.");

        var user = DatabaseService.Instance.GetUserByUsername(usernameOrEmail)
                ?? DatabaseService.Instance.GetUserByEmail(usernameOrEmail);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "Invalid credentials.");

        CurrentUser = user;
        return (true, "");
    }

    public void Logout() => CurrentUser = null;
}
