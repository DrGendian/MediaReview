namespace MediaReview.Model;

public class Database
{
    #region Data
    private static Database? _instance;
    private static readonly object _lock = new();

    Dictionary<string, User> _users;
    Dictionary<int, Review> _reviews;
    Dictionary<int, Media> _medias;

    #region Constructor
    private Database()
    {
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

    #region Media Methods
    public Media GetMedia(int mediaId)
    {
        lock (_medias)
        {
            if (_medias.TryGetValue(mediaId, out var media))
            {
                return media;
            }
            return null;
        }
    }
}