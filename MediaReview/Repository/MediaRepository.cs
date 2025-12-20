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
        if (!Exists((int)id))
        {
            throw new KeyNotFoundException("Media not found");
        }
        var sql = @"
                SELECT
            me.id,
            me.title,
            me.description,
            me.media_type,
            me.release_year,
            me.age_restriction,
            me.avg_score,
            g.id   AS genre_id,
            g.name AS genre_name,
            u.username AS username
        FROM media_entry me
        JOIN media_genre mg ON me.id = mg.media_id
        JOIN genre g       ON g.id = mg.genre_id
        JOIN users u ON u.id = me.creator_id
        WHERE me.id = @id;
    ";

        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = cmd.ExecuteReader();

        Media? media = null;

        while (reader.Read())
        {
            if (media == null)
            {
                media = new Media
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    ownerName = reader.GetString(reader.GetOrdinal("username")),
                    title = reader.GetString(reader.GetOrdinal("title")),
                    description = reader.GetString(reader.GetOrdinal("description")),
                    releaseYear = reader.GetInt32(reader.GetOrdinal("release_year")),
                    ageRestriction = reader.GetInt32(reader.GetOrdinal("age_restriction")),
                    mediaType = (Media.MediaType)reader.GetInt32(reader.GetOrdinal("media_type")),
                    avg_score = reader.GetDouble(reader.GetOrdinal("avg_score"))
                };
            }

            media.genres.Add(
                reader.GetString(reader.GetOrdinal("genre_name"))
            );
        }

        return media;
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
    
    public bool Exists(int id)
    {
        var sql = "SELECT * FROM media_entry WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@id", id);
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
            
            int mediaId = (int)cmd.ExecuteScalar();
            var genres = ((Media)obj).genres;
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
            var sql = "UPDATE media_entry SET " +
                      "title = @title, " +
                      "description = @description, " +
                      "media_type = @media_type, " +
                      "release_year = @release_year, " +
                      "age_restriction = @age_restriction " +
                      "WHERE title = @title RETURNING id";
            using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
            cmd.Parameters.AddWithValue("@title", ((Media)obj).title);
            cmd.Parameters.AddWithValue("@description", ((Media)obj).description);
            cmd.Parameters.AddWithValue("@media_type", (int)((Media)obj).mediaType);
            cmd.Parameters.AddWithValue("@release_year", ((Media)obj).releaseYear);
            cmd.Parameters.AddWithValue("@age_restriction", ((Media)obj).ageRestriction);
            
            cmd.ExecuteScalar();
        }
        
    }

    public void Delete(object obj)
    { 
        var sql = "DELETE FROM media_entry WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@id", ((Media)obj).id);
        cmd.ExecuteNonQuery();
    }

    public void Favorite(int id, int userId)
    {
        var sql = "INSERT INTO favorite (user_id, media_id) VALUES (@user_id, @media_id)";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@user_id", userId);
        cmd.Parameters.AddWithValue("@media_id", id);
        cmd.ExecuteNonQuery();
    }
    
    public void DeleteFavorite(int id, int userId)
    {
        var sql = "DELETE FROM favorite WHERE user_id = @user_id AND media_id = @media_id";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@user_id", userId);
        cmd.Parameters.AddWithValue("@media_id", id);
        cmd.ExecuteNonQuery();
    }
}