using Microsoft.Data.Sqlite;
using StudyApp.Models;

namespace StudyApp.Services;

public class DatabaseService
{
    public static DatabaseService Instance { get; } = new();

    private readonly string _connectionString;

    private DatabaseService()
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StudyApp", "study.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        _connectionString = $"Data Source={dbPath}";
    }

    private SqliteConnection Open()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var pragma = conn.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON;";
        pragma.ExecuteNonQuery();
        return conn;
    }

    public void Initialize()
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Modules (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL DEFAULT '',
                Color TEXT NOT NULL DEFAULT '#007AFF'
            );
            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ModuleId INTEGER NOT NULL REFERENCES Modules(Id) ON DELETE CASCADE,
                Front TEXT NOT NULL,
                Back TEXT NOT NULL,
                Repetitions INTEGER NOT NULL DEFAULT 0,
                Interval INTEGER NOT NULL DEFAULT 1,
                EasinessFactor REAL NOT NULL DEFAULT 2.5,
                NextReviewDate TEXT NOT NULL DEFAULT (date('now'))
            );
            """;
        cmd.ExecuteNonQuery();
    }

    // Users
    public User? GetUserByUsername(string username)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, Email, PasswordHash FROM Users WHERE Username = $u";
        cmd.Parameters.AddWithValue("$u", username);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new User { Id = r.GetInt32(0), Username = r.GetString(1), Email = r.GetString(2), PasswordHash = r.GetString(3) };
    }

    public User? GetUserByEmail(string email)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Username, Email, PasswordHash FROM Users WHERE Email = $e";
        cmd.Parameters.AddWithValue("$e", email);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new User { Id = r.GetInt32(0), Username = r.GetString(1), Email = r.GetString(2), PasswordHash = r.GetString(3) };
    }

    public User CreateUser(string username, string email, string passwordHash)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Users (Username, Email, PasswordHash) VALUES ($u, $e, $p); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$u", username);
        cmd.Parameters.AddWithValue("$e", email);
        cmd.Parameters.AddWithValue("$p", passwordHash);
        var id = (long)cmd.ExecuteScalar()!;
        return new User { Id = (int)id, Username = username, Email = email, PasswordHash = passwordHash };
    }

    // Modules
    public List<Module> GetModules(int userId)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT m.Id, m.UserId, m.Name, m.Description, m.Color,
                   COUNT(CASE WHEN n.NextReviewDate <= date('now') THEN 1 END) AS DueCount,
                   COUNT(n.Id) AS TotalCount
            FROM Modules m
            LEFT JOIN Notes n ON n.ModuleId = m.Id
            WHERE m.UserId = $uid
            GROUP BY m.Id
            ORDER BY m.Name
            """;
        cmd.Parameters.AddWithValue("$uid", userId);
        var list = new List<Module>();
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new Module
            {
                Id = r.GetInt32(0), UserId = r.GetInt32(1), Name = r.GetString(2),
                Description = r.GetString(3), Color = r.GetString(4),
                DueCount = r.GetInt32(5), TotalCount = r.GetInt32(6)
            });
        return list;
    }

    public Module CreateModule(int userId, string name, string description, string color)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Modules (UserId, Name, Description, Color) VALUES ($uid, $n, $d, $c); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$uid", userId);
        cmd.Parameters.AddWithValue("$n", name);
        cmd.Parameters.AddWithValue("$d", description);
        cmd.Parameters.AddWithValue("$c", color);
        var id = (long)cmd.ExecuteScalar()!;
        return new Module { Id = (int)id, UserId = userId, Name = name, Description = description, Color = color };
    }

    public void UpdateModule(int id, string name, string description, string color)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Modules SET Name=$n, Description=$d, Color=$c WHERE Id=$id";
        cmd.Parameters.AddWithValue("$n", name);
        cmd.Parameters.AddWithValue("$d", description);
        cmd.Parameters.AddWithValue("$c", color);
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public void DeleteModule(int id)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Modules WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    // Notes
    public List<Note> GetNotes(int moduleId)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, ModuleId, Front, Back, Repetitions, Interval, EasinessFactor, NextReviewDate FROM Notes WHERE ModuleId=$mid ORDER BY Id";
        cmd.Parameters.AddWithValue("$mid", moduleId);
        var list = new List<Note>();
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new Note
            {
                Id = r.GetInt32(0), ModuleId = r.GetInt32(1), Front = r.GetString(2),
                Back = r.GetString(3), Repetitions = r.GetInt32(4), Interval = r.GetInt32(5),
                EasinessFactor = r.GetDouble(6), NextReviewDate = DateTime.Parse(r.GetString(7))
            });
        return list;
    }

    public List<Note> GetDueNotes(int moduleId)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, ModuleId, Front, Back, Repetitions, Interval, EasinessFactor, NextReviewDate FROM Notes WHERE ModuleId=$mid AND NextReviewDate <= date('now') ORDER BY NextReviewDate";
        cmd.Parameters.AddWithValue("$mid", moduleId);
        var list = new List<Note>();
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new Note
            {
                Id = r.GetInt32(0), ModuleId = r.GetInt32(1), Front = r.GetString(2),
                Back = r.GetString(3), Repetitions = r.GetInt32(4), Interval = r.GetInt32(5),
                EasinessFactor = r.GetDouble(6), NextReviewDate = DateTime.Parse(r.GetString(7))
            });
        return list;
    }

    public Note CreateNote(int moduleId, string front, string back)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Notes (ModuleId, Front, Back) VALUES ($mid, $f, $b); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$mid", moduleId);
        cmd.Parameters.AddWithValue("$f", front);
        cmd.Parameters.AddWithValue("$b", back);
        var id = (long)cmd.ExecuteScalar()!;
        return new Note { Id = (int)id, ModuleId = moduleId, Front = front, Back = back };
    }

    public void UpdateNote(int id, string front, string back)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Notes SET Front=$f, Back=$b WHERE Id=$id";
        cmd.Parameters.AddWithValue("$f", front);
        cmd.Parameters.AddWithValue("$b", back);
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public void UpdateNoteReview(Note note)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Notes SET Repetitions=$rep, Interval=$int, EasinessFactor=$ef, NextReviewDate=$nrd WHERE Id=$id";
        cmd.Parameters.AddWithValue("$rep", note.Repetitions);
        cmd.Parameters.AddWithValue("$int", note.Interval);
        cmd.Parameters.AddWithValue("$ef", note.EasinessFactor);
        cmd.Parameters.AddWithValue("$nrd", note.NextReviewDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$id", note.Id);
        cmd.ExecuteNonQuery();
    }

    public void DeleteNote(int id)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Notes WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }
}
