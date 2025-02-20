public class PlayerManager {
  private static readonly Dictionary<string, Player> players = [];

  public static void AddPlayer(Player player) {
    if (players.ContainsKey(player.id)) {
      return;
    }
    players.Add(player.id, player);
  }

  public static void RemovePlayer(Player player) {
    if (!players.ContainsKey(player.id)) {
      return;
    }
    players.Remove(player.id);
  }

  public static Player? GetPlayer(string id) {
    if (!players.ContainsKey(id)) {
      return null;
    }
    return players[id];
  }

  public static bool IsOnline(string id) {
    return players.ContainsKey(id);
  }
}
