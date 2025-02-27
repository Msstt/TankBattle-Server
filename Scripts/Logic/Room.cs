using System.Dynamic;
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

  private static readonly float[,,] birthTransform = new float[2, 2, 6] {{
      {-3.273307f, -2.384186e-07f, -6.42648f, 0, -50.802f, 0},
      {-9.277866f, 0, -9.314688f, 0, -33.617f, 0},
    },{
      {-28.10796f, -1.192093e-07f, 11.09761f, 0, 134.487f, 0},
      {-19.88399f, -2.980232e-07f, 17.29752f, 0, 152.09f, 0},
    }
  };

  public bool AddPlayer(string playerId) {
    if (players.Count >= 4) {
      return false;
    }
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
    Broadcast(GetMsg());
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
    campCount[player.camp]--;
    Broadcast(GetMsg());

    if (status == Status.Started) {
      player.data.lose++;
      Broadcast(new MsgLeaveBattle() {
        ID = player.id
      });
    }
    return true;
  }

  private int GetCamp() {
    int camp = campCount[0] > campCount[1] ? 1 : 0;
    campCount[camp]++;
    return camp;
  }

  public void Broadcast(MsgBase msg) {
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

  public bool StartBattle() {
    if (!CanStartBattle()) {
      return false;
    }
    status = Status.Started;
    Reset();
    MsgEnterBattle msg = new() {
      Tanks = new TankInfo[players.Count]
    };
    int i = -1;
    foreach (string playerID in players) {
      i++;
      Player? player = PlayerManager.GetPlayer(playerID);
      if (player == null) {
        continue;
      }
      msg.Tanks[i] = GetMsg(player);
    }
    Broadcast(msg);
    return true;
  }

  private bool CanStartBattle() {
    if (players.Count < 2) {
      return false;
    }
    if (status == Status.Started) {
      return false;
    }
    return true;
  }

  private void Reset() {
    int Camp1 = 0, Camp2 = 0;
    foreach (string playerID in players) {
      Player? player = PlayerManager.GetPlayer(playerID);
      if (player == null) {
        continue;
      }
      SetPlayerTransform(player, player.camp == 1 ? Camp1++ : Camp2++);
      player.health = 100;
    }
  }

  private static void SetPlayerTransform(Player player, int index) {
    player.x = birthTransform[player.camp, index, 0];
    player.y = birthTransform[player.camp, index, 1];
    player.z = birthTransform[player.camp, index, 2];
    player.ex = birthTransform[player.camp, index, 3];
    player.ey = birthTransform[player.camp, index, 4];
    player.ez = birthTransform[player.camp, index, 5];
  }

  private static TankInfo GetMsg(Player player) {
    return new() {
      ID = player.id,
      Camp = player.camp,
      X = player.x,
      Y = player.y,
      Z = player.z,
      EX = player.ex,
      EY = player.ey,
      EZ = player.ez,
    };
  }

  private int Judge() {
    int[] count = [0, 0];
    foreach (string playerId in players) {
      Player? player = PlayerManager.GetPlayer(playerId);
      if (player == null) {
        continue;
      }
      if (player.health > 0) {
        count[player.camp]++;
      }
    }
    if (count[0] > 0 && count[1] > 0) {
      return -1;
    }
    return count[0] > 0 ? 0 : 1;
  }

  public void Update() {
    if (status == Status.Pending) {
      return;
    }
    int result = Judge();
    if (result == -1) {
      return;
    }
    status = Status.Pending;
    foreach (string playerId in players) {
      Player? player = PlayerManager.GetPlayer(playerId);
      if (player == null) {
        continue;
      }
      if (player.camp == result) {
        player.data.win++;
      } else {
        player.data.lose++;
      }
    }
    Broadcast(new MsgBattleResult() {
      Result = result
    });
  }
}
