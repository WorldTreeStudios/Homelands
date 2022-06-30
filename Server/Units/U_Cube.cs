using System.Collections.Generic;
using System.Numerics;

public class U_Cube : Unit
{
  public U_Cube(bool il, float x, float y, float z) : base(il, x, y, z)
  {
    Type = UnitType.Cube;
    Ranged = false;
    Flying = false;
    Speed = 8f;
   
    MaxHealth = 100;
    Health = MaxHealth;
    
    DetectRange = 10f;
    AttackRange = 3f;
    AttackPerSecond = 1;
    Damage = 10;
  }
}