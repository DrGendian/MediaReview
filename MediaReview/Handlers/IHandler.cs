using MediaReview.Server;

namespace MediaReview.Handlers;

public interface IHandler
{
    public void Handle(HttpRestEventArgs e);
}

