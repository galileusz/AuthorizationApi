using System.ComponentModel.DataAnnotations;

namespace AuthManager.API.DAL.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
}