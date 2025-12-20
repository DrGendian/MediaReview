using MediaReview.Model;

namespace MediaReview.System;

public sealed class Session
{
    private const string _ALPHABET = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private const int TIMEOUT_MINUTES = 30;

    private static readonly Dictionary<string, Session> _Sessions = new();

    private Session(string username, int id)
    {
       UserName = username;
       UserId = id;
       IsAdmin = (username == "admin");
       Timestamp = DateTime.UtcNow;
       
       Token = string.Empty;
       Random rnd = new();
       for(int i = 0; i < 24; i++) { Token += _ALPHABET[rnd.Next(0,62)]; }

    }
    
    public string Token { get; }
    
    public int UserId { get; private set; }
    
    public string UserName { get; }
    
    public DateTime Timestamp { get; private set; }

    public bool Valid
    {
        get { return _Sessions.ContainsKey(Token); }
    }

    public bool IsAdmin { get; }

    public static void VerifySession(string token)
    {
        _Cleanup();
        if (!_Sessions.ContainsKey(token))
        {
            throw new UnauthorizedAccessException("Invalid or expired session.");
        }
    }
    
    public static Session? Create(string userName, string password)
    {
        var user = User.Get(userName);
        if (user == null) return null;
        if (!(user.checkPassword(userName, password))) return null;

        var session = new Session(userName, user._userId);
        lock (_Sessions)
        {
            _Sessions[session.Token] = session;
        }

        return session;
    }

    public static Session? Get(string token)
    {
        Session? rval = null;
        _Cleanup();

        lock (_Sessions)
        {
            if(_Sessions.ContainsKey(token))
            {
                rval = _Sessions[token];
                rval.Timestamp = DateTime.UtcNow;
            }
        }
        
        return rval;
    }
    
    private static void _Cleanup()
    {
        List<string> toRemove = new();

        lock(_Sessions)
        {
            foreach(KeyValuePair<string, Session> pair in _Sessions)
            {
                if((DateTime.UtcNow - pair.Value.Timestamp).TotalMinutes > TIMEOUT_MINUTES) { toRemove.Add(pair.Key); }
            }
            foreach(string key in toRemove) { _Sessions.Remove(key); }
        }
    }
    
    public void Close()
    {
        bool removed = false;
        lock(_Sessions)
        {
            removed = _Sessions.Remove(Token);
        }
        if (removed)
        {
            Console.WriteLine($"Session {Token} removed");
            return;
        }
        
        Console.WriteLine($"Session {Token} could not be closed");
    }
}