using System.Net;

namespace MediaReview.Server
{
    internal class HttpRestServer : IDisposable
    {
        private readonly HttpListener _Listener;

        public HttpRestServer(int port = 12000)
        {
            _Listener = new();
            _Listener.Prefixes.Add($"http://+:{port}/api/");
        }

        public event EventHandler<HttpRestEventArgs>? RequestReceived;

        public bool Running
        {
            get; private set;
        }

        public void Run()
        {
            if (Running) { return; }

            _Listener.Start();
            Running = true;

            Console.Write("Server listening");

            while (Running) {
                HttpListenerContext context = _Listener.GetContext();

                _ = Task.Run(() =>
                {
                    HttpRestEventArgs args = new(context);
                    RequestReceived?.Invoke(this, args);

                    if (!args.Responded)
                    {
                        args.Respond(HttpStatusCode.NotFound, new() { ["success"] = false, ["reason"] = "Not found" });
                    }
                });
            }
        }


        public void Stop()
        {
            _Listener.Close();
            Running = false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
