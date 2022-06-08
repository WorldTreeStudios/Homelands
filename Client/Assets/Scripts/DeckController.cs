using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
  public GameObject SelectedUnit;
  public GameObject SelectedCard;

  [SerializeField]
  private RectTransform CardPanel;
  [SerializeField]
  private GameObject CardBase;

  public void Start()
  {
    // TODO: Fetch player's deck
    PrimitiveType[] primTypes = { PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Capsule, PrimitiveType.Cylinder };
    for(int i = 0; i < 4; i++) {
      GameObject curr = Instantiate(CardBase);
      curr.transform.SetParent(CardPanel, false);
      Card currData = curr.GetComponent<Card>();
      currData.DeckController = this;
      currData.Unit = GameObject.CreatePrimitive(primTypes[i]);
      curr.SetActive(true);
    }
  }

  public void Select(Card c)
  {
    SelectedUnit = c.Unit;
    SelectedCard = c.gameObject;
  }
  
  public void Update() {}
}
