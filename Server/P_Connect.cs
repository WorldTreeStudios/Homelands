public class P_Connect : Packet
{
  // TODO : What information needs to be sent when connecting?
  public P_Connect()
  {
    code = PacketType.Connect;
  }

  public override byte[] Serialize()
  {
    byte[] result = { (byte)code };
    return result;
  }

  public override void Deserialize(byte[] data)
  {

  }
}
