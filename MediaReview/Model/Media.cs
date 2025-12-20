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
    public int id { get;  set; }
    public string title { get; set; }= string.Empty;
    public string description { get; set; }= string.Empty;
    public MediaType mediaType { get; set; }
    public int releaseYear { get; set; }= 0;
    public List<string> genres { get; set; }= new();
    public int ageRestriction { get; set; }= 0;
    public string ownerName { get; set; } = string.Empty;
    public double avg_score { get; set; }= 0;

    public Media(Session? session = null)
    {
        _EditingSession = session;
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
            return media;
        }
        return null;
    }

    public static void Favorite(int id, Session session)
    {
        if(!_Repository.Exists(id)) throw new KeyNotFoundException("Media not found");
        if (session.Valid)
        {
            _Repository.Favorite(id, session.UserId);
        }
    }
    
    public static void DeleteFavorite(int id, Session session)
    {
        if(!_Repository.Exists(id)) throw new KeyNotFoundException("Media not found");
        if (session.Valid)
        {
            _Repository.DeleteFavorite(id, session.UserId);
        }
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