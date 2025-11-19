using System.Net;
using System.Text.Json.Nodes;
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
            }
            catch(Exception ex)
            {
                e.Respond(HttpStatusCode.InternalServerError, new JsonObject { ["success"] = false, ["reason"] = ex.Message });
            }
        }
        
        e.Responded = true;
    }
}