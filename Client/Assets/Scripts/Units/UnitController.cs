using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object is responsible for associating unit definitions to corresponding unity prefabs
public class UnitController : MonoBehaviour
{
  public Dictionary<UnitType, GameObject> prefabs;
  public Dictionary<UnitType, Func<Vector3, Unit>> behaviors;

  [SerializeField]
  private GameObject Cube;

  void Start()
  {
    // TODO: Maybe load these from the resources folder?
    prefabs = new Dictionary<UnitType, GameObject>();
    behaviors = new Dictionary<UnitType, Func<Vector3, Unit>>();

    prefabs.Add(UnitType.Cube, Cube);
    behaviors.Add(UnitType.Cube, (Vector3 pos) => new U_Cube(pos.x, pos.y, pos.z));
  }

  void Update() {}
}
