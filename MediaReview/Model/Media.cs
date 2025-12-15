using MediaReview.Repository;
using MediaReview.System;

namespace MediaReview.Model;

public class Media : Atom, IAtom
{
    public enum MediaType
    {
       Movie = 1,
       Series,
       Game,
    }
    private static MediaRepository _Repository = new();
    
    protected override IRepository _GetRepository()
    {
        return _Repository;
    }
    
    bool _New;
    public int id { get; private set; }
    public string title { get; set; }= string.Empty;
    public string description { get; set; }= string.Empty;
    public MediaType mediaType { get; set; }
    public int releaseYear { get; set; }= 0;
    public string[] genres { get; set; }= null;
    public int ageRestriction { get; set; }= 0;
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
            Media? media = (Media)_Repository.Get(id);
            if (media == null) return null;
            media._New = false;
            return media;
        }
        return null;
    }
    
    public static bool Exists(string id)
    {
        return _Repository.Exists(id);
    }

    public override void Save()
    {
        _EnsureAdminOrOwner(ownerName);
        base.Save();
        _EndEdit();
    }

    public override void Delete()
    {
        _EnsureAdminOrOwner(ownerName);
        base.Delete();
    }

    public override void Refresh()
    {
        
    }
}