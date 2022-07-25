public enum PacketType : byte
{
  GameStart,
  Connect,
  PlaceUnit,
  PlayCard 
}

public abstract class Packet
{
  public PacketType code;

  public abstract byte[] Serialize();

  public abstract void Deserialize(byte[] data);
}
