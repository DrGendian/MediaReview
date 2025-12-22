using System.Collections;
using MediaReview.Model;
using MediaReview.System;
using Npgsql;

namespace MediaReview.Repository;

public class RatingRepository: RepositoryBase, IRepository
{
    public object? Get(object id, Session? session = null)
    {
        return null;
    }

    public IEnumerable GetAll(Session? session)
    {
        return null;
    }

    public void Refresh(object obj)
    {
        
    }

    public void Save(object obj)
    {
        var sql = "INSERT INTO rating (user_id, media_id, stars, comment) VALUES (@user_id, @media_id, @stars, @comment)";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        cmd.Parameters.AddWithValue("@user_id", ((Rating)obj).user_id);
        cmd.Parameters.AddWithValue("@media_id", ((Rating)obj).media_id);
        cmd.Parameters.AddWithValue("@stars", ((Rating)obj).stars);
        cmd.Parameters.AddWithValue("@comment", ((Rating)obj).comment);
        cmd.ExecuteNonQuery();

        sql = "UPDATE media_entry me SET avg_score = r.avg_score FROM (SELECT media_id, AVG(stars) AS avg_score FROM rating GROUP BY media_id) r WHERE me.id = @media_id";
        using var rcmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        rcmd.Parameters.AddWithValue("@media_id", ((Rating)obj).media_id);
        rcmd.ExecuteNonQuery();
    }

    public void Delete(object obj)
    {
        
    }
}