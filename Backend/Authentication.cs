using Npgsql;

namespace Backend;

public class Authentication
{
    public static void NewUser()
    {
        using var c = new NpgsqlConnection();
    }
}