public class EventHandler {
  public static void OnDisconnect(ClientState state) {
    if (state.player != null) {
      DataBaseManager.UpdatePlayerData(state.player.id, state.player.data);
      PlayerManager.RemovePlayer(state.player);
    }
  }

  public static void OnTimer() {
    CheckPing();
  }

  private static void CheckPing() {
    long time = NetManager.GetTimeStamp();
    foreach (ClientState state in NetManager.clients.Values) {
      if (time - state.lastPingTime > NetManager.pingInterval * 4) {
        string endPoint = state.socket.RemoteEndPoint.ToString();
        Console.WriteLine("Client ping is missing: " + endPoint + "!");
        NetManager.Close(state);
        return;
      }
    }
  }
}
