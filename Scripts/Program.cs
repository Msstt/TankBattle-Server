using System;
using System.Text.Json;

public class MainClass {
  private static void Main(string[] args) {
    if (args.Length < 2) {
      return;
    }
    if (!DataBaseManager.Connect("tank_battle", "127.0.0.1", 3306, "root", args[1])) {
      return;
    }
    NetManager.StartLoop(args[0], 8888);
  }
}
