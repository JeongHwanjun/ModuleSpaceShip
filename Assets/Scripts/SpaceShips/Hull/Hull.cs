using System;
using System.Collections.Generic;
using System.Linq;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Hull : Module
{
    /* Def 및 컴포넌트 */
    [DefName("HullDef")]
    [SerializeField] private string def;
    protected override string DefName => def;
}
