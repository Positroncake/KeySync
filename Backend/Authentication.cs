using Npgsql;

namespace Backend;

public class Authentication
{
    private string connStrFilePath;
    private string connStr;

    public Authentication(string connStrFilePath)
    {
        this.connStrFilePath = connStrFilePath;
        connStr = File.ReadAllLines(this.connStrFilePath)[0];
    }

    public void NewUser()
    {
        using var connection = new NpgsqlConnection(connStr);
    }
}