using System;
using System.Text.Json;

public class MainClass {
  private static void Main(string[] args) {
    if (args.Length < 1) {
      return;
    }
    if (!DataBaseManager.Connect("tank_battle", "127.0.0.1", 3306, "root", args[0])) {
      return;
    }
    NetManager.StartLoop("127.0.0.1", 8888);
  }
}
