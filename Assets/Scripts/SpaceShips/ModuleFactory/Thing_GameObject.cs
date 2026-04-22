using System;
using System.Collections.Generic;
using ModuleSpaceShip.Runtime;
using UnityEngine;

[CreateAssetMenu(fileName = "Thing_GameObject", menuName = "Scriptable Objects/Thing_GameObject")]
public class Thing_GameObject : ScriptableObject
{
    public GameObject HullPrefab;
    public GameObject TurretPrefab;
}
