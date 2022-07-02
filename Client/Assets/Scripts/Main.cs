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
  [SerializeField]
  private GameObject healthBar;
  
  private const int Port = 9999;
  private UdpClient _connection;
  private IPEndPoint _endPoint;

  private Queue<byte[]> _packets;

  public static List<Unit> units;
  
  public void Start()
  {
    // Setup game variables
    units = new List<Unit>();
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
    // The user left clicked with a card selected
    if (Input.GetMouseButtonDown(0) && deckController.SelectedCard != null)
    {
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out hit))
      {
        // Get the appropriate unit type 
        UnitType toSpawn = deckController.SelectedUnit;
        
        // ... and the location of the raycast collision
        float unitHeight = unitController.Prefabs[toSpawn].GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 offsetVector = new Vector3(0, unitHeight, 0);
        Vector3 spawnPos = hit.point + offsetVector;
       
        // ... then spawn the unit
        SpawnUnit(toSpawn, spawnPos, true);
        
        // ... and send it to the server
        Send(new P_PlaceUnit(toSpawn, spawnPos.x, spawnPos.y, spawnPos.z));

        // TODO: Control active cards based on server data
        // Remove the card
        deckController.SelectedCard.SetActive(false);
        deckController.SelectedCard = null;
      }
    }
    // Debug, spawn enemy units with right click
    else if (Input.GetMouseButtonDown(1) && deckController.SelectedCard != null)
    {
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit))
      {
        UnitType toSpawn = deckController.SelectedUnit;
        float unitHeight = unitController.Prefabs[toSpawn].GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 offsetVector = new Vector3(0, unitHeight, 0);
        Vector3 spawnPos = hit.point + offsetVector;
        SpawnUnit(toSpawn, spawnPos, false);
        Send(new P_PlaceUnit(toSpawn, spawnPos.x, spawnPos.y, spawnPos.z));
        deckController.SelectedCard.SetActive(false);
        deckController.SelectedCard = null;
      }
    }
  }

  private void SpawnUnit(UnitType ut, Vector3 pos, bool isLeft)
  {
    // Every unit is represented visually by a gameobject
    GameObject spawned = Instantiate(unitController.Prefabs[ut]);
    // and has a health bar above it
    GameObject health = Instantiate(healthBar, spawned.transform, false);
    
    // Internally, it needs a "Unit" object 
    Unit unitDef = unitController.Behaviors[ut](isLeft, pos);
    units.Add(unitDef); // Which we will keep track of in this list
   
    // And to connect internal definitions to the unity gameobjects, use a "UnitBehavior" object
    UnitBehavior u = spawned.GetComponent<UnitBehavior>();
    u.unit = unitDef;
    u.healthBar = health;
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
          P_PlaceUnit p = new P_PlaceUnit();
          p.Deserialize(packet);
          
          SpawnUnit(p.unitType, new Vector3(p.x,p.y,p.z), false);
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
