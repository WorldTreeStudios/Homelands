using System;

public class P_PlaceUnit : Packet
{
  public UnitType unitType;
  public float x, z;

  public P_PlaceUnit()
  {
    code = PacketType.PlaceUnit;
  }

  public P_PlaceUnit(UnitType ut, float _x, float _z)
  {
    code = PacketType.PlaceUnit;

    unitType = ut;
    x = _x;
    z = _z;
  }

  public override byte[] Serialize()
  {
    byte[] result = new byte[1 + 1 + 2 * sizeof(float)];
    result[0] = (byte)code;
    result[1] = (byte)unitType;
    float[] coords = { x, z };
    Buffer.BlockCopy(coords, 0, result, 2, 2 * sizeof(float));
    return result;
  }

  public override void Deserialize(byte[] data)
  {
    code = (PacketType)data[0];
    unitType = (UnitType)data[1];

    float[] coords = new float[2];
    Buffer.BlockCopy(data, 2, coords, 0, 2 * sizeof(float));
    x = coords[0];
    z = coords[1];
  }
}