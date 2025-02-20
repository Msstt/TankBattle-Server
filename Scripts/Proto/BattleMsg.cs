using System.Collections;
using System.Collections.Generic;

public class MsgMove : MsgBase {
  public MsgMove() { ProtoName = "MsgMove"; }

  public int X { get; set; } = 0;
  public int Y { get; set; } = 0;
  public int Z { get; set; } = 0;
}
