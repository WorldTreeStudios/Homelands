using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object is responsible for associating unit definitions to corresponding unity prefabs
public class UnitController : MonoBehaviour
{
  public Dictionary<UnitType, GameObject> Prefabs { get; set; }
  public Dictionary<UnitType, Func<bool, Vector3, Unit>> Behaviors { get; set; }

  [SerializeField]
  private GameObject cube;
  [SerializeField]
  private GameObject sphere;

  void Start()
  {
    // TODO: Maybe load these from the resources folder?
    Prefabs = new Dictionary<UnitType, GameObject>();
    Behaviors =new Dictionary<UnitType, Func<bool, Vector3, Unit>>();

    Prefabs.Add(UnitType.Cube, cube);
    Behaviors.Add(UnitType.Cube, (bool isLeft, Vector3 pos) => new U_Cube(isLeft, pos.x, pos.y, pos.z));

    Prefabs.Add(UnitType.Sphere, sphere);
    Behaviors.Add(UnitType.Sphere, (bool isLeft, Vector3 pos) => new U_Sphere(isLeft, pos.x, pos.y, pos.z));
  }

  void Update() {}
}
