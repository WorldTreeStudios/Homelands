public enum CardId : byte
{
  Cube,
  Sphere
}

public class Card
{
  public CardId Id;
  public UnitType[] Units;
  public int ManaCost;
}
