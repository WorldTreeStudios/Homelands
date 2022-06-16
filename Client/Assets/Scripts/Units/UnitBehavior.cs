using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitBehavior : MonoBehaviour
{
  public Unit unit;

  void Start()
  {
    
  }

  // Serves as the go-between for the ground truth server definitions, and the unity gameobject data
  void Update()
  {
    unit.Act(Time.deltaTime);
    gameObject.transform.position = new Vector3(unit.x, unit.y, unit.z);
  }
}
