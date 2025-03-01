using System;

[Serializable]
public class TankInfo {
  public string ID { get; set; } = "";
  public float X { get; set; } = 0;
  public float Y { get; set; } = 0;
  public float Z { get; set; } = 0;
  public float EX { get; set; } = 0;
  public float EY { get; set; } = 0;
  public float EZ { get; set; } = 0;
  public int Camp { get; set; } = 0;
}

public class MsgEnterBattle : MsgBase {
  public MsgEnterBattle() { ProtoName = "MsgEnterBattle"; }

  public TankInfo[] Tanks { get; set; }
}

public class MsgBattleResult : MsgBase {
  public MsgBattleResult() { ProtoName = "MsgBattleResult"; }

  public int Result { get; set; } = 0;
}

public class MsgLeaveBattle : MsgBase {
  public MsgLeaveBattle() { ProtoName = "MsgLeaveBattle"; }

  public string ID { get; set; } = "";
}
