public class Player(ClientState state) {
  public readonly ClientState state = state;
  public PlayerData data;

  public string id = "";
  public int roomId = -1;
  public int camp = 0;

  public float x = 0;
  public float y = 0;
  public float z = 0;
  public float ex = 0;
  public float ey = 0;
  public float ez = 0;
  public float health = 0;

  public void Send(MsgBase msg) {
    NetManager.Send(state, msg);
  }
}
