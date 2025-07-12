namespace UserManagement.Infrastructure.Models;
public class AuthResponse
{
    public string AccessToken { set; get; }
    public string RefreshToken { set; get; }
    public DateTime AccessTokenExpiry { set; get; }

}

