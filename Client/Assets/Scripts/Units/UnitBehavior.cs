using UnityEngine;
using UnityEngine.UI;

public class UnitBehavior : MonoBehaviour
{
  public Unit unit;
  public GameObject healthBar;
  private Slider _healthSlider;

  // Serves as the go-between for the ground truth server definitions, and the unity gameobject data
  void Update()
  {
    if (unit is null || healthBar is null) return;
    _healthSlider ??= healthBar.GetComponentInChildren<Slider>();
    
    if (unit.Health < unit.MaxHealth && !healthBar.activeSelf)
      healthBar.SetActive(true);
    else
      _healthSlider.value = ((float)unit.Health / unit.MaxHealth);
    
    // If dead, die
    if (unit.Health <= 0)
    {
      Main.units.Remove(unit);
      Destroy(gameObject);
    }
   
    // Else, do stuff
    unit.Act(Time.deltaTime, Main.units);
    gameObject.transform.position = new Vector3(unit.position.X, unit.height, unit.position.Y);
  }
}
