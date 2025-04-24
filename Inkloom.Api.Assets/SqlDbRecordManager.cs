using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;

namespace Inkloom.Api.Assets;

public class SqlDbRecordManagerOptions
{
    public required DbProviderFactory DbProviderFactory { get; set; } // default is SQL Server
    public string ConnectionString { get; set; } = "";
    public string TableName { get; set; } = "Assets";
}

public class SqlDbRecordManager(SqlDbRecordManagerOptions options) : IAssetRecordManager
{
    private readonly DbProviderFactory _dbProviderFactory = options.DbProviderFactory;
    private readonly string _tableName = options.TableName;
    private readonly string _connectionString = options.ConnectionString;
    private DbConnection GetDbConnection()
    {
        var dbConnection = _dbProviderFactory.CreateConnection() ?? throw new InvalidOperationException("Connection couldn't be created");
        dbConnection.ConnectionString = _connectionString;
        return dbConnection;
    }

    private static void AddDbParameter<T>(DbCommand command, string name, T value)
    {
        DbParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private static string HashMD5(string filePath)
    {
        byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(filePath));
        return Convert.ToHexStringLower(hashBytes);
    }
    public void EnsureTableExists()
    {
        using var connection = GetDbConnection();
        connection.Open();

        // Check if the table exists
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
        AddDbParameter(command, "@TableName", _tableName);

        var tableExists = Convert.ToInt32(command.ExecuteScalar()) > 0;

        if (tableExists)
        {
            return;
        }


        // Create the table
        using var createCommand = connection.CreateCommand();
        createCommand.CommandText = $@"
                            CREATE TABLE ""{_tableName}"" (
                                ""Id"" CHAR(32) PRIMARY KEY,
                                ""FilePath"" TEXT NOT NULL,
                                ""Name"" TEXT NOT NULL,
                                ""Author"" TEXT NOT NULL,
                                ""Type"" VARCHAR(16) NOT NULL,
                                ""Mimetype"" TEXT NOT NULL,
                                ""Size"" BIGINT NULL,
                                ""Metadata"" TEXT NULL
                            )";
        createCommand.ExecuteNonQuery();
    }

    public string AddRecord(AssetRecord record)
    {
        using var connection = GetDbConnection();
        connection.Open();

        var query = $"INSERT INTO \"{_tableName}\" (\"Id\", \"FilePath\", \"Name\", \"Author\", \"Type\", \"Mimetype\", \"Size\", \"Metadata\") VALUES (@Id, @FilePath, @Name, @Author, @Type, @MimeType @Size, @Metadata)";

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddDbParameter(command, "@Id", HashMD5(record.FilePath));
        AddDbParameter(command, "@FilePath", record.FilePath);
        AddDbParameter(command, "@Name", record.Name);
        AddDbParameter(command, "@Author", record.Author);
        AddDbParameter(command, "@Type", record.Type.ToString());
        AddDbParameter(command, "@Mimetype", record.Mimetype);
        AddDbParameter(command, "@Metadata", record.Metadata);

        command.ExecuteNonQuery();

        return record.Id;
    }

    public void RemoveRecord(string filePath)
    {
        using var connection = GetDbConnection();
        connection.Open();

        var query = $"DELETE FROM \"{_tableName}\" WHERE \"Id\" = @Id";

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddDbParameter(command, "@Id", HashMD5(filePath));
        command.ExecuteNonQuery();
    }

    public AssetRecord ReadRecord(string filePath)
    {
        using var connection = GetDbConnection();
        connection.Open();

        var query = $"SELECT Data FROM \"{_tableName}\" WHERE \"Id\" = @Id";

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddDbParameter(command, "@Id", HashMD5(filePath));

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            throw new KeyNotFoundException($"Record with file path '{filePath}' not found.");
        }

        return new AssetRecord
        {
            Id = reader["Id"]?.ToString() ?? "",
            FilePath = reader["FilePath"]?.ToString() ?? "",
            Name = reader["Name"].ToString() ?? "",
            Author = reader["Author"].ToString() ?? "",
            Type = Enum.Parse<AssetType>(reader["Type"].ToString() ?? "Document"),
            Mimetype = reader["Mimetype"].ToString() ?? "",
            Size = long.TryParse(reader["Size"].ToString(), out long size) ? size : 0,
            Metadata = reader["Metadata"].ToString() ?? ""
        };
    }

    public AssetRecord UpdateRecord(string filePath, AssetRecord asset)
    {
        using var connection = GetDbConnection();
        connection.Open();

        var query = $"UPDATE \"{_tableName}\" SET \"Name\" = @Name, \"Author\" = @Author, \"Type\" = @Type, \"Mimetype\" = @Mimetype, \"Metadata\" = @Metadata WHERE \"Id\" = @Id";

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = query;

        var assetId = HashMD5(filePath);

        AddDbParameter(command, "@Id", assetId);
        AddDbParameter(command, "@Name", asset.Name);
        AddDbParameter(command, "@Author", asset.Author);
        AddDbParameter(command, "@Type", asset.Type.ToString());
        AddDbParameter(command, "@Mimetype", asset.Mimetype);
        AddDbParameter(command, "@Metadata", asset.Metadata);

        var rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            throw new KeyNotFoundException($"Record with file path '{filePath}' not found.");
        }

        asset.Id = assetId;
        return asset;
    }
}
