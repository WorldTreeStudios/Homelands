using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
{
  private LinkedList<GameObject> _deck = new();
  public GameObject SelectedCard { get; set; }

  [SerializeField]
  private RectTransform cardPanel;
  [SerializeField]
  private GameObject cardBase;

  private static Dictionary<CardId, Card> _cards;
  
  public void Start()
  {
    _cards = new Dictionary<CardId, Card>();
    _cards.Add(CardId.Cube, new C_Cube());
    _cards.Add(CardId.Sphere, new C_Sphere());
    
    // TODO: Fetch player's deck
    CardId[] deck = 
    { CardId.Cube, CardId.Cube, CardId.Cube, CardId.Cube, CardId.Sphere, CardId.Sphere, CardId.Sphere, CardId.Sphere };
    
    for(int i = 0; i < deck.Length; i++) {
      GameObject curr = Instantiate(cardBase);
     
      CardBehaviour currData = curr.GetComponent<CardBehaviour>();
      currData.card = _cards[deck[i]];
      
      Image currPic = curr.GetComponent<Image>();
      currPic.color = (i < 4) ? Color.white : Color.red;
      
      Button currButton = curr.GetComponent<Button>();
      currButton.onClick.AddListener(delegate { Select(curr); });
      
      _deck.AddLast(curr);
    }
    
    UpdatePanel();
  }

  private void Select(GameObject c)
  {
    SelectedCard = c;
  }

  public void Play(GameObject c)
  {
    _deck.Remove(c);
    c.SetActive(false);
    c.transform.SetParent(null);
    _deck.AddLast(c);
    
    UpdatePanel();
    
    SelectedCard = null;
  }
  
  private void UpdatePanel()
  {
    int index = 0;
    foreach (GameObject c in _deck.TakeWhile(_ => index < 4))
    {
      if (!c.activeSelf)
      {
        c.transform.SetParent(cardPanel);
        c.SetActive(true);
      }

      index++;
    }
  }
}
