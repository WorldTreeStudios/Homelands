using System.Collections.Generic;
using Vector2 = System.Numerics.Vector2;

public class U_Cube : Unit
{
  public U_Cube(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    Type = UnitType.Cube;
    Flying = false;
    Speed = 8f;
    DetectRange = 15f;
    AttackRange = 3f;
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