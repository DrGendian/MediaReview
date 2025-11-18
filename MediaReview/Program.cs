using MediaReview.Server;
using MediaReview.Handlers;

namespace MediaReview
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpRestServer server = new();
            server.RequestReceived += Handler.HandleEvent;
            server.Run();
        }
    }
}
