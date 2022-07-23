using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;

public enum UnitType : byte
{
  Cube,
  Sphere,
  Tower
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
  
  // Base constructor for every Unit, instances responsible for setting type specific Params.
  public Unit(bool il, float x, float y, float z)
  {
    TimeSinceAttack = ((1 / AttackPerSecond) - 1);
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
      
      // Lastly, check that walking to the target won't cross invalid space
      if (!GameDefs.CrossesRiver(position, u.position)) return u;
    }
    
    return null;
  }

  protected Vector2 TowardsBase()
  {
    Vector2 destination = new Vector2(GameDefs.BaseX, GameDefs.BaseZ);
    float normalX = IsLeft ? position.X : -position.X;
    float normalY = Math.Abs(position.Y);
    
    // Midlane
    if (position.Y is < GameDefs.MidZ and > -GameDefs.MidZ )
    {
      switch (normalX)
      {
        // Before the bridge 
        case < -GameDefs.MidBarrier:
          destination.X = -GameDefs.MidBarrier;
          destination.Y = destination.Y switch
          {
            (> GameDefs.MidBridge) => GameDefs.MidBridge,
            (< -GameDefs.MidBridge) => -GameDefs.MidBridge,
            (_) => destination.Y
          };
          break;
        // On the bridge TODO: Make them attack the orb
        case < GameDefs.MidBarrier:
          destination.X = GameDefs.MidBarrier;
          destination.Y = position.Y;
          break;
      }
    }
    // Top or bottom lane
    else
    {
      switch (normalX)
      {
        // Before the bridge
        case < -GameDefs.ExtremeBarrier:
          destination.X = -GameDefs.ExtremeBarrier;
          destination.Y = normalY switch
          {
            (> (GameDefs.BridgeZ + GameDefs.BridgeWidth)) => (GameDefs.BridgeZ + GameDefs.BridgeWidth),
            (< GameDefs.BridgeZ) => (GameDefs.BridgeZ),
            (_) => normalY
          };
          break;
        // On the bridge
        case < GameDefs.ExtremeBarrier:
          destination.X = GameDefs.ExtremeBarrier;
          destination.Y = normalY;
          break;
      }
    }

    destination.X *= IsLeft ? 1 : -1;
    destination.Y *= (position.Y > 0) ? 1 : -1;
    
    Vector2 movement = Vector2.Normalize(destination - position);
    return movement;
  }
}
