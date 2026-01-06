using MediaReview.System;
using System.Security.Cryptography;
using System.Text;
using MediaReview.Repository;

namespace MediaReview.Model;

public class User : Atom, IAtom
{
    private static UserRepository _Repository = new();
    
    protected override IRepository _GetRepository()
    {
        return _Repository;
    }
    public User(Session? session)
    {
        _EditingSession = session;
    }
    public User(): this(null)
    {}
    
    public int _userId = 0;
    public string _UserName { get; set; }= null;
    
    public string? _PasswordHash { get; set; }

    public bool checkPassword(string username, string password)
    {
        if(_HashPassword(username, password) == null) return false;
        return _Repository.checkPassword(username, _HashPassword(username, password));
    }
    
    public static string? _HashPassword(string userName, string password)
    {
        if(string.IsNullOrWhiteSpace(password)) { return null; }

        StringBuilder rval = new();
        foreach(byte i in SHA256.HashData(Encoding.UTF8.GetBytes(userName + password)))
        {
            rval.Append(i.ToString("x2"));
        }
        return rval.ToString();
    }
    public void SetPassword(string password)
    {
        _PasswordHash = _HashPassword(_UserName, password);
    }

    public static bool Exists(string username)
    {
        return _Repository.Exists(username);
    }
    
    public string FullName
    {
        get; set;
    } = string.Empty;
    public string EMail
    {
        get; set;
    } = string.Empty;

    public static User Get(string username)
    {
        var user = _Repository.Get(username);
        if(user == null) return null;
        return ((User)user);
    }

    public static List<string> GetFavorites(int id)
    {
        return _Repository.GetFavorites(id);
    }

    public static List<Rating> GetRatings(int id)
    {
        return _Repository.GetRatings(id);
    }

    public bool isAdmin { get; set; } = false;
    public override void Save()
    {
        if(isAdmin) { _EnsureAdmin(); }
        base.Save();
    }

    public override void Delete()
    {
        _EnsureAdminOrOwner(_UserName);
        base.Delete();
    }

    public override void Refresh()
    {
        _EndEdit();
    }
}