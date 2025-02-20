using System.Diagnostics;

public partial class MsgHandler {
  public static void MsgMove(ClientState state, MsgBase msgBase) {
    if (msgBase is not MsgMove msg) {
      return;
    }
    msg.X++;
    NetManager.Send(state, msg);
  }
}
