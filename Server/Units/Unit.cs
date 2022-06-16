public enum UnitType : byte
{
  Cube,
  Sphere,
  Capsule,
  Cylinder
}
public abstract class Unit
{
  public UnitType type;
  public bool flying;
  public float speed;
  public float x, y, z;

  public abstract void Act(float deltaTime);
}
