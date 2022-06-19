public class U_Cube : Unit
{
  public U_Cube(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    flying = false;
    speed = 1f;
  }

  public override void Act(float deltaTime)
  {
    x = x + deltaTime * (speed * (isLeft ? 1 : -1));
  }
}
