using System;
using System.Collections.Generic;
using System.Numerics;

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

  // HP
  public int MaxHealth { get; protected set; } 
  public int Health { get; set; }

  // Movement 
  public bool Ranged { get; protected set; }
  public bool Flying { get; protected set; }
  protected float Speed { get; set; }
 
  public bool IsLeft { get; protected set; }
  public Vector2 position;
  public float height;
  
  // Attack
  protected float DetectRange { get; set; }
  protected float AttackRange { get; set; }
  
  protected float AttackPerSecond { get; set; }
  protected float TimeSinceAttack { get; set; }
  protected int Damage { get; set; }
  
  // TODO: Do these belong here? No. Not sure where they go yet tho lol
  private const float bridgeEdge = -6f;
  private const float bridgeBottom = 10f;
  private const float bridgeTop = 17f;

  const float baseX = 22f;
  const float baseZ = 0f;
  
  // Base constructor for every Unit, instances responsible for setting type specific Params.
  public Unit(bool il, float x, float y, float z)
  {
    IsLeft = il;
    position = new Vector2(x, z);
    height = y;
  }

  public void Act(float deltaTime, List<Unit> units)
  {
    TimeSinceAttack += deltaTime;

    Vector2 movementVector = GetMove(units, out Unit toAttack); 
    if (toAttack is not null) Attack(toAttack); 
    
    position.X += Speed * movementVector.X * deltaTime;
    position.Y += Speed * movementVector.Y * deltaTime;
  }

  protected void Attack(Unit target)
  {
    // TODO: Queue these attacks in some way
    if (TimeSinceAttack < (1 / AttackPerSecond)) return;
    
    target.Health -= Damage;
    if (target.Health < 0) target.Health = 0;
    TimeSinceAttack = 0;
  }
  
  protected Vector2 GetMove(List<Unit> units, out Unit toAttack)
  {
    Vector2 movementVector = new Vector2(0, 0);
    toAttack = null;
    
    // First, check for an enemy to target
    Unit target = FindTarget(units);
    
    // If we can target an enemy, either walk towards them, or stand still and attack;
    if (target is not null)
    {
      float distance = Vector2.Distance(position, target.position);
      
      // If we're close enough to attack, we don't need to move, just set the target
      if (distance <= AttackRange) toAttack = target;
      // Otherwise, we walk towards the target
      else movementVector = Vector2.Normalize(target.position - position);
    }
    // If there was no target, we walk towards the base 
    else movementVector = TowardsBase();

    return movementVector;
  }
  
  protected Unit FindTarget(List<Unit> units)
  {
    foreach (Unit u in units)
    {
      // If on the same team, don't target
      if (u.IsLeft == IsLeft) continue;

      // Only flying or ranged units can hit flying enemies 
      if (u.Flying && !(Ranged || Flying)) continue;

      float distance = Vector2.Distance(position, u.position);

      // If unit is out of detection range, don't target;
      if (!(distance <= DetectRange)) continue;

      // If ranged, we can target any unit in our detection range
      if (Ranged) return u;

      // If on the same side, there will be no obstructions to the enemy
      // TODO: Ally collision, Accounting for building targeting units?
      if (position.X <= bridgeEdge && u.position.X <= bridgeEdge ||
          position.X >= -bridgeEdge && u.position.X >= -bridgeEdge)
      {
        return u;
      }
     
      // If units are both on bridges, guaranteed either obstructed or clear (depends on which bridges)
      switch (position.Y)
      {
        // On top bridge
        case >= bridgeBottom and <= bridgeTop:
          switch (u.position.Y)
          {
            case >= bridgeBottom and <= bridgeTop: // Enemy on top bridge too
              return u;
            case <= -bridgeBottom and >= -bridgeTop: // Enemy on bottom bridge
              return null;
          }
          break;
       
        // On bottom bridge
        case <= -bridgeBottom and >= -bridgeTop:
          switch (u.position.Y)
          {
            case >= bridgeBottom and <= bridgeTop: // Enemy on top bridge
              return null;
            case <= -bridgeBottom and >= -bridgeTop: // Enemy on bottom bridge too
              return u;
          }
          break;
      }

      // General case, find if the line between units crosses the river at a bridge opening 
      float slope = (u.position.Y - position.Y) / (u.position.X - position.X); // m = y2-y1 / x2 - x1
     
      float yAtLeftRiver = slope * (bridgeEdge - position.X) + position.Y; // y = m(x - x1) + y1
      bool validLeft = (yAtLeftRiver is (< bridgeTop and > bridgeBottom) or (< -bridgeBottom and > -bridgeTop));
      bool crossesLeft = (position.X < bridgeEdge && bridgeEdge < u.position.X ||
                          position.X > bridgeEdge && bridgeEdge > u.position.X);
      bool left = (!crossesLeft || validLeft); // Either the line doesn't cross, or it crosses at bridge
      
      float yAtRightRiver = slope * (-bridgeEdge - position.X) + position.Y;
      bool validRight = (yAtRightRiver is (< bridgeTop and > bridgeBottom) or (< -bridgeBottom and > -bridgeTop)); 
      bool crossesRight = (position.X < -bridgeEdge && -bridgeEdge < u.position.X ||
                          position.X > -bridgeEdge && -bridgeEdge > u.position.X);
      bool right = (!crossesRight || validRight); // Either the line doesn't cross, or it crosses at bridge
     
      // If both checks pass, there is a valid line between the units
      return (left && right) ? u : null;
    }

    return null;
  }

  protected Vector2 TowardsBase()
  {
    Vector2 destination = position;
    
    float travelledX = IsLeft ? position.X : -position.X;
    float absY = Math.Abs(position.Y); 
    
    // While on the left side or bridge, get Z in line with the bridge
    if (travelledX < -bridgeEdge)
    {
      destination.Y = absY switch
      {
        (<= bridgeBottom) => bridgeBottom,
        (>= bridgeTop) => bridgeTop,
        _ => absY
      };
    }
    // Once crossing the bridge, just target the base
    else
      destination.Y = baseZ;
    
    destination.X = travelledX switch
    {
      (<= bridgeEdge) => bridgeEdge,
      (<= -bridgeEdge) => -bridgeEdge,
      _ => baseX
    };
    
    destination.X *= IsLeft ? 1 : -1;
    destination.Y *= (position.Y >= 0) ? 1 : -1;
    
    Vector2 movement = Vector2.Normalize(destination - position);
    return movement;
  }
}
