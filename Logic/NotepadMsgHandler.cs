public partial class MsgHandler {
  public static void MsgGetText(ClientState state, MsgBase msgBase) {
    if (state.player == null) {
      return;
    }
    state.player.Send(new MsgGetText() {
      Text = state.player.data.text
    });
  }

  public static void MsgSaveText(ClientState state, MsgBase msgBase) {
    if (state.player == null) {
      return;
    }
    if (msgBase is not MsgSaveText msg) {
      return;
    }
    state.player.data.text = msg.Text;
    msg.Result = 0;
    state.player.Send(msg);
  }
}
