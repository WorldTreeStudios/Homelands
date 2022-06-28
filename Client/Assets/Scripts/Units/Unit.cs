using System;
using System.Collections.Generic;
using Vector2 = System.Numerics.Vector2;

public enum UnitType : byte
{
  Cube,
  Sphere,
  Capsule,
  Cylinder
}

public class Unit
{
  public UnitType Type { get; protected set; }
  public bool Flying { get; protected set; }
  public float Speed { get; protected set; }
  public float DetectRange { get; protected set; }
  public float AttackRange { get; protected set; }
  
  public bool IsLeft { get; protected set; }
  public float x, y, z;
  
  // Base constructor for every Unit, instances responsible for setting type, flying, and speed.
  public Unit(bool il, float _x, float _y, float _z)
  {
    IsLeft = il; 
    x = _x;
    y = _y;
    z = _z;
  }

  public virtual void Act(float deltaTime, List<Unit> units) {}

  protected Vector2 TargetEnemies(List<Unit> units, out Unit enemyTargeted)
  {
    Vector2 position = new Vector2(x, z);
   
    foreach(Unit u in units)
    {
      if (u.IsLeft == IsLeft) continue;
      
      // Check the distance to every enemy, if they're too far to detect, ignore them
      Vector2 enemyPos = new Vector2(u.x, u.z);
      float distance = Vector2.Distance(position, enemyPos);
      if (!(distance <= DetectRange)) continue;
        
      // If an enemy is close enough to detect, we will target it
      enemyTargeted = u;
       
      // If the enemy is in attack range then stand still to attack it
      if (distance <= AttackRange)
        return new Vector2(0, 0);
        
      // else walk towards it
      Vector2 movement = Vector2.Normalize(enemyPos - position);
      return movement;
    }
    
    // We didn't target any enemies
    enemyTargeted = null;
    return default;
  }
  
  // TODO: This is a fix for the prototype map, should be removed on final map
  private float Flip(float coord)
  {
    return IsLeft ? coord : ((coord - 16) * -1 + 16);
  }
  
  protected Vector2 TargetBase()
  {
    // Here we pretend the unit is on the top left, and simply mirror the movements for other cases 
    Vector2 position = new Vector2(Flip(x), Math.Abs(z));
    Vector2 destination = new Vector2(0, 0);
    
    // TODO: Do these belong here? No. Not sure where they go yet tho lol
    const float bridgeLeft = 8.6f;
    const float bridgeRight = 23.4f;
    const float bridgeBottom = 10f;
    const float bridgeTop = 17f;

    const float baseX = 34f;
    const float baseY = 0f;
   
    // While on the left side, get Z in line with the bridge
    if (position.X < bridgeRight)
    {
      destination.Y = position.Y switch
      {
        (<= bridgeBottom) => bridgeBottom,
        (>= bridgeTop) => bridgeTop,
        _ => position.Y
      };
    }
    // Once crossing the bridge, just target the base
    else
      destination.Y = baseY;
    
    destination.X = position.X switch
    {
      (<= bridgeLeft) => bridgeLeft,
      (<= bridgeRight) => bridgeRight,
      _ => baseX
    };
   
    Vector2 movement = Vector2.Normalize(destination - position);
    movement.X = IsLeft ? movement.X : -movement.X;
    movement.Y = (z >= 0) ? movement.Y : -movement.Y;
    return movement;
  }
}
