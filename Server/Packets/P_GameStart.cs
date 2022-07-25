public class P_GameStart : Packet
{
  public P_GameStart()
  {
    code = PacketType.GameStart;
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
