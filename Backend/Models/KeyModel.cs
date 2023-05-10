using Dapper.Contrib.Extensions;

namespace Backend.Models;

public class KeyModel
{
    [Key] public ulong Id { get; set; }
    public byte[] Ipv4 { get; set; } = Array.Empty<byte>();
    public short[] Ipv6 { get; set; } = Array.Empty<short>();
    public int Port { get; set; }
    public string Label { get; set; } = string.Empty;
}