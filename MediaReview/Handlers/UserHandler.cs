using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json;
using MediaReview.Server;
using MediaReview.System;
using MediaReview.Model;

namespace MediaReview.Handlers;

public class UserHandler: Handler, IHandler
{
    
    public override void Handle(HttpRestEventArgs e)
    {
        if (e.Path == "/register" && e.Method == HttpMethod.Post)
        {
            try
            {
                string username = e.Content["username"]?.GetValue<string>() ?? "";
                string password = e.Content["password"]?.GetValue<string>() ?? "";
                
                if(User.Exists(username))
                {
                    e.Respond(HttpStatusCode.Conflict, new JsonObject { ["success"] = false, ["reason"] = "User already exists" });
                    return;
                }

                User.Create(username, password);
                
                e.Respond(HttpStatusCode.OK, new JsonObject { ["success"] = true });
                e.Responded = true;
            }
            catch(Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                e.Responded = true;
            }
        }else if (e.Path == "/users" && e.Method == HttpMethod.Get)
        {
            try
            {
                string token = e.Context.Request.Headers["Authorization"]?.Replace("Bearer ", "") ?? "";
                    
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine($"[{nameof(VersionHandler)}] No token provided.");
                    throw new ArgumentException("No token provided.");
                }
                Session? session = Session.Get(token);

                if (!session.Valid)
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
                
                e.Responded = true;
                
            }
            catch (Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
                e.Responded = true;
            }
            
        }
        
        
    }
}