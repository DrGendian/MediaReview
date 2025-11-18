using System.Net.NetworkInformation;
using System.Reflection;
using MediaReview.Server;

namespace MediaReview.Handlers
{
    public abstract class Handler: IHandler
    {
        private static List<IHandler>? _Handlers = null;

        private static List<IHandler> _GetHandlers()
        {
            List<IHandler> rval = new();

            foreach (Type i in Assembly.GetExecutingAssembly().GetTypes()
                .Where(m => m.IsAssignableTo(typeof(IHandler)) && !m.IsAbstract))
            {
                IHandler? h = (IHandler?) Activator.CreateInstance(i);
                if (h is not null) { rval.Add(h); }
            }

            return rval;
        }

        public static void HandleEvent(object? sender, HttpRestEventArgs e)
        {
            foreach (IHandler i in (_Handlers ??= _GetHandlers()))
            {
                i.Handle(e);
                if (e.Responded) break;
            }
        }
        public abstract void Handle(HttpRestEventArgs e);
    }

}
