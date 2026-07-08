using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Runtime;
using UnityEditor;
using UnityEngine;

public class ShipBuilder : MonoBehaviour
{
    private static ShipBuilder _instance;
    public static ShipBuilder instance
    {
        get
        {
            if(_instance == null) return null;
            else return _instance;
        }
    }
    [SerializeField] private GameObject playerShip;
    [SerializeField] private GameObject normalShip;
    [SerializeField] private GameObject cameraRig;

    // Events
    public event Action OnPlayerShipBuildEnd;

    void Awake()
    {
        if(_instance == null) _instance = this;
        else Destroy(gameObject);

        Debug.Log($"playerShip path : {AssetDatabase.GetAssetPath(playerShip)}");
    }
    // [SerializeField] private GameObject cameraRig;
    // 제공받은 XElement를 기반으로 Ship을 생성
    // 해당 Ship에 module을 생성해 부착하여 배치함
    public void DeployShip(XElement ship)
    {
        // Ship을 Build한 후 완료된 Ship을 지정된 위치에 배치함
        // '지정된 위치' -> 얘는 나중에 구현함
        GameObject newShip = BuildShip(ship);
        newShip.transform.position = Vector3.zero;
        // playerShip이 처음으로 생성됐을 경우엔 카메라도 같이 생성함
        Instantiate(cameraRig);

        // PlayerShip 생성이라면 이벤트 발생
        OnPlayerShipBuildEnd?.Invoke();
    }

    private GameObject BuildShip(XElement shipXML)
    {
        // Ship을 생성하고, 해당 ship에 Module을 부착해 반환하는 함수
        // 1. Ship을 생성함, 일단은 playerShip을 생성하지만 주후에 어떤 Ship을 생성해야 하는지 결정 과정 필요
        GameObject newShip = DecideShip();
        ShipGrid newShipGrid = newShip.GetComponentInChildren<ShipGrid>();
        List<XElement> modulesXML = shipXML.Element("modules").Elements("module").ToList();

        foreach(XElement moduleXML in modulesXML)
        {
            GameObject newModule = GetNewModule(moduleXML);
            XElement position = moduleXML.Element("position");
            GridPos pos = new(int.Parse(position.Element("x").Value), int.Parse(position.Element("y").Value));
            newShipGrid.TryPlaceModule(pos, newModule);
        }
        return newShip;
    }

    private GameObject GetNewModule(XElement targetXML)
    {
        // 제공받은 XML에 따라 Module을 생성해 초기화 후 반환함
        ModuleFactory moduleFactory = ModuleFactory.instance;
        if (!moduleFactory)
        {
            Debug.LogWarning($"[ShipBuilder] ModuleFactory is not ready");
            return null;
        }

        string targetDefName = targetXML.Element("defName").Value;
        ModuleDef targetDef = (ModuleDef)DefDatabase.GetAny(targetDefName);

        GameObject newModule = moduleFactory.CreateModuleFromDef(targetDef.defName);
        // ModuleFactory에서 알아서 Init까지 돌려서 반환해줌;; 다음 과정은 필요 없음 ㅎㅎ
        /*
        Module newModuleScript = newModule.GetComponent<Module>();
        if (!newModuleScript)
        {
            Debug.LogError($"[ShipBuilder] Invalid Module : {newModule} has no 'Module' Component");
            Destroy(newModule);
            return null;
        }
        newModuleScript.Init(targetDef);
        */

        return newModule;
    }

    private GameObject DecideShip()
    {
        return Instantiate(playerShip, Vector3.zero, Quaternion.identity);
    }

    // --- 디버그 ----
    [ContextMenu("Deploy new 'PlayerShip'")]
    public void DeployPlayerShip()
    {
        XElement newShipXML = ShipBlueprintSerializer.DeserializeBlueprint();
        DeployShip(newShipXML);
    }
}
