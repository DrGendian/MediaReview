using System.Net;
using System.Text.Json.Nodes;
using MediaReview.Server;
using MediaReview.System;
using MediaReview.Model;
using System.Text.RegularExpressions;
/*
namespace MediaReview.Handlers
{
    internal class MediaHandler : Handler, IHandler
    {
        private const string RoutePrefix = "/api/media";

        public override void Handle(HttpRestEventArgs e)
        {
            if (e.Path == $"{RoutePrefix}" && e.Method == HttpMethod.Get)
            {
                try
                {
                    throw new NotImplementedException();

                    string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";

                    if (string.IsNullOrWhiteSpace(token))
                    {
                        Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                        throw new ArgumentException("No token provided.");
                    }

                    Session.VerifySession(token);
                    
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError,
                        new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"[{nameof(VersionHandler)}] Exception getting media. {e.Method} {e.Path}: {ex.Message}");
                    e.Responded = true;
                }
                
            }
            else if(e.Path == $"{RoutePrefix}" && e.Method == HttpMethod.Post)
            {
                try
                {
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
            }else if (Regex.Match(e.Path, @"^/api/media/(?<id>[^/]+)$").Success && e.Method == HttpMethod.Get)
            {
                try
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

                    Session session = Session.Get(token);
                    
                    Media media = Media.GetMedia(mediaId, session);
                    
                    if (media == null)
                    {
                        e.Respond(HttpStatusCode.NotFound, new JsonObject
                        {
                            ["success"] = false,
                            ["reason"] = "Media not found"
                        });
                        e.Responded = true;
                        return;
                    }
                    
                    e.Respond(HttpStatusCode.OK, new JsonObject
                    {
                        ["success"] = true,
                        ["content"] = new JsonObject
                        {
                            ["id"] = media.id,
                            ["title"] = media.title,
                            ["description"] = media.description,
                            ["genres"] = new JsonArray(media.genres?.Select(g => JsonValue.Create(g)).ToArray() ?? Array.Empty<JsonNode>()),
                            ["mediaType"] = media.mediaType,
                            ["releaseYear"] = media.releaseYear,
                            ["ageRestriction"] = media.ageRestriction
                        }
                    });
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{nameof(VersionHandler)}] Get media success.");
                    e.Responded = true;

                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError,
                        new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"[{nameof(VersionHandler)}] Exception getting media. {e.Method.ToString()} {e.Path}: {ex.Message}");
                    e.Responded = true;
                }
                
            }else if (Regex.Match(e.Path, @"^/api/media/(?<id>[^/]+)$").Success && e.Method == HttpMethod.Put)
            {
                try
                {
                    string title = e.Content["title"]?.GetValue<string>() ?? "";
                    string description = e.Content["description"]?.GetValue<string>() ?? "";
                    string mediaType = e.Content["mediaType"]?.GetValue<string>() ?? "";
                    int releaseYear = e.Content["releaseYear"]?.GetValue<int>() ?? 0;
                    string[] genres = e.Content["genres"] is JsonArray genresArray 
                        ? genresArray.Select(g => g?.GetValue<string>() ?? "").ToArray() 
                        : Array.Empty<string>();

                    int ageRestriction = e.Content["ageRestriction"]?.GetValue<int>() ?? 0;
                    
                    var match = Regex.Match(e.Path, @"^/api/media/(?<id>[^/]+)$");
                    int mediaId = int.Parse(match.Groups["id"].Value);
                    
                    string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                    
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                        throw new ArgumentException("No token provided.");
                    }

                    Session.VerifySession(token);

                    Session session = Session.Get(token);
                    
                    Media media = Media.GetMedia(mediaId, session);

                    if (media == null)
                    {
                        e.Respond(HttpStatusCode.NotFound, new JsonObject
                        {
                            ["success"] = false,
                            ["reason"] = "Media not found"
                        });
                        e.Responded = true;
                        return;
                    }
                    media.BeginEdit(session);
                    if(!string.IsNullOrWhiteSpace(title)) media.title = title;
                    if(!string.IsNullOrWhiteSpace(description)) media.description = description;
                    if(genres != Array.Empty<string>()) media.genres = genres;
                    if(ageRestriction != 0) media.ageRestriction = ageRestriction;
                    if(!string.IsNullOrWhiteSpace(mediaType)) media.mediaType = mediaType;
                    if(releaseYear != 0) media.releaseYear = releaseYear;
                    media.Save();
                    
                    e.Respond(HttpStatusCode.OK, new JsonObject { ["success"] = true, ["description"] = "Media entry updated." });
                    e.Responded = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{nameof(VersionHandler)}] Media updated.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError,
                        new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"[{nameof(VersionHandler)}] Exception updating media. {e.Method.ToString()} {e.Path}: {ex.Message}");
                    e.Responded = true;
                }
            }else if (Regex.Match(e.Path, @"^/api/media/(?<id>[^/]+)$").Success && e.Method == HttpMethod.Delete)
            {
                try
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

                    Session session = Session.Get(token);
                    
                    Media media = Media.GetMedia(mediaId, session);
                    
                    media.BeginEdit(session);
                    media.Delete();

                    e.Respond(HttpStatusCode.OK, new JsonObject { ["success"] = true, ["description"] = "Media entry deleted." });
                    e.Responded = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{nameof(VersionHandler)}] Media deleted.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError,
                        new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"[{nameof(VersionHandler)}] Exception deleting media. {e.Method.ToString()} {e.Path}: {ex.Message}");
                    e.Responded = true;
                }
            }
        }
    }
}*/
