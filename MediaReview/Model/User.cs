using MediaReview.System;
using System.Security.Cryptography;
using System.Text;

namespace MediaReview.Model;

public class User : Atom, IAtom
{
    private static readonly Dictionary<string, User> _Users = new();
    
    public string? _UserName { get; private set; }= null;

    private bool _New;

    public string? _PasswordHash { get; private set; } = null;



    public User(Session? session = null)
    {
        _EditingSession = session;
        _New = true;
    }


    public static User Get(string userName, Session? session = null)
    {
        
        lock (_Users)
        {
            if (_Users.TryGetValue(userName, out var user))
            {
                return user;
            }
            return null;
        }
    }
    
    public static String GetAll(Session? session = null)
    {
        
        lock (_Users)
        {
            return string.Join(Environment.NewLine,
                _Users.Values.Select(u =>
                    $"UserName: {u.UserName}, FullName: {u.FullName}, Email: {u.EMail}"
                ));
        }
    }

    public static User Create(string userName, string password, Session? session = null)
    {
        lock (_Users)
        {
            if (_Users.ContainsKey(userName))
                throw new Exception("User already exists");

            User user = new User(session);
            user.UserName = userName;
            user._PasswordHash = _HashPassword(userName, password);
            _Users.Add(userName, user);
            return user;
        }
        
    }

    public static bool Exists(string userName)
    {
        lock (_Users)
        {
            return _Users.ContainsKey(userName);
        }
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
        _PasswordHash = _HashPassword(UserName, password);
    }

    public override void Save()
    {
        if(!_New) { _EnsureAdminOrOwner(UserName); }
        _EndEdit();
    }

    public override void Delete()
    {
        _EnsureAdminOrOwner(UserName);

        _EndEdit();
    }

    public override void Refresh()
    {
        _EndEdit();
    }
}