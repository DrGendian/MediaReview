using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace MediaReview.Server
{
    public class HttpRestEventArgs: EventArgs
    {
        public HttpRestEventArgs(HttpListenerContext context) {
            Context = context;
            Method = HttpMethod.Parse(context.Request.HttpMethod);
            Path = context.Request.Url?.AbsolutePath ?? string.Empty;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Received: {Method} {Path}");

            if (context.Request.HasEntityBody)
            {
                using Stream input = context.Request.InputStream;
                using StreamReader reader = new(input, context.Request.ContentEncoding);
                Body = reader.ReadToEnd();
                Content = JsonNode.Parse(Body)?.AsObject() ?? new JsonObject();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Body);
            }
            else
            {
                Body = string.Empty;
                Content = new JsonObject();
            }
        }

        public HttpListenerContext Context { get; }
        public HttpMethod Method { get; }

        public string Path { get; }

        public string Body { get; }

        public JsonObject Content { get; }

        public bool Responded { get; set; } = false;

        public void Respond(HttpStatusCode statusCode, JsonObject? content)
        {
            HttpListenerResponse response = Context.Response;
            response.StatusCode = (int)statusCode;
            string rstr = content?.ToString() ?? string.Empty;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Responding: {statusCode}: {rstr}");

            byte[] buf = Encoding.UTF8.GetBytes( rstr );
            response.ContentLength64 = buf.Length;
            response.ContentType = "application/json; charset=UTF-8";

            using Stream output = response.OutputStream;
            output.Write( buf, 0, buf.Length );
            output.Close();

            Responded = true;
        }
    }
}
