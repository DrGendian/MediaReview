using System.Collections;
using System.Data;
using MediaReview.Model;
using MediaReview.System;
using Npgsql;

namespace MediaReview.Repository;

public class MediaRepository: RepositoryBase, IRepository
{
    public object? Get(object id, Session? session = null)
    {
        return session is null;
    }

    public IEnumerable GetAll(Session? session = null)
    {
        return null;
    }

    public bool Exists(string title)
    {
        var sql = "SELECT * FROM media_entry WHERE title = @title";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@title", title);
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
    private int GetOrCreateGenreId(string genreName, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand(
                   "SELECT id FROM genre WHERE name = @name", conn))
        {
            cmd.Parameters.AddWithValue("@name", genreName);
            var result = cmd.ExecuteScalar();
            Console.WriteLine("Select genre" + result);
            if (result != null)
                return (int)result;
        }
        
        using (var cmd = new NpgsqlCommand(
                   "INSERT INTO genre (name) VALUES (@name) RETURNING id", conn))
        {
            cmd.Parameters.AddWithValue("@name", genreName);
            Console.WriteLine("Create genre" + (int)cmd.ExecuteScalar());
            return (int)cmd.ExecuteScalar();
        }
    }

    private int GetUserId(string username, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand(
                   "SELECT id FROM users WHERE username = @username", conn))
        {
            cmd.Parameters.AddWithValue("@username", username);
            var result = cmd.ExecuteScalar();
            if (result != null)
                return (int)result;
        }

        return 0;
    }
    
    public void Save(object obj)
    {
        if (!Exists(((Media)obj).title))
        {
            if(string.IsNullOrWhiteSpace(((Media)obj).title))
            {
                throw new InvalidOperationException("User name must not be empty.");
            }
            if(string.IsNullOrWhiteSpace(((Media) obj).description))
            {
                throw new InvalidOperationException("Password must not be empty.");
            }
            
            var sql = "INSERT INTO media_entry (creator_id, title, description, media_type, release_year, age_restriction) VALUES (@creator_id, @title, @description, @media_type, @release_year, @age_restriction) RETURNING id";
            using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
            cmd.Parameters.AddWithValue("@creator_id", GetUserId(((Media)obj).ownerName,  (NpgsqlConnection)_Cn));
            cmd.Parameters.AddWithValue("@title", ((Media)obj).title);
            cmd.Parameters.AddWithValue("@description", ((Media)obj).description);
            cmd.Parameters.AddWithValue("@media_type", (int)((Media)obj).mediaType);
            cmd.Parameters.AddWithValue("@release_year", ((Media)obj).releaseYear);
            cmd.Parameters.AddWithValue("@age_restriction", ((Media)obj).ageRestriction);
            
            Console.WriteLine("EXECUTING COMMAND");
            int mediaId = (int)cmd.ExecuteScalar();
            Console.WriteLine("THE MEDIA ID IS " + mediaId);
            var genres = ((Media)obj).genres ?? Array.Empty<string>();
            foreach (var genreName in genres)
            {
                if (string.IsNullOrWhiteSpace(genreName))
                    continue;

                int genreId = GetOrCreateGenreId(genreName, (NpgsqlConnection)_Cn);

                using var gcmd = new NpgsqlCommand("INSERT INTO media_genre (media_id, genre_id) VALUES (@m, @g)", (NpgsqlConnection)_Cn);

                gcmd.Parameters.AddWithValue("@m", mediaId);
                gcmd.Parameters.AddWithValue("@g", genreId);
                gcmd.ExecuteNonQuery();
                Thread.Sleep(100);
            }
        }
        else
        {
            
        }
    }

    public void Delete(object obj)
    {
        
    }
}