public enum UnitType : byte
{
  Cube,
  Sphere,
  Capsule,
  Cylinder
}

public class Unit
{
  public UnitType type;
  public bool isLeft;

  public bool flying;
  public float speed;
  public float x, y, z;

  // Base constructor for every Unit, instances responsible for setting type, flying, and speed.
  public Unit(bool il, float _x, float _y, float _z)
  {
    isLeft = il; 
    x = _x;
    y = _y;
    z = _z;
  }

  public virtual void Act(float deltaTime) {}
}
