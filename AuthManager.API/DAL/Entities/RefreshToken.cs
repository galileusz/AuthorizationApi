using System.ComponentModel.DataAnnotations;

namespace AuthManager.API.DAL.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
}