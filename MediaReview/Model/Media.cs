using MediaReview.System;

namespace MediaReview.Model;

public class Media : Atom, IAtom
{
    protected Session? _EditingSession = null;
        
        
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
}