using MediaReview.Repository;

namespace MediaReview.Model;

public class Rating: Atom, IAtom
{
    private static RatingRepository _Repository = new();
    
    public int stars {get;}
    public string comment {get; set;}
    private bool comment_confirmed;
    public int user_id { get; }
    public int media_id { get; }

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
        base.Save();
    }
}