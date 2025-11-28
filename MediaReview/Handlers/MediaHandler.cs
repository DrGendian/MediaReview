using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json;
using MediaReview.Server;
using MediaReview.System;
using MediaReview.Model;

namespace MediaReview.Handlers
{
    internal class MediaHandler : Handler, IHandler
    {
        private const string RoutePrefix = "/media";

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

            }
        }
    }
}
