using MediaReview.Server;
using System.Net;
using System.Text.Json.Nodes;
using MediaReview.Server;
using MediaReview.System;
using MediaReview.Model;
using System.Text.RegularExpressions;

namespace MediaReview.Handlers;

public class RatingsHandler: Handler, IHandler
{
    private const string RoutePrefix = "/api/media";
    
    public override void Handle(HttpRestEventArgs e)
    {

        if (Regex.Match(e.Path, @"^/api/media/(?<id>[^/]+)/rate$").Success && e.Method == HttpMethod.Post)
        {
            try
            {
                var match = Regex.Match(e.Path, @"^/api/media/(?<id>[^/]+)/rate$");
                int mediaId = int.Parse(match.Groups["id"].Value);

                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                    throw new ArgumentException("No token provided.");
                }

                Session.VerifySession(token);
                Session session = Session.Get(token);
                
                int stars = e.Content["stars"]?.GetValue<int>() ?? -1;
                string comment = e.Content["comment"]?.GetValue<string>() ?? "";

                if (stars == -1)
                {
                    throw new ArgumentException("No stars provided.");
                }
                
                Rating rating = new Rating(stars, comment, session.UserId, mediaId);
                rating.Save();
                
                e.Respond(HttpStatusCode.OK,
                    new JsonObject() { ["success"] = "true" });
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[{nameof(VersionHandler)}] Handled {e.Method.ToString()} {e.Path}.");
                
                e.Responded = true;
            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError,
                    new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    $"[{nameof(VersionHandler)}] Exception creating rating. {e.Method.ToString()} {e.Path}: {ex.Message}");
                e.Responded = true;
            }
            
            
        }
        else if (Regex.Match(e.Path, @"^/api/ratings/(?<id>[^/]+)/like$").Success && e.Method == HttpMethod.Get)
        {
            var match = Regex.Match(e.Path, @"^/api/media/(?<id>[^/]+)$");
            int mediaId = int.Parse(match.Groups["id"].Value);
                    
            string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                throw new ArgumentException("No token provided.");
            }

            Session.VerifySession(token);
            
            
        }
    }
}