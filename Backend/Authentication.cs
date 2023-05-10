using System.Security.Cryptography;
using Backend.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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

    #region Accounts

    public async Task<string> NewAccount(string username, string password)
    {
        #region Generate salt/hash

        byte[] salt = RandomNumberGenerator.GetBytes(512 / 8);
        byte[] hash = HashPbkdf2(password, salt);

        #endregion

        #region Store to database

        await using var connection = new NpgsqlConnection(connStr);
        var newAcc = new AccountModel
        {
            Username = username,
            Hash = hash,
            Salt = salt,
            Created = DateTime.UtcNow
        };
        await connection.InsertAsync(newAcc);

        #endregion
        
        return await NewToken(username);
    }

    public async Task<(bool, string)> Login(string username, string password)
    {
        #region Retrieve hash and salt from db

        await using var connection = new NpgsqlConnection(connStr);
        List<PasswordModel> hashes = (await connection.QueryAsync<PasswordModel>(
            "SELECT * FROM accounts WHERE Username = @Username LIMIT 1", new
            {
                Username = username
            })).ToList();
        PasswordModel storedPwd = hashes[0];

        #endregion

        #region Compute hash with user-provided password

        byte[] userHash = HashPbkdf2(password, storedPwd.Salt);
        bool HashesEqual(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) => x.SequenceEqual(y);

        #endregion
        
        return HashesEqual(userHash, storedPwd.Hash) == false ? (false, "") : (true, await NewToken(username));
    }

    #endregion

    #region Tokens

    private async Task<string> NewToken(string username, DateTime? expiry = null)
    {
        #region Generate token info

        byte[] tokenArr = RandomNumberGenerator.GetBytes(64);
        string token = Convert.ToBase64String(tokenArr);

        expiry ??= DateTime.UtcNow.AddDays(3);

        #endregion

        #region Store to database

        await using var connection = new NpgsqlConnection(connStr);
        var newToken = new TokenModel
        {
            Username = username,
            Token = token,
            Expiry = expiry.Value
        };
        await connection.InsertAsync(newToken);

        #endregion

        return token;
    }

    private async Task<(bool, string)> GetUsername(string token)
    {
        #region Query database

        await using var connection = new NpgsqlConnection(connStr);
        List<TokenModel> tokens = (await connection.QueryAsync<TokenModel>("SELECT * FROM tokens WHERE Token = @Token",
            new
            {
                Token = token
            })).ToList();

        #endregion

        #region Delete invalid tokens

        foreach (TokenModel tk in tokens.Where(tk => DateTime.Compare(DateTime.UtcNow, tk.Expiry) > 0))
        {
            await connection.DeleteAsync(new TokenModel { Id = tk.Id });
            tokens.Remove(tk);
        }

        #endregion
        
        return tokens.Count is 0 ? (false, "") : (true, tokens[0].Username);
    }

    #endregion

    private static byte[] HashPbkdf2(string password, byte[] salt) => KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        KeyDerivationPrf.HMACSHA512,
        iterationCount: 215_000,
        numBytesRequested: 512 / 8);
}