using Org.BouncyCastle.Crypto.Generators;

public class RoomManager {
  private static int roomId = 0;
  private static readonly Dictionary<int, Room> rooms = [];

  public static Room? GetRoom(int id) {
    if (!rooms.ContainsKey(id)) {
      return null;
    }
    return rooms[id];
  }

  public static Room AddRoom() {
    Room room = new() {
      id = roomId++
    };
    rooms.Add(room.id, room);
    return room;
  }

  public static void RemoveRoom(int id) {
    if (!rooms.ContainsKey(id)) {
      return;
    }
    rooms.Remove(id);
  }

  public static MsgBase GetMsg() {
    MsgGetRoomList msg = new() {
      Rooms = new RoomInfo[rooms.Count]
    };
    int i = -1;
    foreach (Room room in rooms.Values) {
      i++;
      msg.Rooms[i] = new() {
        ID = room.id,
        Count = room.players.Count,
        Status = room.status == Room.Status.Pending ? 0 : 1
      };
    }
    return msg;
  }

  public static void Update() {
    foreach (Room room in rooms.Values) {
      room.Update();
    }
  }
}
