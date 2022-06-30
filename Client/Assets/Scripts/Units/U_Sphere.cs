using System.Collections.Generic;
using System.Numerics;

public class U_Sphere : Unit
{
  public U_Sphere(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    Type = UnitType.Sphere;
    Flying = false;
    Speed = 5f;
   
    MaxHealth = 100;
    Health = MaxHealth;
    
    DetectRange = 10f;
    AttackRange = 4f;
    AttackPerSecond = 2f;
    Damage = 10;
  }
}
