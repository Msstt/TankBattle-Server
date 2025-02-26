using System;

public class MsgGetScore : MsgBase {
  public MsgGetScore() { ProtoName = "MsgGetScore"; }

  public int Win { get; set; } = 0;
  public int Lose { get; set; } = 0;
}

[Serializable]
public class RoomInfo {
  public int ID { get; set; } = 0;
  public int Count { get; set; } = 0;
  public int Status { get; set; } = 0;
}

public class MsgGetRoomList : MsgBase {
  public MsgGetRoomList() { ProtoName = "MsgGetRoomList"; }

  public RoomInfo[] Rooms { get; set; }
}

public class MsgCreateRoom : MsgBase {
  public MsgCreateRoom() { ProtoName = "MsgCreateRoom"; }

  public int ID { get; set; } = 0;
  public int Result { get; set; } = 0;
}

public class MsgEnterRoom : MsgBase {
  public MsgEnterRoom() { ProtoName = "MsgEnterRoom"; }

  public int ID { get; set; } = 0;
  public int Result { get; set; } = 0;
}

public class MsgLeaveRoom : MsgBase {
  public MsgLeaveRoom() { ProtoName = "MsgLeaveRoom"; }

  public int Result { get; set; } = 0;
}

[Serializable]
public class PlayerInfo {
  public string ID { get; set; } = "";
  public int Win { get; set; } = 0;
  public int Lose { get; set; } = 0;
  public int Camp { get; set; } = 0;
  public int isOwner { get; set; } = 0;
}

public class MsgGetRoomInfo : MsgBase {
  public MsgGetRoomInfo() { ProtoName = "MsgGetRoomInfo"; }

  public int RoomID { get; set; } = 0;
  public PlayerInfo[] Players { get; set; }
}

public class MsgStartGame : MsgBase {
  public MsgStartGame() { ProtoName = "MsgStartGame"; }

  public int Result { get; set; } = 0;
}
