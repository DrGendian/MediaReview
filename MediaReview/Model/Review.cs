using MediaReview.System;

namespace MediaReview.Model;
/*
public class Review : Atom, IAtom
{
    protected Session? _EditingSession = null;

    private int id;
    private string title;
    private string review;
    private int rating = 0;
    private Media media;
    private Session _Session;

    public Review(string title, string review, Media media, Session session)
    {
       this.title = title;
       this.review = review;
       this.media = media;
       _Session = session;
    }
    
    protected void _VerifySession(Session? session = null)
    {
        if(session is not null) { _EditingSession = session; }
        if(_EditingSession is null || !_EditingSession.Valid) { throw new UnauthorizedAccessException("Invalid session."); }
    }
        
    protected void _EndEdit()
    {
        _EditingSession = null;
    }
        
    protected void _EnsureAdmin()
    {
        _VerifySession();
        if(!_EditingSession!.IsAdmin) { throw new UnauthorizedAccessException("Admin privileges required."); }
    }
        
    protected void _EnsureAdminOrOwner(string owner)
    {
        _VerifySession();
        if(!(_EditingSession!.IsAdmin || (_EditingSession.UserName == owner)))
        {
            throw new UnauthorizedAccessException("Admin or owner privileges required.");
        }
    }
        
    public virtual void BeginEdit(Session session)
    {
        _VerifySession(session);
    }

    public override void Save()
    {
        
    }

    public override void Delete()
    {
        
    }

    public override void Refresh()
    {
        
    }
}*/