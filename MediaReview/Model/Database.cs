using Npgsql;

namespace MediaReview.Model;

public class Database
{
    #region Data
    private static Database? _instance;
    private static readonly object _lock = new();

    Dictionary<string, User> _users;
    Dictionary<int, Review> _reviews;
    Dictionary<int, Media> _medias;

    #endregion

    #region Constructor
    private Database()
    {
        var connString = "Host=localhost;Port=5432;Username=admin;Password=admin;Database=admin";
        
        using var conn = new NpgsqlConnection(connString);
        conn.Open();
        Console.WriteLine("Connected to Database");
        
        _users = new Dictionary<string, User>();
        _reviews = new Dictionary<int, Review>();
        _medias = new Dictionary<int, Media>();
    }

    public static Database Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new Database();
            }
        }
    }
    
    public Dictionary<string, User> GetAllUsers()
    {
        return _users;
    }
    #endregion
    #region User Methods
    public User GetUser(string userName)
    {
        lock (_users)
        {
            if (_users.TryGetValue(userName, out var user))
            {
                return user;
            }
            return null;
        }
    }
    
    public void SaveUser(User user)
    {
        lock (_users)
        {
            if (!_users.ContainsKey(user.UserName))
            {
                _users.Add(user.UserName, user);
            }
            else
            {
                _users[user.UserName] = user;
            }
        }
    }

    public bool UserExists(string userName)
    {
        lock (_users);
        return _users.ContainsKey(userName);
    }

    public bool DeleteUser(string userName)
    {
        try
        {
            lock (_users)
            {
                _users.Remove(userName);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    #endregion
    #region Media Methods
    public Media? GetMedia(int mediaId)
    {
        lock (_medias)
        {
            if (_medias.ContainsKey(mediaId))
            {
                if (_medias.TryGetValue(mediaId, out var media))
                {
                    return media;
                }
            }
            return null;
        }
    }

    public bool MediaExists(int mediaId)
    {
        lock (_medias)
        {
            if (_medias.ContainsKey(mediaId))
            {
                return true;
            }
            return false;
        }
    }

    public void SaveMedia(Media media)
    {
        lock (_medias)
        {
            if (!_medias.ContainsKey(media.id))
            {
                _medias.Add(media.id, media);
            }
            else if(_medias.ContainsKey(media.id) && _medias[media.id].title == media.title)
            {
                _medias[media.id] = media;
            }
            else
            {
                _medias.Add(_medias.Count, media);
            }
        }
    }
    
    public bool DeleteMedia(int id)
    {
        try
        {
            lock (_users)
            {
                _medias.Remove(id);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    #endregion
}