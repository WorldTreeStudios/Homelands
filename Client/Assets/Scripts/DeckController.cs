using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
{
  public UnitType SelectedUnit { get; set; }
  public GameObject SelectedCard { get; set; }

  [SerializeField]
  private RectTransform cardPanel;
  [SerializeField]
  private GameObject cardBase;
  public void Start()
  {
    // TODO: Fetch player's deck
    for(int i = 0; i < 4; i++) {
      GameObject curr = Instantiate(cardBase, cardPanel, false);
      Card currData = curr.GetComponent<Card>();
      currData.UnitType = (i%2==0) ? UnitType.Cube : UnitType.Sphere;

      Button currButton = curr.GetComponent<Button>();
      currButton.onClick.AddListener(delegate { Select(currData); }); 
      curr.SetActive(true);
    }
  }

  public void Select(Card c)
  {
    SelectedUnit = c.UnitType;
    SelectedCard = c.gameObject;
  }
  
  public void Update() {}
}
