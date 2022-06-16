using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_Cube : Unit
{
  public U_Cube(float _x, float _y, float _z)
  {
    x = _x;
    y = _y;
    z = _z;

    flying = false;
    speed = 1f;
  }

  public override void Act(float deltaTime)
  {
    x = x + speed * deltaTime;   
  }
}
