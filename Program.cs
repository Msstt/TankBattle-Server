using System;
using System.Text.Json;

public class MainClass {
  private static void Main(string[] args) {
    if (args.Length < 4) {
      return;
    }
    if (!DataBaseManager.Connect("tank_battle", "127.0.0.1", 3306, args[2], args[3])) {
      return;
    }
    NetManager.StartLoop(args[0], int.Parse(args[1]));
  }
}
