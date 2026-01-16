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

    public object? GetFilter(string? title, Media.MediaType? mediaType, string? genreName, int? releaseYear, int? ageRestriction, int? rating, string? sortBy)
    {
        

        return new Media();
    }

    public List<Media> GetAll(string filterTitle, string filterGenre, string filterMediaType, string filterReleaseYear, string filterAgeRestriction, string  filterRating, string sortBy)
    {
        var  sql = "SELECT * FROM media_entry";
        return new List<Media>();
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
    private int GetOrCreateGenreId(string genreName, NpgsqlConnection conn, NpgsqlTransaction trans)
    {
        var sql = "INSERT INTO genre (name) VALUES (@name) " +
                  "ON CONFLICT (name) DO UPDATE SET name = EXCLUDED.name " +
                  "RETURNING id;";

        using var cmd = new NpgsqlCommand(sql, conn, trans);
        cmd.Parameters.AddWithValue("@name", genreName);

        return (int)cmd.ExecuteScalar();
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
        var media = (Media)obj;
        var conn = (NpgsqlConnection)_Cn;

        if (string.IsNullOrWhiteSpace(media.title))
            throw new InvalidOperationException("Title must not be empty.");
        if (string.IsNullOrWhiteSpace(media.description))
            throw new InvalidOperationException("Description must not be empty.");

        if (conn.State != ConnectionState.Open) conn.Open();

        using var transaction = conn.BeginTransaction();

        try
        {
            int mediaId;

            if (!Exists(media.title))
            {
                var sql = "INSERT INTO media_entry (creator_id, title, description, media_type, release_year, age_restriction) " +
                          "VALUES (@creator_id, @title, @description, @media_type, @release_year, @age_restriction) RETURNING id";

                using var cmd = new NpgsqlCommand(sql, conn, transaction);

                cmd.Parameters.AddWithValue("@creator_id", GetUserId(media.ownerName, conn));
                cmd.Parameters.AddWithValue("@title", media.title);
                cmd.Parameters.AddWithValue("@description", media.description);
                cmd.Parameters.AddWithValue("@media_type", (int)media.mediaType);
                cmd.Parameters.AddWithValue("@release_year", media.releaseYear);
                cmd.Parameters.AddWithValue("@age_restriction", media.ageRestriction);

                mediaId = (int)cmd.ExecuteScalar();
            }
            else
            {
                var sql = "UPDATE media_entry SET " +
                          "description = @description, " +
                          "media_type = @media_type, " +
                          "release_year = @release_year, " +
                          "age_restriction = @age_restriction " +
                          "WHERE title = @title RETURNING id";

                using var cmd = new NpgsqlCommand(sql, conn, transaction);
                cmd.Parameters.AddWithValue("@title", media.title);
                cmd.Parameters.AddWithValue("@description", media.description);
                cmd.Parameters.AddWithValue("@media_type", (int)media.mediaType);
                cmd.Parameters.AddWithValue("@release_year", media.releaseYear);
                cmd.Parameters.AddWithValue("@age_restriction", media.ageRestriction);

                object result = cmd.ExecuteScalar();
                if (result == null) throw new Exception("Media not found for update.");
                mediaId = (int)result;

                using var delCmd = new NpgsqlCommand("DELETE FROM media_genre WHERE media_id = @mid", conn, transaction);
                delCmd.Parameters.AddWithValue("@mid", mediaId);
                delCmd.ExecuteNonQuery();
            }

            if (media.genres != null)
            {
                foreach (var genreName in media.genres.Distinct())
                {
                    if (string.IsNullOrWhiteSpace(genreName)) continue;
                    int genreId = GetOrCreateGenreId(genreName, conn, transaction);

                    using var gcmd = new NpgsqlCommand("INSERT INTO media_genre (media_id, genre_id) VALUES (@m, @g)", conn, transaction);
                    gcmd.Parameters.AddWithValue("@m", mediaId);
                    gcmd.Parameters.AddWithValue("@g", genreId);
                    gcmd.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
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