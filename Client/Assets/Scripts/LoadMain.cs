using TMPro;
using UnityEngine;

public class LoadMain : MonoBehaviour
{
  [SerializeField]
  private TextMeshProUGUI status;
  public void Start()
  {
    NetworkController.Connect();
    status.text = "Waiting for server...";
  }

  public void Update()
  {
    while (NetworkController.Packets.Count > 0)
    {
      byte[] packet = NetworkController.Packets.Dequeue();
      PacketType packetType = (PacketType)packet[0];
      if (packetType == PacketType.Connect)
      {
        status.text = "Waiting for player...";
      }
      else if (packetType == PacketType.GameStart)
      {
        SceneController.LoadBattle();
      }
      else
      {
        // Just put the packet back, this might cause weirdness, but it also shouldn't happen anyway
        NetworkController.Packets.Enqueue(packet);
      }
    }
  }
}
