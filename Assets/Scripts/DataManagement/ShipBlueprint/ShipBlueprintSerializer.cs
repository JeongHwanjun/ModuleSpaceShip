using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ModuleSpaceShip.Defs;
using Unity.VisualScripting;
using UnityEngine;

public static class ShipBlueprintSerializer
{
    public static void SerializeBlueprint(Ship ship)
    {
        // ship을 받아서 ShipGrid의 modules를 직렬화 해야 함
        XElement blueprint = new("blueprint");
        string shipName = "playerShip"; // 아무튼 가져옴
        string shipId = "101010"; // 아무튼 가져왔다고 가정

        blueprint.Add(
            new XElement("shipName", shipName),
            new XElement("shipId", shipId)
        );

        ShipGrid shipGrid = ship.gameObject.GetComponentInChildren<ShipGrid>();
        // shipGrid에서 modules를 가져와 하나씩 읽으면서 XML로 변환하여 작성하면 된다
        XElement moduleBlueprints = new("modules");
        List<GameObject> modules = shipGrid.GetModules();
        foreach(GameObject module in modules)
        {
            moduleBlueprints.Add(
                SerializeModule(module)
            );
        }
        blueprint.Add(moduleBlueprints);

        string blueprintFolderPath = Path.Combine(XMLPathUtilities.DefPath, "Blueprints");
        if(!Directory.Exists(blueprintFolderPath)) Directory.CreateDirectory(blueprintFolderPath);
        blueprint.Save(Path.Combine(blueprintFolderPath, "PlayerShip.xml"));
    }

    private static XElement SerializeModule(GameObject module)
    {
        Module moduleScript = module.GetComponent<Module>();
        // 모듈의 def를 참조해서 XML로 전환
        ModuleDef def = moduleScript.GetDef();
        
        return AddModuleBlueprint(def, moduleScript.GetGridPos());
    }

    private static XElement AddModuleBlueprint(ModuleDef def, GridPos pos)
    { // 아직 완벽하지 않음
        XElement position = new("position");
        position.Add(
            new XElement("x", pos.x),
            new XElement("y", pos.y)
        );
        XElement module = new("module");
        module.Add(
            new XElement("defName", def.defName),
            new XElement("prefabPath", def.prefabPath),
            position
        );

        return module;
    }

    public static void DeserializeBlueprint()
    {
        
    }
}