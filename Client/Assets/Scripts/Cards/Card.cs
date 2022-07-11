public enum CardId : byte
{
  Cube,
  Sphere
}

public abstract class Card
{
  public CardId Id;
  public UnitType[] Units;
  public int ManaCost;
}
