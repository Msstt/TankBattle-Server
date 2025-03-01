public class MsgGetText : MsgBase {
  public MsgGetText() { ProtoName = "MsgGetText"; }

  public string Text { get; set; } = "";
}

public class MsgSaveText : MsgBase {
  public MsgSaveText() { ProtoName = "MsgSaveText"; }

  public string Text { get; set; } = "";
  public int Result { get; set; } = 0;
}
