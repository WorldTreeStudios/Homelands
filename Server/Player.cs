using System.Net;

public class Player
{
  public IPEndPoint EndPoint { get; set; }
  public float Mana { get; set; }

  public Player(IPEndPoint ip, float mana = 0)
  {
    EndPoint = ip;
    Mana = mana;
  }
}