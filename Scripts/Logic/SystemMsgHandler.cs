using System.Diagnostics;

public partial class MsgHandler {
  public static void MsgPing(ClientState state, MsgBase msg) {
    state.lastPingTime = NetManager.GetTimeStamp();
    NetManager.Send(state, new MsgPong());
  }
}
