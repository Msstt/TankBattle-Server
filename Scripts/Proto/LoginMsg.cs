public class MsgRegister : MsgBase {
  public MsgRegister() { ProtoName = "MsgRegister"; }

  public string Id { get; set; } = "";
  public string Password { get; set; } = "";
  public int Result { get; set; } = 0;
}

public class MsgLogin : MsgBase {
  public MsgLogin() { ProtoName = "MsgLogin"; }

  public string Id { get; set; } = "";
  public string Password { get; set; } = "";
  public int Result { get; set; } = 0;
}

public class MsgKick : MsgBase {
  public MsgKick() { ProtoName = "MsgKick"; }

  public int Reason { get; set; } = 0;
}
