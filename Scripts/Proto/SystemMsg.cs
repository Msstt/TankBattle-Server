using System.Collections;
using System.Collections.Generic;

public class MsgPing : MsgBase {
  public MsgPing() { ProtoName = "MsgPing"; }
}

public class MsgPong : MsgBase {
  public MsgPong() { ProtoName = "MsgPong"; }
}
