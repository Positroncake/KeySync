using Dapper.Contrib.Extensions;

namespace Backend.Models;

[Table("tokens")]
public class TokenModel
{
    [Key] public ulong Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}