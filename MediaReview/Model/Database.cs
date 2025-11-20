namespace MediaReview.Model;

public class Database
{

    private static Database? _instance;
    private static readonly object _lock = new();

    Dictionary<string, User> _users;
    Dictionary<int, Review> _reviews;
    Dictionary<int, Media> _medias;

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
    
    public void AddUser(User user)
    {
        lock (_users)
        {
            _users.Add(user.UserName, user);
        }
    }

    public bool UserExists(string userName)
    {
        return _users.ContainsKey(userName);
    }
}