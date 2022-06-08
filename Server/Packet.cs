public enum PacketType : byte
{
  Connect,
  PlaceUnit
}

public abstract class Packet
{
  public PacketType code;
  
  // Turn instance variables into bytes to send over UDP
  public abstract byte[] Serialize();

  // Set instance variables based on data sent over UDP 
  public abstract void Deserialize(byte[] data);
}
