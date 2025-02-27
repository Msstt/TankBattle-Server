public class MsgSyncTank : MsgBase {
  public MsgSyncTank() { ProtoName = "MsgSyncTank"; }

  public float X { get; set; } = 0;
  public float Y { get; set; } = 0;
  public float Z { get; set; } = 0;
  public float EX { get; set; } = 0;
  public float EY { get; set; } = 0;
  public float EZ { get; set; } = 0;
  public float TurretY { get; set; } = 0;
  public float Time { get; set; } = 0;
  public string ID { get; set; } = "";
}

public class MsgFire : MsgBase {
  public MsgFire() { ProtoName = "MsgFire"; }

  public float X { get; set; } = 0;
  public float Y { get; set; } = 0;
  public float Z { get; set; } = 0;
  public float EX { get; set; } = 0;
  public float EY { get; set; } = 0;
  public float EZ { get; set; } = 0;
  public string ID { get; set; } = "";
}

public class MsgHit : MsgBase {
  public MsgHit() { ProtoName = "MsgHit"; }

  public string ID { get; set; } = "";
  public float Health { get; set; } = 0;
}
