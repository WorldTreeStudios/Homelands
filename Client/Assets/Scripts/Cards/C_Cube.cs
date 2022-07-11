public class C_Cube : Card
{
  public C_Cube()
  {
    Id = CardId.Cube;
    Units = new[] { UnitType.Cube };
    ManaCost = 1;
  }
}