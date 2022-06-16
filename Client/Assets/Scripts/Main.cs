using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Main : MonoBehaviour
{
  [SerializeField]
  private DeckController DeckController;
  [SerializeField]
  private UnitController UnitController;

  private const int port = 9999;
  private UdpClient connection;
  private IPEndPoint endPoint;

  private Queue<byte[]> packets;

  public void Start()
  {
    // Establish connection
    connection = new UdpClient();

    // TODO: Un-hardcode this IP address
    IPAddress address = IPAddress.Parse("127.0.0.1");
    endPoint = new IPEndPoint(address, port);
    connection.Connect(endPoint);

    // Send packet to notify server
    Send(new P_Connect());

    packets = new Queue<byte[]>();
    connection.BeginReceive(new AsyncCallback(Receive), null);
  }

  public void Update()
  {
    HandleInput();

    // Receive/Parse messages from the server
    HandleMessages();
  }

  private void HandleInput()
  {
    if (Input.GetMouseButtonDown(0) && DeckController.SelectedCard != null)
    {
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out hit))
      {
        // Spawn the Unit ...
        UnitType toSpawn = DeckController.SelectedUnit;
        GameObject spawnedUnit = Instantiate(UnitController.prefabs[toSpawn]);

        // ... at the location of the raycast collision
        float unitHeight = spawnedUnit.GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 offsetVector = new Vector3(0, unitHeight, 0);
        Vector3 spawnPos = hit.point + offsetVector;
        spawnedUnit.transform.position = spawnPos; // TODO: Further validate location

        // ... assign the unit's behavior script
        spawnedUnit.GetComponent<UnitBehavior>().unit = UnitController.behaviors[toSpawn](spawnPos);

        // ... and send it to the server
        Send(new P_PlaceUnit(toSpawn, spawnPos.x, spawnPos.z));

        // TODO: Control active cards based on server data
        // Remove the card
        DeckController.SelectedCard.SetActive(false);
        DeckController.SelectedCard = null;
      }
    }
  }

  private void HandleMessages()
  {
    while (packets.Count > 0)
    {
      byte[] packet = packets.Dequeue();
      PacketType packetType = (PacketType)packet[0];
      switch(packetType)
      {
        case PacketType.Connect:
          break;
        case PacketType.PlaceUnit:
          // Spawn the unit, TODO: Use unit definitions here
          P_PlaceUnit p = new P_PlaceUnit();
          p.Deserialize(packet);

          GameObject spawnedUnit = Instantiate(UnitController.prefabs[p.unitType]);
          spawnedUnit.transform.position = new Vector3(p.x, 10.5f, p.z);
          spawnedUnit.GetComponent<UnitBehavior>().unit = UnitController.behaviors[p.unitType](spawnedUnit.transform.position);
          break;
      }
    }
  }

  // Send a packet over the current connection
  // This is blocking, can be made Asynchronous with BeginSend if necessary
  public void Send(Packet p)
  {
    byte[] data = p.Serialize();
    try
    {
      connection.Send(data, data.Length);
    } catch(Exception e)
    {
      Debug.Log(e.ToString());
    }
  }

  // Receive packets over the current connection ...
  // ... adds them to the packetQueue parsed in HandleMessages()
  private void Receive(IAsyncResult ar)
  {
    byte[] received = connection.EndReceive(ar, ref endPoint);
    packets.Enqueue(received);
    connection.BeginReceive(new AsyncCallback(Receive), null);
  }

  public void OnDestroy()
  {
    connection.Close();
  }
}
