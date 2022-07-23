using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server
{  
  // Network variables
  private const int Port = 9999;
  private static UdpClient _listener;
  private static IPEndPoint _endPoint;
  private static Queue<IPEndPoint> _playerQueue;

  private static bool _active = true;   
  
  // Gameplay variables
  public static Dictionary<UnitType, Func<bool, float, float, float, Unit>> Behaviors;
  public static Dictionary<CardId, Card> Cards;
  private static List<Game> activeGames;
  private static Dictionary<IPEndPoint, Game> relatedGames;
  
  static void Main(string[] args)
  {
    Behaviors = new Dictionary<UnitType, Func<bool, float, float, float, Unit>>();
    Behaviors.Add(UnitType.Cube, (bool isLeft, float x, float y, float z) => new U_Cube(isLeft, x, y, z));
    Behaviors.Add(UnitType.Sphere, (bool isLeft, float x, float y, float z) => new U_Sphere(isLeft, x, y, z));

    Cards = new Dictionary<CardId, Card>();
    Cards.Add(CardId.Cube, new C_Cube());
    Cards.Add(CardId.Sphere, new C_Sphere());

    _playerQueue = new Queue<IPEndPoint>();
    activeGames = new List<Game>();
    relatedGames = new Dictionary<IPEndPoint, Game>();
    
    _listener = new UdpClient(Port);
    _endPoint = new IPEndPoint(IPAddress.Any, Port);

    while (_active)
    {
      byte[] received = _listener.Receive(ref _endPoint);
      HandleMessage(received, _endPoint);
    }
  }

  private static void HandleMessage(byte[] packet, IPEndPoint endPoint)
  {
    PacketType packetType = (PacketType)packet[0];

    switch (packetType)
    {
      case PacketType.Connect:
        Console.WriteLine("Connection accepted from: " + endPoint.Address + ".");
        _playerQueue.Enqueue(endPoint);
        if (_playerQueue.Count >= 2)
        {
          IPEndPoint p1 = _playerQueue.Dequeue();
          IPEndPoint p2 = _playerQueue.Dequeue();
          Game g = new Game(p1, p2);
          relatedGames.Add(p1,g);
          relatedGames.Add(p2,g);
          activeGames.Add(g);

          Thread t = new Thread(g.Start);
          t.Start();
        }
        break;
      
      default:
        relatedGames[endPoint].Packets.Enqueue((packet, endPoint));
        break;
    }
  }

  public static void Send(Player p, byte[] data)
  {
    _listener.Send(data, data.Length, p.EndPoint);
  }
}
