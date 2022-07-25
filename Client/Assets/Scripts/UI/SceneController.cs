using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
   public static void Load()
   {
      SceneManager.LoadScene("Loading");
   }
   public static void LoadMainMenu()
   {
      SceneManager.LoadScene("MainMenu");
   }
   
   public static void LoadBattle()
   {
      SceneManager.LoadScene("Battle");
   }

   public static void LoadDecks()
   {
      // TODO: Make deck builder 
      Debug.Log("Deck builder button pressed!");
      //SceneManager.LoadScene("Decks");
   }
   
   public static void LoadStore()
   {
      // TODO: Make store
      Debug.Log("Store button pressed!");
      //SceneManager.LoadScene("Store");
   }
}
