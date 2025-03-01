using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public partial class MsgHandler {
  public static void MsgRegister(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgRegister msg) {
      return;
    }
    if (DataBaseManager.Register(msg.Id, msg.Password)) {
      DataBaseManager.CreatePlayer(msg.Id);
      msg.Result = 0;
    } else {
      msg.Result = 1;
    }
    NetManager.Send(state, msg);
  }

  public static void MsgLogin(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgLogin msg) {
      return;
    }
    void LoginFail() {
      msg.Result = 1;
      NetManager.Send(state, msg);
    }
    if (!DataBaseManager.CheckPassword(msg.Id, msg.Password)) {
      LoginFail();
      return;
    }
    if (state.player != null) {
      LoginFail();
      return;
    }
    if (PlayerManager.IsOnline(msg.Id)) {
      Player? otherPlayer = PlayerManager.GetPlayer(msg.Id);
      otherPlayer?.Send(new MsgKick() {
        Reason = 0
      });
      NetManager.Close(otherPlayer?.state);
    }
    PlayerData? playerData = DataBaseManager.GetPlayerData(msg.Id);
    if (playerData == null) {
      LoginFail();
      return;
    }
    Player player = new(state) {
      id = msg.Id,
      data = playerData
    };
    state.player = player;
    PlayerManager.AddPlayer(player);
    msg.Result = 0;
    player.Send(msg);
  }
}
