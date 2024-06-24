using Dapper;
using Microsoft.Data.SqlClient;

namespace Repositories.Utils;


public static class DapperUtilsRepository<T>
{
    private static string _connectionString = "Data Source=127.0.0.1; Initial Catalog=DBAccountUpBank; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";

    public static bool Insert(string query, object obj)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection.Execute(query, obj) > 0;
    }

    public static int InsertWithScalar(string query, object obj)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection.ExecuteScalar<int>(query, obj);
    }

    public static List<T> GetAll(string query)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection.Query<T>(query).ToList();
    }

    public static List<T> GetAll(string query, object obj)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection.Query<T>(query, obj).ToList();
    }




    public static T Get(string query, object obj)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection.QueryFirstOrDefault<T>(query, obj);
    }

}