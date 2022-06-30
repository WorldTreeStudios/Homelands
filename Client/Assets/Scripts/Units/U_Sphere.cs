using System.Collections.Generic;
using System.Numerics;

public class U_Sphere : Unit
{
  public U_Sphere(bool il, float x, float y, float z) : base(il, x, y, z)
  {
    Type = UnitType.Sphere;
    Ranged = false;
    Flying = false;
    Speed = 5f;
   
    MaxHealth = 100;
    Health = MaxHealth;
    
    DetectRange = 8f;
    AttackRange = 4f;
    AttackPerSecond = 2f;
    Damage = 7;
  }
}
