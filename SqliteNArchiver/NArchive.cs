using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Xml.Linq;

namespace SqliteNArchiver
{
    public class NArchive : IDisposable
    {
        private readonly Lazy<SQLiteConnection> _getDbConnection;
        private SQLiteConnection _dbConnection => _getDbConnection.Value;

        public string DbName { get; }

        public NArchive(string dbName)
        {
            DbName = $"{dbName}.narchive";
            _getDbConnection = new Lazy<SQLiteConnection>(() =>
            {
                var dbConnection = new SQLiteConnection($"Data Source={DbName}");
                dbConnection.Open();
                return dbConnection;
            });
        }

        public void CreateArchive()
        {
            ExecuteNonQuery("CREATE TABLE files (file_name TEXT NOT NULL, path TEXT NOT NULL, hash BLOB, piece_length INTEGER);");
            ExecuteNonQuery("CREATE TABLE file_pieces (file_rowid INTEGER REFERENCES file(rowid), data BLOB, hash BLOB);");
        }

        public void AddFile(FileInfo fileInfo)
        {
            var fileName = fileInfo.Name;
            var path = fileInfo.DirectoryName == null ? "" : Path.GetRelativePath(Environment.CurrentDirectory, fileInfo.DirectoryName);

            using var transaction = _dbConnection.BeginTransaction();

            var command = new SQLiteCommand("INSERT INTO files (file_name, path) VALUES (:file_name, :path); SELECT last_insert_rowid();", _dbConnection, transaction);
            command.Parameters.Add(new SQLiteParameter("file_name", fileName));
            command.Parameters.Add(new SQLiteParameter("path", path));

            var rowid = command.ExecuteScalar();

            var file = fileInfo.OpenRead();

            transaction.Commit();
        }

        private void ExecuteNonQuery(string query)
        {
            var command = _dbConnection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            if (_getDbConnection.IsValueCreated)
            {
                _getDbConnection.Value.Close();
            }

            GC.SuppressFinalize(this);
        }
    }
}