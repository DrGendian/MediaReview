using MediaReview.System;

namespace MediaReview.Model;

public class Media : Atom, IAtom
{
  
    public virtual void BeginEdit(Session session)
    {
        _VerifySession(session);
    }

    public Media Get()
    {
        return this;
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