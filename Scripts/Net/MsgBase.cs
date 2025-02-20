using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

public class MsgBase {
  public string ProtoName { get; set; } = "";

  public static byte[] Encode(MsgBase msg) {
    string msgString = JsonConvert.SerializeObject(msg);
    return System.Text.Encoding.UTF8.GetBytes(msgString);
  }

  public static MsgBase? Decode(string protoName, byte[] bytes, int offset, int count) {
    string msgString = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
    MsgBase msg;
    try {
      msg = JsonConvert.DeserializeObject(msgString, Type.GetType(protoName)) as MsgBase;
    } catch (JsonReaderException) {
      return null;
    }
    return msg;
  }

  public static byte[] EncodeName(MsgBase msg) {
    byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.ProtoName);
    byte[] bytes = new byte[2 + nameBytes.Length];
    bytes[0] = (byte)(nameBytes.Length % 256);
    bytes[1] = (byte)(nameBytes.Length / 256);
    Array.Copy(nameBytes, 0, bytes, 2, nameBytes.Length);
    return bytes;
  }

  public static string DecodeName(byte[] bytes, int offset, out int count) {
    count = 0;
    if (offset + 2 > bytes.Length) {
      return "";
    }
    Int16 length = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
    if (offset + 2 + length > bytes.Length) {
      return "";
    }
    count = 2 + length;
    return System.Text.Encoding.UTF8.GetString(bytes, offset + 2, length);
  }
}
