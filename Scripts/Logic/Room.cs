using System.Reflection.Metadata;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Signers;

public class Room {
  public int id = 0;
  public readonly HashSet<string> players = [];

  private string owner = "";
  private readonly int[] campCount = [0, 0];

  public enum Status {
    Pending,
    Started
  }

  public Status status;

  public bool AddPlayer(string playerId) {
    Player? player = PlayerManager.GetPlayer(playerId);
    if (player == null || player.roomId != -1) {
      return false;
    }
    if (players.Contains(player.id)) {
      return false;
    }
    if (players.Count == 0) {
      owner = playerId;
    }
    players.Add(player.id);
    player.roomId = id;
    player.camp = GetCamp();
    Broadcast();
    return true;
  }

  public bool RemovePlayer(string playerId) {
    Player? player = PlayerManager.GetPlayer(playerId);
    if (player == null || player.roomId != id) {
      return false;
    }
    if (!players.Contains(player.id)) {
      return false;
    }
    player.roomId = -1;
    players.Remove(player.id);
    if (players.Count == 0) {
      RoomManager.RemoveRoom(id);
      return true;
    }
    if (IsOwner(playerId)) {
      owner = players.First();
    }
    Broadcast();
    return true;
  }

  private int GetCamp() {
    int camp = campCount[0] > campCount[1] ? 1 : 0;
    campCount[camp]++;
    return camp;
  }

  private void Broadcast() {
    MsgBase msg = GetMsg();
    foreach (string playerId in players) {
      Player? player = PlayerManager.GetPlayer(playerId);
      if (player == null) {
        continue;
      }
      player.Send(msg);
    }
  }

  public MsgBase GetMsg() {
    MsgGetRoomInfo msg = new() {
      RoomID = id,
      Players = new PlayerInfo[players.Count]
    };
    int i = -1;
    foreach (string playerId in players) {
      i++;
      Player? player = PlayerManager.GetPlayer(playerId);
      if (player == null) {
        continue;
      }
      msg.Players[i] = new() {
        ID = player.id,
        Camp = player.camp,
        Win = player.data.win,
        Lose = player.data.lose,
        isOwner = IsOwner(player.id) == true ? 1 : 0
      };
    }
    return msg;
  }

  public bool IsOwner(string PlayerId) {
    return PlayerId == owner;
  }
}
