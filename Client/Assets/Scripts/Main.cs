using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
  [SerializeField]
  private DeckController deckController;
  [SerializeField]
  private UnitController unitController;
  [SerializeField]
  private GameObject healthBar;
  [SerializeField]
  private Slider continuousMana;
  [SerializeField] 
  private Slider discreteMana;
  
  private const int Port = 9999;
  private UdpClient _connection;
  private IPEndPoint _endPoint;

  private Queue<byte[]> _packets;

  public static List<Unit> units;
  private const float MaxMana = 10f;
  private const float ManaPerSecond = .5f; // TODO: This is a shared definition, gather these in a file? 
  private float _currentMana;
  
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
    
    // Update Mana, UI
    float increase = Time.deltaTime * ManaPerSecond;
    if ((_currentMana + increase) < MaxMana)
      _currentMana += increase; 
    else
      _currentMana = MaxMana;
    discreteMana.value = (float) Math.Truncate(_currentMana) / MaxMana;   
    continuousMana.value = _currentMana / MaxMana;
  }

  private void HandleInput()
  {
    // The user left clicked with a card selected
    GameObject selected = deckController.SelectedCard;
    if (Input.GetMouseButtonDown(0) && selected != null)
    {
      Card card = selected.GetComponent<CardBehaviour>().card;
      if (_currentMana >= card.ManaCost)
      {
        _currentMana -= card.ManaCost;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
          // Get the appropriate unit type
          foreach (UnitType ut in card.Units)
          {
            UnitType toSpawn = ut;
            
            // ... and the location of the raycast collision
            float unitHeight = unitController.Prefabs[toSpawn].GetComponent<MeshRenderer>().bounds.extents.y;
            Vector3 offsetVector = new Vector3(0, unitHeight, 0);
            Vector3 spawnPos = hit.point + offsetVector;
         
            // ... then spawn the unit
            SpawnUnit(toSpawn, spawnPos, true);
            
            // ... and send it to the server
            Send(new P_PlayCard(card.Id, spawnPos.x, spawnPos.y, spawnPos.z));
          }

          // Last, put the card at the back of our deck
          deckController.Play(selected);
        }
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
        case PacketType.PlayCard:
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
