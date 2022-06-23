using System.Collections.Generic;
using System.Numerics;

public class U_Sphere : Unit
{
  public U_Sphere(bool il, float _x, float _y, float _z) : base(il, _x, _y, _z)
  {
    Type = UnitType.Sphere;
    Flying = false;
    Speed = 5f;
  }
  
  public override void Act(float deltaTime, List<Unit> units)
  {
    //Pathfinding TODO: This probably requires a smarter algorithm
    
    // TODO: The math is ugly because of the prototype map right now, but if we pretend ...
    // ... every unit is on the left side, we can use the same destinations for pathfinding
    Vector2 leftPosition = new Vector2(IsLeft ? x : ((x - 16) * -1) + 16, z);
   
    bool isTop = z >= 0;
    float bridgeHeight = isTop ? 13.6f : -13.6f;
 
    // We will set the destination depending on the current location of the unit
    // TODO: Smarter destination refinement
    Vector2 destination;
    if (leftPosition.X <= 8.6f) // Unit on left side of the arena
    {
      destination = new Vector2(8.6f, bridgeHeight);
    }
    else if (leftPosition.X <= 23.4f) // Unit on the bridge 
    {
      destination = new Vector2(23.4f, bridgeHeight);
    }
    else // Unit on the right side of the arena
    {
      destination = new Vector2(34f, 0f);
    }
    
    Vector2 movement = Vector2.Normalize(destination - leftPosition);
    float horizontalSpeed = Speed * (IsLeft ? 1 : -1);
    
    x += horizontalSpeed * movement.X * deltaTime;
    z += Speed * movement.Y * deltaTime;
  }
}
