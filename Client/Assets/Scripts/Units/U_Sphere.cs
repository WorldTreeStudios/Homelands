public class U_Sphere : Unit
{
  public U_Sphere(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    Type = UnitType.Sphere;
    Flying = false;
    Speed = 2f;
  }

  public override void Act(float deltaTime)
  {
    x = x + deltaTime * (Speed * (IsLeft ? 1 : -1));
  }
}
