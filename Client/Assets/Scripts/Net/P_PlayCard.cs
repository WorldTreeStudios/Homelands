using System;

public class P_PlayCard : Packet
{
  public CardId card;
  public float x, y, z;

  public P_PlayCard()
  {
    code = PacketType.PlayCard;
  }

  public P_PlayCard(CardId cardId, float x, float y, float z)
  {
    code = PacketType.PlayCard;

    card = cardId;
    this.x = x;
    this.y = y;
    this.z = z;
  }

  public override byte[] Serialize()
  {
    byte[] result = new byte[1 + 1 + 3 * sizeof(float)];
    result[0] = (byte)code;
    result[1] = (byte)card;
    float[] coords = { x, y, z };
    Buffer.BlockCopy(coords, 0, result, 2, 3 * sizeof(float));
    return result;
  }

  public override void Deserialize(byte[] data)
  {
    code = (PacketType)data[0];
    card = (CardId)data[1];

    float[] coords = new float[3];
    Buffer.BlockCopy(data, 2, coords, 0, 3 * sizeof(float));
    x = coords[0];
    y = coords[1];
    z = coords[2];
  }
}