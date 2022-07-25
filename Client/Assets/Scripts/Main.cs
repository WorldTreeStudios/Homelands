using System;
using System.Collections.Generic;
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
  
  public static List<Unit> units;
  private float _currentMana = GameDefs.StartingMana;
  
  public void Start()
  {
    // Setup game variables
    units = new List<Unit>();
  }

  public void Update()
  {
    HandleInput();

    // Receive/Parse messages from the server
    HandleMessages();
    
    // Update Mana, UI
    float increase = Time.deltaTime * GameDefs.ManaPerSecond;
    if ((_currentMana + increase) < GameDefs.MaxMana)
      _currentMana += increase; 
    else
      _currentMana = GameDefs.MaxMana;
    discreteMana.value = (float) Math.Truncate(_currentMana) / GameDefs.MaxMana;   
    continuousMana.value = _currentMana / GameDefs.MaxMana;

    foreach (Unit u in units)
    {
      while (u.AttackQueue.Count > 0)
      {
        (Unit t, int d) = u.AttackQueue.Dequeue();
        t.Health -= d;
        if (t.Health <= 0) t.Health = 0;
      }
    }
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
            NetworkController.Send(new P_PlayCard(card.Id, spawnPos.x, spawnPos.y, spawnPos.z));
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
    while (NetworkController.Packets.Count > 0)
    {
      byte[] packet = NetworkController.Packets.Dequeue();
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
}
