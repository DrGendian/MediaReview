using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.RegularExpressions;
using MediaReview.Server;
using MediaReview.System;
using MediaReview.Model;

namespace MediaReview.Handlers;

public class UserHandler: Handler, IHandler
{
    private const string RoutePrefix = "/api/users";
    public override void Handle(HttpRestEventArgs e)
    {
        if (e.Path == $"{RoutePrefix}/register" && e.Method == HttpMethod.Post)
        {
            try
            {
                string username = e.Content["username"]?.GetValue<string>() ?? "";
                string password = e.Content["password"]?.GetValue<string>() ?? "";
                
                if(User.Exists(username))
                {
                    e.Respond(HttpStatusCode.Conflict, new JsonObject { ["success"] = false, ["reason"] = "User already exists" });
                    e.Responded = true;
                    return;
                }
                
                User user = new User();
                user._UserName = username;
                user.SetPassword(password);
                user.Save();
                    
                e.Respond(HttpStatusCode.Created, new JsonObject { ["success"] = true, ["description"] = "User registered." });
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{nameof(VersionHandler)}] User registration success.");
                e.Responded = true;
            }
            catch(Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                e.Responded = true;
            }
        }else if (e.Path == $"{RoutePrefix}/users" && e.Method == HttpMethod.Get)
        {
            try
            {
                throw new NotImplementedException();
                
                /*
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                    
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                    throw new ArgumentException("No token provided.");
                }
                Session? session = Session.Get(token);

                if (session == null || !session.Valid)
                {
                    e.Respond(HttpStatusCode.Unauthorized,
                        new JsonObject() { ["success"] = false, ["reason"] = "Invalid or expired session." });
                    return;
                }

                if (!session.IsAdmin)
                {
                    e.Respond(HttpStatusCode.Unauthorized,new JsonObject()
                        { ["success"] = false,["reason"] = "You do not have permission to use this command." });
                }

                var userList = User.GetAll(session);
                
                e.Respond(HttpStatusCode.OK, new JsonObject { ["success"] = true, ["content"] = userList });
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{nameof(VersionHandler)}] User list success.");
                e.Responded = true;*/
                
            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                e.Responded = true;
            }
            
        }else if ((Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/profile$")).Success && e.Method == HttpMethod.Get)
        {
            try
            {
                var match = Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/profile$");
                string userId = match.Groups["id"].Value;
            
            
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                    
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                    throw new ArgumentException("No token provided.");
                }
                Session? session = Session.Get(token);
            
                if (session == null || !session.Valid)
                {
                    e.Respond(HttpStatusCode.Unauthorized,
                        new JsonObject() { ["success"] = false, ["reason"] = "Invalid or expired session." });
                    e.Responded = true;
                    return;
                }
            
                User user = User.Get(userId);

                if (user == null)
                {
                    e.Respond(HttpStatusCode.Unauthorized,
                        new JsonObject() { ["success"] = false, ["reason"] = "User not found." });
                    e.Responded = true;
                    return;
                }
            
                e.Respond(HttpStatusCode.OK, new JsonObject
                {
                    ["success"] = true,
                    ["content"] = new JsonObject
                    {
                        ["id"] = user._userId,
                        ["username"] = user._UserName,
                        ["fullname"] = user.FullName,
                        ["email"] = user.EMail
                    }
                });
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{nameof(VersionHandler)}] User profile success.");
                e.Responded = true;

            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError,
                    new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    $"[{nameof(VersionHandler)}] Exception getting user. {e.Method.ToString()} {e.Path}: {ex.Message}");
                e.Responded = true;
            }
        }else if ((Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/profile$")).Success && e.Method == HttpMethod.Put)
        {
            try
            {
                var match = Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/profile$");
                string userId = match.Groups["id"].Value;
                
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                string fullname = e.Content["fullname"]?.GetValue<string>() ?? "";
                string email = e.Content["email"]?.GetValue<string>() ?? "";
                string password = e.Content["password"]?.GetValue<string>() ?? "";
            
                Session? session = Session.Get(token);

                if (session == null || !session.Valid)
                {
                    e.Respond(HttpStatusCode.Unauthorized,
                        new JsonObject() { ["success"] = false, ["reason"] = "Invalid or expired session." });
                    e.Responded = true;
                    return;
                }
            
                User user = User.Get(userId);

                if (user == null)
                {
                    e.Respond(HttpStatusCode.Unauthorized,
                        new JsonObject() { ["success"] = false, ["reason"] = "User not found." });
                    e.Responded = true;
                    return;
                }
            
                user.BeginEdit(session);
                if (!string.IsNullOrWhiteSpace(fullname)) user.FullName = fullname;
                if (!string.IsNullOrWhiteSpace(email)) user.EMail = email;
                if (!string.IsNullOrWhiteSpace(password)) user.SetPassword(password);
                user.Save();
                
                e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true});
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{nameof(VersionHandler)}] User edit success.");
            
                e.Responded = true;
            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                e.Responded = true;
            }
            
        }else if ((Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/delete$")).Success && e.Method == HttpMethod.Delete)
        {
            try
            {
                var match = Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/delete$");
                string userId = match.Groups["id"].Value;
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                
                Session session = Session.Get(token);

                if (!session.Valid)
                {
                    e.Respond(HttpStatusCode.Unauthorized,
                        new JsonObject() { ["success"] = false, ["reason"] = "Invalid or expired session." });
                    e.Responded = true;
                    return;
                }
                
                User user = User.Get(userId);
                user.BeginEdit(session);
                user.Delete();
                e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true});
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{nameof(VersionHandler)}] User delete success.");
                
                e.Responded = true;
            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                e.Responded = true;
            }
        }
        else if ((Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/favorites$")).Success && e.Method == HttpMethod.Get)
        {
            try
            {
                var match = Regex.Match(e.Path, @"^/api/users/(?<id>[^/]+)/favorites$");
                int userId = int.Parse(match.Groups["id"].Value);
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                        
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                    throw new ArgumentException("No token provided.");
                }
                Session.VerifySession(token);

                List<string> favorites = User.GetFavorites(userId);
                
                e.Respond(HttpStatusCode.OK, new JsonObject
                {
                    ["success"] = true,
                    ["favorites"] = new JsonArray(favorites?.Select(f => JsonValue.Create(f)).ToArray() ??  Array.Empty<JsonValue>())
                });
                
                e.Responded = true;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{nameof(VersionHandler)}] Media unmarked as favorite.");
            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                e.Responded = true;
            }
        }
    }
}