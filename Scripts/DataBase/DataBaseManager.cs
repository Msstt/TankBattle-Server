using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public partial class DataBaseManager {
  public static MySqlConnection mysql;

  public static bool Connect(string dataBase, string ip, int port, string user, string password) {
    mysql = new() {
      ConnectionString = $"Database={dataBase};Data Source={ip}; port={port};User Id={user}; Password={password}"
    };
    try {
      mysql.Open();
      Console.WriteLine("DataBaseManager connect success.");
      return true;
    } catch (Exception ex) {
      Console.WriteLine("DataBaseManager connect failed: " + ex.Message + ".");
      return false;
    }
  }

  public static bool IsAccountExist(string id) {
    if (!IsSafeString(id)) {
      return false;
    }
    string sql = string.Format("select * from account where id = '{0}'; ", id);
    return QuerySQL(sql);
  }

  public static bool Register(string id, string password) {
    if (!IsSafeString(id) || !IsSafeString(password)) {
      return false;
    }
    if (IsAccountExist(id)) {
      return false;
    }
    string sql = string.Format("insert into account set id = '{0}', password = '{1}'; ", id, password);
    return UpdateSQL(sql);
  }

  public static bool CreatePlayer(string id) {
    if (!IsSafeString(id)) {
      return false;
    }
    string dataString = JsonConvert.SerializeObject(new PlayerData());
    string sql = string.Format("insert into player set id = '{0}', data = '{1}'; ", id, dataString);
    return UpdateSQL(sql);
  }

  public static bool CheckPassword(string id, string password) {
    if (!IsSafeString(id) || !IsSafeString(password)) {
      return false;
    }
    string sql = string.Format("select * from account where id='{0}' and password='{1}';", id, password);
    return QuerySQL(sql);
  }

  public static PlayerData? GetPlayerData(string id) {
    if (!IsSafeString(id)) {
      return null;
    }
    string sql = string.Format("select * from player where id = '{0}'; ", id);
    try {
      MySqlCommand cmd = new(sql, mysql);
      MySqlDataReader dataReader = cmd.ExecuteReader();
      if (!dataReader.HasRows) {
        dataReader.Close();
        return null;
      }
      dataReader.Read();
      string data = dataReader.GetString("data");
      PlayerData? playerData = JsonConvert.DeserializeObject<PlayerData>(data);
      dataReader.Close();
      return playerData;
    } catch (Exception e) {
      Console.WriteLine("DataBaseManager get PlayerData fail: " + e.Message + "!");
      return null;
    }
  }

  public static bool UpdatePlayerData(string id, PlayerData playerData) {
    string data = JsonConvert.SerializeObject(playerData);
    string sql = string.Format("update player set data = '{0}' where id = '{1}'; ", data, id);
    return UpdateSQL(sql);
  }

  private static bool QuerySQL(string sql) {
    try {
      MySqlCommand cmd = new(sql, mysql);
      MySqlDataReader dataReader = cmd.ExecuteReader();
      bool hasRows = dataReader.HasRows;
      dataReader.Close();
      return hasRows;
    } catch (Exception e) {
      Console.WriteLine("DataBaseManager query failed: " + e.Message + "!");
      return false;
    }
  }

  private static bool UpdateSQL(string sql) {
    try {
      MySqlCommand cmd = new(sql, mysql);
      cmd.ExecuteNonQuery();
      return true;
    } catch (Exception e) {
      Console.WriteLine("DataBaseManager update failed: " + e.Message + "!");
      return false;
    }
  }

  private static bool IsSafeString(string str) {
    return !MyRegex().IsMatch(str);
  }

  [GeneratedRegex(@"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']")]
  private static partial Regex MyRegex();
}
