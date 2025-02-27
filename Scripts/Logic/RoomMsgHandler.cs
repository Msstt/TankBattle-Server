using Org.BouncyCastle.Pqc.Crypto.Ntru;

public partial class MsgHandler {
  public static void MsgGetRoomList(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgGetRoomList msg || state.player == null) {
      return;
    }
    state.player.Send(RoomManager.GetMsg());
  }

  public static void MsgGetScore(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgGetScore msg || state.player == null) {
      return;
    }
    msg.Win = state.player.data.win;
    msg.Lose = state.player.data.lose;
    state.player.Send(msg);
  }

  public static void MsgEnterRoom(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgEnterRoom msg || state.player == null) {
      return;
    }
    void Finish(int Result) {
      msg.Result = Result;
      NetManager.Send(state, msg);
    }
    Room? room = RoomManager.GetRoom(msg.ID);
    if (room == null) {
      Finish(1);
      return;
    }
    if (!room.AddPlayer(state.player.id)) {
      Finish(1);
      return;
    }
    Finish(0);
  }

  public static void MsgCreateRoom(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgCreateRoom msg || state.player == null) {
      return;
    }
    void Finish(int Result) {
      msg.Result = Result;
      NetManager.Send(state, msg);
    }
    if (state.player.roomId >= 0) {
      Finish(1);
      return;
    }
    Room room = RoomManager.AddRoom();
    if (!room.AddPlayer(state.player.id)) {
      Finish(1);
      return;
    }
    Finish(0);
  }

  public static void MsgLeaveRoom(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgLeaveRoom msg || state.player == null) {
      return;
    }
    void Finish(int Result) {
      msg.Result = Result;
      NetManager.Send(state, msg);
    }
    Room? room = RoomManager.GetRoom(state.player.roomId);
    if (room == null) {
      Finish(1);
      return;
    }
    if (!room.RemovePlayer(state.player.id)) {
      Finish(1);
      return;
    }
    Finish(0);
  }

  public static void MsgGetRoomInfo(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgGetRoomInfo msg || state.player == null) {
      return;
    }
    Room? room = RoomManager.GetRoom(state.player.roomId);
    if (room == null) {
      state.player.Send(msg);
      return;
    }
    state.player.Send(room.GetMsg());
  }

  public static void MsgStartGame(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgStartGame msg || state.player == null) {
      return;
    }
    void Finish(int Result) {
      msg.Result = Result;
      NetManager.Send(state, msg);
    }
    Room? room = RoomManager.GetRoom(state.player.roomId);
    if (room == null) {
      Finish(1);
      return;
    }
    if (!room.IsOwner(state.player.id)) {
      Finish(1);
      return;
    }
    if (!room.StartBattle()) {
      Finish(1);
      return;
    }
    Finish(0);
  }
}
