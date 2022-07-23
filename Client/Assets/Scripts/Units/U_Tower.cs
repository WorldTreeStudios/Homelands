public class U_Tower : Unit
{
  public const float Width = 3f; // TODO: This may be applicable for units beyond just towers
  
  // We treat the tower as a unit that doesn't move, for purposes of targetting
  public U_Tower(bool il, float x, float y, float z) : base(il, x, y, z)
  {
    Type = UnitType.Tower;
    Ranged = false;
    Flying = false;

    MaxHealth = 300;
    Health = MaxHealth;

    // TODO: This stuff can control tower auto-attacking
    DetectRange = 0;
    AttackRange = 0;
    AttackPerSecond = 0;
    Damage = 0;
  }
}
