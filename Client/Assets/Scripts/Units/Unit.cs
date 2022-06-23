using System.Collections.Generic;
public enum UnitType : byte
{
  Cube,
  Sphere,
  Capsule,
  Cylinder
}

public class Unit
{
  public UnitType Type { get; protected set; }
  public bool IsLeft { get; protected set; }

  public bool Flying { get; protected set; }
  public float Speed { get; protected set; }
  public float x, y, z;
  
  // Base constructor for every Unit, instances responsible for setting type, flying, and speed.
  public Unit(bool il, float _x, float _y, float _z)
  {
    IsLeft = il; 
    x = _x;
    y = _y;
    z = _z;
  }

  public virtual void Act(float deltaTime, List<Unit> units) {}
}
