using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

public class Game
{
  // Networking
  public Queue<(byte[], IPEndPoint)> Packets;
  private Dictionary<IPEndPoint, Player> _players;
 
  // Gameplay 
  private List<Unit> _units;
  
  // Time variables
  private static Stopwatch _stopwatch;
  private const int TickRate = 20;

  public Game(IPEndPoint p1, IPEndPoint p2)
  {
    Packets = new Queue<(byte[], IPEndPoint)>();

    _players = new Dictionary<IPEndPoint, Player>();
    _players.Add(p1, new Player(p1,true));
    _players.Add(p2, new Player(p2,false));
    
    _units = new List<Unit>();
    _stopwatch = new Stopwatch();
  }

  public void Start()
  {
    _stopwatch.Start();
    
    Update();
  }
  
  private void Update()
  {
    double time = _stopwatch.Elapsed.TotalSeconds;
    float delta;
    
    while (true)
    {
      // Calculate deltaTime from last update
      double currTime = _stopwatch.Elapsed.TotalSeconds;
      (delta, time) = ((float) (currTime - time), currTime);

      foreach(Unit u in _units)
      {
        u.Act(delta, _units);
      }
      _units.RemoveAll(u => (u.Health <= 0));
      
      foreach (Player p in _players.Values)
      {
        float increase = delta * GameDefs.ManaPerSecond;
        if ((p.Mana + increase) < GameDefs.MaxMana)
          p.Mana += increase; 
        else
          p.Mana = GameDefs.MaxMana;
      }

      // Parse messages that have been received since last update
      HandleMessages();

      Thread.Sleep(1000 / TickRate);
    }
  }
  
  // Parse packet queue
  private void HandleMessages()
  {
    while (Packets.Count > 0)
    {
      (byte[] packet, IPEndPoint endPoint) = Packets.Dequeue();
      PacketType packetType = (PacketType)packet[0];

      switch (packetType)
      {
        case PacketType.PlayCard:
          P_PlayCard parsed = new P_PlayCard();
          parsed.Deserialize(packet);
         
          Player p = _players[endPoint];
          Card c = Server.Cards[parsed.card];
          if (p.Mana < c.ManaCost) break;
          p.Mana -= c.ManaCost; 
          
          foreach (UnitType ut in c.Units)
          {
            // Spawn units from left player at true x, mirror x for units placed by the right player
            float serverX = p.Left ? parsed.x : -parsed.x;
            _units.Add(Server.Behaviors[ut](p.Left, serverX, parsed.y, parsed.z));

            P_PlaceUnit response = new P_PlaceUnit(ut, -parsed.x, parsed.y, parsed.z);
            byte[] data = response.Serialize();
            foreach (var entry in _players.Where(entry => !Equals(entry.Key, endPoint)))
            {
              Server.Send(entry.Value, data);
            }
            
            Console.WriteLine("Placed a unit of type: " + ut + " at ( " + parsed.x + ", " + parsed.z + " ).");
          }
          break;
      }
    }
  }
}