using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Networking.Transport;
using UnityEngine;

public class Main : MonoBehaviour
{
  [SerializeField]
  private DeckController DeckController;

  private const int port = 9999;
  private Socket socket;
  private IPEndPoint endPoint;

  public void Start()
  {
    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    IPAddress address = IPAddress.Parse("127.0.0.1");
    endPoint = new IPEndPoint(address, port);
  }

  public void Update()
  {
    HandleInput();

    // Receive/Parse messages from the server
    HandleMessages();
  }

  private void HandleInput()
  {
    if (Input.GetMouseButtonDown(0) && DeckController.SelectedUnit != null)
    {
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray, out hit))
      {

        // Spawn the Unit ...
        GameObject spawnedUnit = Instantiate(DeckController.SelectedUnit);
        // ... at the location of the raycast collision
        float unitHeight = spawnedUnit.GetComponent<MeshRenderer>().bounds.extents.y;
        Vector3 offsetVector = new Vector3(0, unitHeight, 0);
        spawnedUnit.transform.position = hit.point + offsetVector; // TODO: Further validate location
        // ... and send it to the server
        Send(new P_PlaceUnit(1, spawnedUnit.transform.position.x,spawnedUnit.transform.position.z));

        // TODO: Control active cards based on server data
        // Remove the card
        DeckController.SelectedCard.SetActive(false);
        DeckController.SelectedUnit = null;
      }
    }
  }

  private void HandleMessages()
  {
    
  }

  // Send a packet over the current connection
  public void Send(Packet p)
  {
    byte[] data = p.Serialize();
    try
    {
      socket.SendTo(data, endPoint);
    } catch(Exception e)
    {
      Debug.Log(e.ToString());
    }
  }

  public void OnDestroy()
  {
    socket.Close();
  }
}
