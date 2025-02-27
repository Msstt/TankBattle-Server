using System.Diagnostics;

public partial class MsgHandler {
  public static void MsgSyncTank(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgSyncTank msg || state.player == null) {
      return;
    }
    Player player = state.player;
    Room? room = RoomManager.GetRoom(player.roomId);
    if (room == null || room.status == Room.Status.Pending) {
      return;
    }
    if (Math.Abs(msg.X - player.x) > 5 || Math.Abs(msg.Y - player.y) > 5 || Math.Abs(msg.Z - player.z) > 5) {
      Console.WriteLine(player.id + "'s MsgSyncTank has error!");
      return;
    }
    player.x = msg.X;
    player.y = msg.Y;
    player.z = msg.Z;
    player.ex = msg.EX;
    player.ey = msg.EY;
    player.ez = msg.EZ;

    msg.ID = player.id;
    room.Broadcast(msg);
  }

  public static void MsgFire(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgFire msg || state.player == null) {
      return;
    }
    Player player = state.player;
    Room? room = RoomManager.GetRoom(player.roomId);
    if (room == null || room.status == Room.Status.Pending) {
      return;
    }
    msg.ID = player.id;
    room.Broadcast(msg);
  }

  public static void MsgHit(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgHit msg || state.player == null) {
      return;
    }
    Player player = state.player;
    Room? room = RoomManager.GetRoom(player.roomId);
    if (room == null || room.status == Room.Status.Pending) {
      return;
    }
    msg.Health = 35;
    Player? hitPlayer = PlayerManager.GetPlayer(msg.ID);
    if (hitPlayer != null && hitPlayer.roomId == player.roomId) {
      hitPlayer.health -= 35;
    }
    room.Broadcast(msg);
  }
}
