using System.Data;
using Npgsql;

namespace MediaReview.Repository;

public abstract class RepositoryBase
{
    private static IDbConnection? _DbConnection;

    protected static IDbConnection _Cn
    {
        get
        {
            if (_DbConnection == null)
            {
                var connString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=postgres";
                _DbConnection = new NpgsqlConnection(connString);
                _DbConnection.Open();
            }

            return _DbConnection;
        }
    }
}