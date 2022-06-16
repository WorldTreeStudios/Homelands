using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class Server
{
  private const int port = 9999;
  private List<IPEndPoint> connections;

  static void Main(string[] args)
  {
    new Server().Start();
  }

  public void Start()
  {
    UdpClient listener = new UdpClient(port);
    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
    connections = new List<IPEndPoint>();

    bool active = true;

    byte[] received;
    while (active)
    {
      Console.WriteLine("Waiting for data");
      received = listener.Receive(ref endPoint);
      Console.WriteLine("Received data!");

      PacketType packetType = (PacketType)received[0];
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
          parsed.Deserialize(received);
          parsed.x = ((parsed.x - 16) * -1) + 16;
          byte[] response = parsed.Serialize();
          foreach(IPEndPoint ep in connections)
          {
            if(!ep.Equals(endPoint))
              listener.Send(response, response.Length, ep);
          }
          Console.WriteLine("Placed a unit of type: " + parsed.unitType + " at ( " + parsed.x + ", " + parsed.z + " ).");
          break;
      }
    }
    listener.Close();
  }
}
