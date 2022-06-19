using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object is responsible for associating unit definitions to corresponding unity prefabs
public class UnitController : MonoBehaviour
{
  public Dictionary<UnitType, GameObject> prefabs;
  public Dictionary<UnitType, Func<bool, Vector3, Unit>> behaviors;

  [SerializeField]
  private GameObject Cube;
  [SerializeField]
  private GameObject Sphere;

  void Start()
  {
    // TODO: Maybe load these from the resources folder?
    prefabs = new Dictionary<UnitType, GameObject>();
    behaviors = new Dictionary<UnitType, Func<bool, Vector3, Unit>>();

    prefabs.Add(UnitType.Cube, Cube);
    behaviors.Add(UnitType.Cube, (bool isLeft, Vector3 pos) => new U_Cube(isLeft, pos.x, pos.y, pos.z));

    prefabs.Add(UnitType.Sphere, Sphere);
    behaviors.Add(UnitType.Sphere, (bool isLeft, Vector3 pos) => new U_Sphere(isLeft, pos.x, pos.y, pos.z));
  }

  void Update() {}
}
