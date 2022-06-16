using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
  public DeckController DeckController;

  public UnitType UnitType;

  public void Start() {}

  public void Update() {}
  
  public void OnClick()
  {
    DeckController.Select(this);
  }

}
