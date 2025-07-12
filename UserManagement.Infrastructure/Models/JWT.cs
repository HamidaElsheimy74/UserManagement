namespace UserManagement.Infrastructure.Models;
public class JWT
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public double AccessTokenExpirationTime { set; get; }
    public double RefreshTokenExpirationTime { set; get; }
    public string Audience { get; set; }

}