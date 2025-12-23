using System.Collections;
using MediaReview.Model;
using MediaReview.System;
using Npgsql;

namespace MediaReview.Repository;

public struct UserRating
{
    public string username;
    public int rating_count;
}

public class LeaderboardRepository: RepositoryBase
{
    public static List<UserRating> Get()
    {
        List<UserRating> leaderboard = new List<UserRating>();
        
        var sql = "SELECT username, COUNT(media_id) AS rating_count from users JOIN rating ON users.id = rating.user_id GROUP BY username ORDER BY rating_count DESC";
        using var cmd = new NpgsqlCommand(sql, (NpgsqlConnection)_Cn);
        using var re = cmd.ExecuteReader();
        while (re.Read())
        {
            UserRating user = new UserRating();
            user.username = re.GetString(re.GetOrdinal("username"));
            user.rating_count = re.GetInt32(re.GetOrdinal("rating_count"));
            leaderboard.Add(user);
        }
        
        
        return leaderboard;
    }
}