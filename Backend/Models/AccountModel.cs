using Dapper.Contrib.Extensions;

namespace Backend.Models;

[Table("accounts")]
public class AccountModel
{
    [Key] public ulong Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public byte[] Hash { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public DateTime Created { get; set; }
}

public class PasswordModel
{
    public byte[] Hash { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
}