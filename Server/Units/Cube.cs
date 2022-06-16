public class Cube : Unit
{
  public Cube()
  {
    type = UnitType.Cube;
    flying = false;
    speed = 0.5f;
  }

  public Cube(float _x, float _y, float _z)
  {
    type = UnitType.Cube;
    flying = false;
    speed = 0.5f;

    x = _x;
    y = _y;
    z = _z;
  }
 
  public override void Act(float deltaTime)
  {
    x += speed * deltaTime;
  }
}
