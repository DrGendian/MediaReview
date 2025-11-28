using System.Net;
using System.Text.Json.Nodes;
using MediaReview.Server;
using MediaReview.System;
using MediaReview.Model;

namespace MediaReview.Handlers
{
    internal class MediaHandler : Handler, IHandler
    {
        private const string RoutePrefix = "/api/media";

        public override void Handle(HttpRestEventArgs e)
        {
            if (e.Path == $"{RoutePrefix}" && e.Method == HttpMethod.Get)
            {
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";

                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                    throw new ArgumentException("No token provided.");
                }

                Session.VerifySession(token);


            }
            else if(e.Path == $"{RoutePrefix}" && e.Method == HttpMethod.Post)
            {
                try
                {
                    if (e.Content == null)
                    {
                        Console.WriteLine($"[{nameof(VersionHandler)}] No content provided.");
                        throw new ArgumentException("No Media Content provided.");
                    }

                    string title = e.Content["title"]?.GetValue<string>() ?? "";
                    string description = e.Content["description"]?.GetValue<string>() ?? "";
                    string mediaType = e.Content["mediaType"]?.GetValue<string>() ?? "";
                    int releaseYear = e.Content["releaseYear"]?.GetValue<int>() ?? 0;
                    string[] genres = e.Content["genres"] is JsonArray genresArray 
                        ? genresArray.Select(g => g?.GetValue<string>() ?? "").ToArray() 
                        : Array.Empty<string>();

                    int ageRestriction = e.Content["ageRestriction"]?.GetValue<int>() ?? 0;

                    string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";

                    if (string.IsNullOrWhiteSpace(token))
                    {
                        Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                        throw new ArgumentException("No token provided.");
                    }

                    Session.VerifySession(token);

                    Session session = Session.Get(token);

                    Media media = new Media(session);
                    media.ownerName = session.UserName;
                    media.title = title;
                    media.description = description;
                    media.genres = genres;
                    media.ageRestriction = ageRestriction;
                    media.mediaType = mediaType;
                    media.releaseYear = releaseYear;
                    media.Save();
                    
                    e.Respond(HttpStatusCode.Created, new JsonObject { ["success"] = true, ["description"] = "Media entry created." });
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{nameof(VersionHandler)}] Media entry creation success.");
                    e.Responded = true;
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                    e.Responded = true;
                }
            }
        }
    }
}
