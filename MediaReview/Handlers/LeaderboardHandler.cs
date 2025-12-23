using System.Net;
using System.Text.Json.Nodes;
using MediaReview.Repository;
using MediaReview.Server;
using MediaReview.System;

namespace MediaReview.Handlers;

public class LeaderboardHandler: Handler, IHandler
{
    private const string RoutePrefix = "/api/leaderboard";
    public override void Handle(HttpRestEventArgs e)
    {
        if (e.Path.StartsWith(RoutePrefix) && e.Method == HttpMethod.Get)
        {
            try
            {
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                        
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                    throw new ArgumentException("No token provided.");
                }
                Session.VerifySession(token);

                List<UserRating> leaderboard = LeaderboardRepository.Get();
                
                var leaderboardJson = new JsonArray();
                foreach (var r in leaderboard)
                {
                    leaderboardJson.Add(new JsonObject
                    {
                        ["username"] = r.username,
                        ["rating_count"] = r.rating_count
                    });
                }

                e.Respond(HttpStatusCode.OK, new JsonObject
                {
                    ["success"] = true,
                    ["leaderboard"] = leaderboardJson
                });
                
                e.Responded = true;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{nameof(VersionHandler)}] Got leaderboard. {e.Method.ToString()} {e.Path}.");
            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError,
                    new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    $"[{nameof(VersionHandler)}] Exception getting leaderboard. {e.Method.ToString()} {e.Path}: {ex.Message}");
                e.Responded = true;
            }
        }  
    }
}