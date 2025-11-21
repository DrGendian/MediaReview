using MediaReview.System;
using System.Security.Cryptography;
using System.Text;

namespace MediaReview.Model;

public class User : Atom, IAtom
{
    public int _userId = 0;
    private string? _UserName { get; set; }= null;

    private bool _New;

    private string? _PasswordHash { get; set; } = null;



    public User(Session? session = null)
    {
        _EditingSession = session;
        _New = true;
    }


    public static User Get(string userName, Session? session = null)
    {
        
        return Database.Instance.GetUser(userName);
    }

    public static User GetUser(string userName, Session session)
    {
        if ((session.Valid) && (session.IsAdmin || session.UserName == userName))
        {
            var u = Database.Instance.GetUser(userName);
            u._New = false;
            return u;
        }
        return null;
    }
    
    public static String GetAll(Session session)
    {
        //if (!session.Valid && !session.IsAdmin) throw new UnauthorizedAccessException("Admin privileges required.");
        
        var users = Database.Instance.GetAllUsers();
        
        lock (users)
        {
            return string.Join(Environment.NewLine,
                users.Values.Select(u =>
                    $"UserName: {u.UserName}, FullName: {u.FullName}, Email: {u.EMail}"
                ));
        }
    }

    public static bool Exists(string userName)
    {
        return Database.Instance.UserExists(userName);
    }

    public static User Create(string userName, string password, Session? session = null)
    {
        if (Database.Instance.UserExists(userName))
        {
            throw new Exception("User already exists");
        }

        User user = new User(session);
        user.UserName = userName;
        user._PasswordHash = _HashPassword(userName, password);
        return user;
    }

    public string UserName
    {
        get { return _UserName ?? string.Empty; }
        set 
        {
            if(!_New) { throw new InvalidOperationException("User name cannot be changed."); }
            if(string.IsNullOrWhiteSpace(value)) { throw new ArgumentException("User name must not be empty."); }
            
            _UserName = value; 
        }
    }

    internal static string _HashPassword(string userName, string password)
    {
        StringBuilder rval = new();
        foreach(byte i in SHA512.HashData(Encoding.UTF8.GetBytes(userName + password)))
        {
            rval.Append(i.ToString("x2"));
        }
        return rval.ToString();
    }

    public bool checkPassword(string username, string password)
    {
        return ((_HashPassword(username, password)) == _PasswordHash);
    }

    public string FullName
    {
        get; set;
    } = string.Empty;


    public string EMail
    {
        get; set;
    } = string.Empty;


    public void SetPassword(string password)
    {
        _EnsureAdminOrOwner(UserName);
        _PasswordHash = _HashPassword(UserName, password);
    }

    public override void Save()
    {
        if(!_New) { _EnsureAdminOrOwner(UserName); }
        Database.Instance.SaveUser(this);
        _EndEdit();
    }

    public override void Delete()
    {
        _EnsureAdminOrOwner(UserName);

        Database.Instance.DeleteUser(UserName);
        
        _EndEdit();
    }

    public override void Refresh()
    {
        _EndEdit();
    }
}