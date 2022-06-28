using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server
{  
  // Network variables
  private const int Port = 9999;
  private List<IPEndPoint> _connections;
  private UdpClient _listener;
  private IPEndPoint _endPoint;
  private Queue<(byte[], IPEndPoint)> _packets;

  // Gameplay variables
  private List<Player> _players;
  private List<Unit> _units;
  private static Dictionary<UnitType, Func<bool, float, float, Unit>> _behaviors;
  private const float ManaPerSecond = .5f; 
  
  // Time variables
  private static Stopwatch _stopwatch;
  private const int TickRate = 20;

  static void Main(string[] args)
  {
    // Initialize static variables
    _behaviors = new Dictionary<UnitType, Func<bool, float, float, Unit>>();
    _behaviors.Add(UnitType.Cube, (bool isLeft, float x, float z) => new U_Cube(isLeft, x, 0, z));
    _behaviors.Add(UnitType.Sphere, (bool isLeft, float x, float z) => new U_Sphere(isLeft, x, 0, z));

    _stopwatch = new Stopwatch();

    // Start a server instance
    new Server().Start();

  }

  public void Start()
  {
    // Initialize instance variables
    _listener = new UdpClient(Port);
    _endPoint = new IPEndPoint(IPAddress.Any, Port);
    _connections = new List<IPEndPoint>();
    _packets = new Queue<(byte[], IPEndPoint)>();

    _players = new List<Player>();
    _units = new List<Unit>();
    _stopwatch.Start();

    // Start receiving data asynchronously over UDP
    _listener.BeginReceive(new AsyncCallback(Receive), null);

    // Start the server's loop
    Update();

    // After loop exits, stop timer, close UDP connection
    _stopwatch.Stop();
    _listener.Close();
  }

  public void Update()
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

      foreach (Player p in _players)
      {
        p.Mana += delta * ManaPerSecond;
      }

      // Parse messages that have been received since last update
      HandleMessages();

      Thread.Sleep(1000 / TickRate);
    }
  }

  // Parse packet queue
  private void HandleMessages()
  {
    while (_packets.Count > 0)
    {
      (byte[] packet, IPEndPoint endPoint) = _packets.Dequeue();
      PacketType packetType = (PacketType)packet[0];

      switch (packetType)
      {
        case PacketType.Connect:
          Console.WriteLine("Connection accepted from: " + endPoint.Address + ".");
          if (!_connections.Contains(endPoint))
          {
            _connections.Add(endPoint);
            _players.Add(new Player(endPoint));
          }
          break;

        case PacketType.PlaceUnit:

          P_PlaceUnit parsed = new P_PlaceUnit();
          parsed.Deserialize(packet);
          parsed.x = ((parsed.x - 16) * -1) + 16;
          bool left = _connections[0].Equals(endPoint);
          _units.Add(_behaviors[parsed.unitType](left, parsed.x, parsed.z));

          byte[] response = parsed.Serialize();
          foreach (IPEndPoint ep in _connections)
          {
            if (!ep.Equals(endPoint))
              _listener.Send(response, response.Length, ep);
          }
          Console.WriteLine("Placed a unit of type: " + parsed.unitType + " at ( " + parsed.x + ", " + parsed.z + " ).");
          break;
      }
    }
  }

  private void Receive(IAsyncResult ar)
  {
    byte[] received = _listener.EndReceive(ar, ref _endPoint);

    // Add received data to the queue for parsing ...
    _packets.Enqueue((received, _endPoint));
    // ... and recursively start listening for data again
    _listener.BeginReceive(new AsyncCallback(Receive), null);
  }
}
