using System.Collections.Generic;
using System.Numerics;

public class U_Sphere : Unit
{
  public U_Sphere(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    Type = UnitType.Sphere;
    Flying = false;
    Speed = 5f;
    DetectRange = 10f;
    AttackRange = 4f;
  }
  
  public override void Act(float deltaTime, List<Unit> units)
  {
    Vector2 movementVector = TargetEnemies(units, out Unit enemyTargeted);
    if(enemyTargeted is null)
      movementVector = TargetBase();
    
    x += Speed * movementVector.X * deltaTime;
    z += Speed * movementVector.Y * deltaTime;
  }
}
