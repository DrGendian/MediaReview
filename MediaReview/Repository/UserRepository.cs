using System.Collections;
using System.Data;
using MediaReview.Model;
using MediaReview.System;
using Npgsql;

namespace MediaReview.Repository;

public class UserRepository: RepositoryBase, IRepository
{
    public object? Get(object id, Session? session = null)
    {
        var sql = "SELECT * FROM Users WHERE username = @username";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("username", id);
        using var re = cmd.ExecuteReader();
        if(re.Read())
        {
            User user = new User();
            user._userId = re.GetInt32("id");
            user._UserName = re.GetString("username");
            user.FullName =  re.GetString("name");
            user.EMail = re.GetString("email");
            user.isAdmin = re.GetBoolean("hadmin");
            return user;
        }

        return null;
    }

    public IEnumerable GetAll(Session? session = null)
    {
        return null;
    }

    public bool checkPassword(string username, string password_hash)
    {
        var sql = "SELECT password_hash FROM Users WHERE username = @username";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@username", username);
        using var re = cmd.ExecuteReader();
        
        if (re.Read())
        {
            return (re.GetString("password_hash") == password_hash);
        }
        return false;
    }

    public bool Exists(string username)
    {
        var sql = "SELECT * FROM Users WHERE username = @username";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@username", username);
        using var re = cmd.ExecuteReader();
        if (re.Read())
        {
            return true;
        }
        return false;
    }

    public void Refresh(object obj)
    {
        
    }

    public void Save(object obj)
    {
        if (!Exists(((User)obj)._UserName))
        {
            if(string.IsNullOrWhiteSpace(((User)obj)._UserName))
            {
                throw new InvalidOperationException("User name must not be empty.");
            }
            if(string.IsNullOrWhiteSpace(((User) obj)._PasswordHash))
            {
                throw new InvalidOperationException("Password must not be empty.");
            }
            
            var sql = "INSERT INTO Users (username, password_hash, name, email, hadmin) VALUES (@username, @password,  @name, @email, @hadmin)";
            using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
            cmd.Parameters.AddWithValue("@username", ((User)obj)._UserName);
            cmd.Parameters.AddWithValue("@password", ((User)obj)._PasswordHash);
            cmd.Parameters.AddWithValue("@name", ((User)obj).FullName);
            cmd.Parameters.AddWithValue("@email", ((User)obj).EMail);
            cmd.Parameters.AddWithValue("@hadmin", false);
            cmd.ExecuteNonQuery();
        }
        else
        {
            string pwdPart = string.IsNullOrWhiteSpace(((User)obj)._PasswordHash)
                ? string.Empty
                : "password_hash = @password, ";

            var sql =
                $"UPDATE users SET " +
                $"name = @name, {pwdPart}email = @email, hadmin = @hadmin " +
                $"WHERE username = @username";

            using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);

            cmd.Parameters.AddWithValue("@name", ((User)obj).FullName);
            cmd.Parameters.AddWithValue("@email", ((User)obj).EMail);
            cmd.Parameters.AddWithValue("@hadmin", ((User)obj).isAdmin);
            cmd.Parameters.AddWithValue("@username", ((User)obj)._UserName);

            if (!string.IsNullOrWhiteSpace(((User)obj)._PasswordHash))
            {
                cmd.Parameters.AddWithValue("@password", ((User)obj)._PasswordHash);
            }

            cmd.ExecuteNonQuery();
        }
    }

    public void Delete(object obj)
    {
        
    }

    public List<string> GetFavorites(int userId)
    {
        var sql = "SELECT user_id, me.title FROM favorite JOIN media_entry me ON media_id = me.id WHERE user_id = @userId";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@userId", userId);
        using var re = cmd.ExecuteReader();

        List<string> favorites = new();
        while (re.Read())
        {
            favorites.Add(re.GetString("title"));
        }
        
        return favorites;
    }
}