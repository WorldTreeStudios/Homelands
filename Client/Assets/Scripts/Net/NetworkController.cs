using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public static class NetworkController
{
  private const int Port = 9999;
  private static UdpClient _connection;
  private static IPEndPoint _endPoint;
  public static Queue<byte[]> Packets;
  
  public static void Connect()
  {
    // Establish connection
    _connection = new UdpClient();

    // TODO: Un-hardcode this IP address
    IPAddress address = IPAddress.Parse("127.0.0.1");
    _endPoint = new IPEndPoint(address, Port);
    _connection.Connect(_endPoint);

    // Send packet to notify server
    Send(new P_Connect());
    
    Packets = new Queue<byte[]>();
    _connection.BeginReceive(Receive, null);
  }
  
  // Send a packet over the current connection
  // This is blocking, can be made Asynchronous with BeginSend if necessary
  public static void Send(Packet p)
  {
    byte[] data = p.Serialize();
    try
    {
      _connection.Send(data, data.Length);
    } catch(Exception _)
    {
      // TODO: Error out of connecting 
    }
  }

  // Receive packets over the current connection ...
  // ... adds them to the packetQueue parsed in HandleMessages()
  private static void Receive(IAsyncResult ar)
  {
    byte[] received = _connection.EndReceive(ar, ref _endPoint);
    Packets.Enqueue(received);
    _connection.BeginReceive(Receive, null);
  }
}
