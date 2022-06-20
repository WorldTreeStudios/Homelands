using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Main : MonoBehaviour
{
  [SerializeField]
  private DeckController deckController;
  [SerializeField]
  private UnitController unitController;

  private const int Port = 9999;
  private UdpClient _connection;
  private IPEndPoint _endPoint;

  private Queue<byte[]> _packets;

  public void Start()
  {
    // Establish connection
    _connection = new UdpClient();

    // TODO: Un-hardcode this IP address
    IPAddress address = IPAddress.Parse("127.0.0.1");
    _endPoint = new IPEndPoint(address, Port);
    _connection.Connect(_endPoint);

    // Send packet to notify server
    Send(new P_Connect());

    _packets = new Queue<byte[]>();
    _connection.BeginReceive(new AsyncCallback(Receive), null);
  }

  public void Update()
  {
    HandleInput();

    // Receive/Parse messages from the server
    HandleMessages();
  }

  private void HandleInput()
  {
    if (Input.GetMouseButtonDown(0) && deckController.SelectedCard != null)
    {
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out hit))
      {
        // Spawn the Unit ...
        UnitType toSpawn = deckController.SelectedUnit;
        GameObject spawnedUnit = Instantiate(unitController.Prefabs[toSpawn]);

        // ... at the location of the raycast collision
        float unitHeight = spawnedUnit.GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 offsetVector = new Vector3(0, unitHeight, 0);
        Vector3 spawnPos = hit.point + offsetVector;
        spawnedUnit.transform.position = spawnPos; // TODO: Further validate location

        // ... assign the unit's behavior script
        spawnedUnit.GetComponent<UnitBehavior>().unit = unitController.Behaviors[toSpawn](true, spawnPos);

        // ... and send it to the server
        Send(new P_PlaceUnit(toSpawn, spawnPos.x, spawnPos.z));

        // TODO: Control active cards based on server data
        // Remove the card
        deckController.SelectedCard.SetActive(false);
        deckController.SelectedCard = null;
      }
    }
  }

  private void HandleMessages()
  {
    while (_packets.Count > 0)
    {
      byte[] packet = _packets.Dequeue();
      PacketType packetType = (PacketType)packet[0];
      switch(packetType)
      {
        case PacketType.Connect:
          break;
        case PacketType.PlaceUnit:
          // Spawn the unit, TODO: Use unit definitions here
          P_PlaceUnit p = new P_PlaceUnit();
          p.Deserialize(packet);

          GameObject spawnedUnit = Instantiate(unitController.Prefabs[p.unitType]);
          spawnedUnit.transform.position = new Vector3(p.x, 10.5f, p.z);
          spawnedUnit.GetComponent<UnitBehavior>().unit = unitController.Behaviors[p.unitType](false, spawnedUnit.transform.position);
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
      _connection.Send(data, data.Length);
    } catch(Exception e)
    {
      Debug.Log(e.ToString());
    }
  }

  // Receive packets over the current connection ...
  // ... adds them to the packetQueue parsed in HandleMessages()
  private void Receive(IAsyncResult ar)
  {
    byte[] received = _connection.EndReceive(ar, ref _endPoint);
    _packets.Enqueue(received);
    _connection.BeginReceive(new AsyncCallback(Receive), null);
  }

  public void OnDestroy()
  {
    _connection.Close();
  }
}
