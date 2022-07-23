using System.Net;

public class Player
{
  public IPEndPoint EndPoint { get; set; }
  public float Mana { get; set; }
  public bool Left { get; set; }
  
  public Player(IPEndPoint ip, bool left, float mana = GameDefs.StartingMana)
  {
    EndPoint = ip;
    Left = left;
    Mana = mana;
  }
}