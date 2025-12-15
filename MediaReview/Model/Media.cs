using MediaReview.System;

namespace MediaReview.Model;
/*
public class Media : Atom, IAtom
{
    bool _New;
    public int id { get; private set; }
    public string title { get; set; }= string.Empty;
    public string description { get; set; }= string.Empty;
    public string mediaType { get; set; }= string.Empty;
    public int releaseYear { get; set; }= 0;
    public string[] genres { get; set; }= null;
    public int ageRestriction { get; set; }= 0;
    //int ownerId = 0;
    public string ownerName { get; set; } = string.Empty;

    public Media(Session? session = null)
    {
        _EditingSession = session;
        _New = true;
    }
  
    public virtual void BeginEdit(Session session)
    {
        _VerifySession(session); 
    }

    public static Media? GetMedia(int id, Session session)
    {
        if (session.Valid)
        {
            Media? media = Database.Instance.GetMedia(id);
            if (media == null) return null;
            media._New = false;
            return media;
        }
        return null;
    }
    
    public static bool Exists(int id)
    {
        return Database.Instance.MediaExists(id);
    }

    public override void Save()
    {
        if(!_New) { _EnsureAdminOrOwner(ownerName); }
        Database.Instance.SaveMedia(this);
        _EndEdit();
    }

    public override void Delete()
    {
        _EnsureAdminOrOwner(ownerName);
        Database.Instance.DeleteMedia(id);
    }

    public override void Refresh()
    {
        
    }
}*/