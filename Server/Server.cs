using System;
using System.Net;
using System.Net.Sockets;

public class Server
{
  private const int port = 9999;

  static void Main(string[] args)
  {
    UdpClient listener = new UdpClient(port);
    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
    bool active = true;

    byte[] received;
    while (active)
    {
      Console.WriteLine("Waiting for data");
      received = listener.Receive(ref endPoint);
      Console.WriteLine("Received data!");

      PacketType packetType = (PacketType)received[0];
      switch(packetType)
      {
        case PacketType.Connect:
          break;
        case PacketType.PlaceUnit:
          P_PlaceUnit parsed = new P_PlaceUnit();
          parsed.Deserialize(received);

          Console.WriteLine("Placed a unit of type: " + parsed.unitType + " at ( " + parsed.x + ", " + parsed.z + " ).");
          break;
      }
    }
    listener.Close();
  }
}
