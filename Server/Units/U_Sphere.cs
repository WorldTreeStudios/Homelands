public class U_Sphere : Unit
{
  public U_Sphere(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    flying = false;
    speed = 2f;
  }

  public override void Act(float deltaTime)
  {
    x = x + deltaTime * (speed * (isLeft ? 1 : -1));
  }
}
