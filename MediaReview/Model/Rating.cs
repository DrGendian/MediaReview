using MediaReview.Repository;
using MediaReview.System;

namespace MediaReview.Model;

public class Rating: Atom, IAtom
{
    private static RatingRepository _Repository = new();
    
    public int stars {get; set; }
    public string comment {get; set;}
    public bool comment_confirmed { get; set; }
    public int user_id { get; set; }
    public int media_id { get; set; }

    public Rating()
    {
        stars = 0;
        comment = "";
        comment_confirmed = false;
    }

    public static Rating Get(int media_id, Session session)
    {
        Rating r = (Rating)_Repository.Get(media_id, session);
        if (r == null)
        {
            throw new Exception("Rating not found");
        }
        return r;
    }

    public static void Like(int ratingId, Session session)
    {
        _Repository.Like(ratingId, session);
    }
    
    public virtual void BeginEdit(Session session)
    {
        _VerifySession(session); 
    }

    public Rating(int stars, string comment, int user_id, int media_id)
    {
        this.stars = stars;
        this.comment = comment;
        this.user_id = user_id;
        this.media_id = media_id;
    }
    protected override IRepository _GetRepository()
    {
        return _Repository;
    }

    public void Save()
    {
        Console.WriteLine(user_id);
        _EnsureAdminOrOwner(user_id);
        base.Save();
    }
}