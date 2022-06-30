using System;

public class P_PlaceUnit : Packet
{
  public UnitType unitType;
  public float x, y, z;

  public P_PlaceUnit()
  {
    code = PacketType.PlaceUnit;
  }

  public P_PlaceUnit(UnitType ut, float x, float y, float z)
  {
    code = PacketType.PlaceUnit;

    unitType = ut;
    this.x = x;
    this.y = y;
    this.z = z;
  }

  public override byte[] Serialize()
  {
    byte[] result = new byte[1 + 1 + 3 * sizeof(float)];
    result[0] = (byte)code;
    result[1] = (byte)unitType;
    float[] coords = { x, y, z };
    Buffer.BlockCopy(coords, 0, result, 2, 3 * sizeof(float));
    return result;
  }

  public override void Deserialize(byte[] data)
  {
    code = (PacketType)data[0];
    unitType = (UnitType)data[1];

    float[] coords = new float[3];
    Buffer.BlockCopy(data, 2, coords, 0, 3 * sizeof(float));
    x = coords[0];
    y = coords[1];
    z = coords[2];
  }
}