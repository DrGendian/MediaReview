using System.Collections;
using MediaReview.Model;
using MediaReview.System;
using Npgsql;

namespace MediaReview.Repository;

public class RatingRepository: RepositoryBase, IRepository
{
    public object? Get(object id, Session session)
    {
        ArgumentNullException.ThrowIfNull(session);

        const string sql = "SELECT * FROM rating WHERE media_id = @id AND user_id = @user_id";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@user_id", session.UserId);
        using var re = cmd.ExecuteReader();
        if (!re.Read()) return null;
        var rating = new Rating
        {
            stars = re.GetInt32(re.GetOrdinal("stars")),
            comment = re.GetString(re.GetOrdinal("comment")),
            comment_confirmed = re.GetBoolean(re.GetOrdinal("comment_confirmed")),
            user_id =  re.GetInt32(re.GetOrdinal("user_id")),
            media_id = re.GetInt32(re.GetOrdinal("media_id")),
        };
        return rating;
    }

    public IEnumerable GetAll(Session? session)
    {
        return null;
    }

    public bool Exists(int media_id, int user_id)
    {
        var sql = "SELECT * FROM rating WHERE media_id = @media_id AND user_id = @user_id";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@media_id", media_id);
        cmd.Parameters.AddWithValue("@user_id", user_id);
        using var re = cmd.ExecuteReader();
        if (re.Read())
        {
            return true;
        }
        return false;
    }

    public void Refresh(object obj)
    {
        throw new NotImplementedException();
    }

    public void Save(object obj)
    {
        Rating r = (Rating)obj;
        if (!Exists(r.media_id, r.user_id))
        {
            var sql = "INSERT INTO rating (user_id, media_id, stars, comment) VALUES (@user_id, @media_id, @stars, @comment)";
            using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
            cmd.Parameters.AddWithValue("@user_id", r.user_id);
            cmd.Parameters.AddWithValue("@media_id", r.media_id);
            cmd.Parameters.AddWithValue("@stars", r.stars);
            cmd.Parameters.AddWithValue("@comment", r.comment);
            cmd.ExecuteNonQuery();

            sql = "UPDATE media_entry me SET avg_score = r.avg_score FROM (SELECT media_id, AVG(stars) AS avg_score FROM rating GROUP BY media_id) r WHERE me.id = @media_id";
            using var rcmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
            rcmd.Parameters.AddWithValue("@media_id", r.media_id);
            rcmd.ExecuteNonQuery();
        }
        else
        {
            var sql = "UPDATE rating SET stars =  @stars, comment = @comment, comment_confirmed = @confirm WHERE media_id = @media_id AND user_id = @user_id";
            using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
            cmd.Parameters.AddWithValue("@user_id", r.user_id);
            cmd.Parameters.AddWithValue("@media_id", r.media_id);
            cmd.Parameters.AddWithValue("@stars", r.stars);
            cmd.Parameters.AddWithValue("@comment", r.comment);
            cmd.Parameters.AddWithValue("@confirm", r.comment_confirmed);
            cmd.ExecuteNonQuery();
        
            sql = "UPDATE media_entry me SET avg_score = r.avg_score FROM (SELECT media_id, AVG(stars) AS avg_score FROM rating GROUP BY media_id) r WHERE me.id = @media_id";
            using var rcmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
            rcmd.Parameters.AddWithValue("@media_id", r.media_id);
            rcmd.ExecuteNonQuery();
        }
    }


    public void Delete(object obj)
    {
        
    }
}