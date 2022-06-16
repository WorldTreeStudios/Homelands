using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server
{  
  // Network variables
  private const int port = 9999;
  private List<IPEndPoint> connections;
  private UdpClient listener;
  private IPEndPoint endPoint;
  private Queue<(byte[], IPEndPoint)> packets;

  // Gameplay variables
  private bool active = true;
  private List<Unit> units;
  private static Dictionary<UnitType, Func<float, float, Unit>> behaviors;
  
  // Time variables
  private static Stopwatch stopwatch;
  private const int TICKRATE = 20;

  static void Main(string[] args)
  {
    // Initialize static variables
    behaviors = new Dictionary<UnitType, Func<float, float, Unit>>();
    behaviors.Add(UnitType.Cube, (float x, float z) => new U_Cube(x, 0, z));
    
    stopwatch = new Stopwatch();

    // Start a server instance
    new Server().Start();

  }

  public void Start()
  {
    // Initialize instance variables
    listener = new UdpClient(port);
    endPoint = new IPEndPoint(IPAddress.Any, port);
    connections = new List<IPEndPoint>();
    packets = new Queue<(byte[], IPEndPoint)>();

    units = new List<Unit>();
    stopwatch.Start();

    // Start receiving data asynchronously over UDP
    listener.BeginReceive(new AsyncCallback(Receive), null);

    // Start the server's loop
    Update();

    // After loop exits, stop timer, close UDP connection
    stopwatch.Stop();
    listener.Close();
  }

  public void Update()
  {
    double time = stopwatch.Elapsed.TotalSeconds;
    float delta;

    while (active)
    {
      // Calculate deltaTime from last update
      double currTime = stopwatch.Elapsed.TotalSeconds;
      (delta, time) = ((float) (currTime - time), currTime);

      foreach(Unit u in units)
      {
        u.Act(delta);
      }

      // Parse messages that have been received since last update
      HandleMessages();

      Thread.Sleep(1000 / TICKRATE);
    }
  }

  // Parse packet queue
  private void HandleMessages()
  {
    while (packets.Count > 0)
    {
      (byte[] packet, IPEndPoint endPoint) = packets.Dequeue();
      PacketType packetType = (PacketType)packet[0];

      switch (packetType)
      {
        case PacketType.Connect:
          Console.WriteLine("Connection accepted from: " + endPoint.Address + ".");
          if (!connections.Contains(endPoint))
          {
            connections.Add(endPoint);
          }
          break;

        case PacketType.PlaceUnit:

          P_PlaceUnit parsed = new P_PlaceUnit();
          parsed.Deserialize(packet);
          parsed.x = ((parsed.x - 16) * -1) + 16;
          units.Add(behaviors[parsed.unitType](parsed.x, parsed.z));

          byte[] response = parsed.Serialize();
          foreach (IPEndPoint ep in connections)
          {
            if (!ep.Equals(endPoint))
              listener.Send(response, response.Length, ep);
          }
          Console.WriteLine("Placed a unit of type: " + parsed.unitType + " at ( " + parsed.x + ", " + parsed.z + " ).");
          break;
      }
    }
  }

  private void Receive(IAsyncResult ar)
  {
    byte[] received = listener.EndReceive(ar, ref endPoint);

    // Add received data to the queue for parsing ...
    packets.Enqueue((received, endPoint));
    // ... and recursively start listening for data again
    listener.BeginReceive(new AsyncCallback(Receive), null);
  }
}
