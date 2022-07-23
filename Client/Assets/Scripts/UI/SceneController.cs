using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
   public void LoadMainMenu()
   {
      SceneManager.LoadScene("MainMenu");
   }
   
   public void LoadBattle()
   {
      SceneManager.LoadScene("Battle");
   }

   public void LoadDecks()
   {
      // TODO: Make deck builder 
      Debug.Log("Deck builder button pressed!");
      //SceneManager.LoadScene("Decks");
   }
   
   public void LoadStore()
   {
      // TODO: Make store
      Debug.Log("Store button pressed!");
      //SceneManager.LoadScene("Store");
   }
}
