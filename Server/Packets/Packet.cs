public enum PacketType : byte
{
  Connect,
  PlaceUnit
}

public abstract class Packet
{
  public PacketType code;

  public abstract byte[] Serialize();

  public abstract void Deserialize(byte[] data);
}
