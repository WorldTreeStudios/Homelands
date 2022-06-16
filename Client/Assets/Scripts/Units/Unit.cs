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
  public bool flying;
  public float speed;
  public float x, y, z;

  public virtual void Act(float deltaTime) {}
}
