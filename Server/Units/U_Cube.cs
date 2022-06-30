using System.Collections.Generic;
using System.Numerics;

public class U_Cube : Unit
{
  public U_Cube(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    Type = UnitType.Cube;
    Flying = false;
    Speed = 8f;
   
    MaxHealth = 100;
    Health = MaxHealth;
    
    DetectRange = 15f;
    AttackRange = 3f;
    AttackPerSecond = 1;
    Damage = 10;
  }
  
  public override void Act(float deltaTime, List<Unit> units)
  {
    TimeSinceAttack += deltaTime;
    
    Vector2 movementVector = TargetEnemies(units, out Unit target);
    if(target is null)
      movementVector = TargetBase();
    
    x += Speed * movementVector.X * deltaTime;
    z += Speed * movementVector.Y * deltaTime;
  }
}