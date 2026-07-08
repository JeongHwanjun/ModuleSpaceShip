using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        XElement shipBlueprint = new("ship");
        string shipName = ship.shipName;

        shipBlueprint.Add(
            new XElement("shipName", shipName)
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
        shipBlueprint.Add(moduleBlueprints);
        blueprint.Add(shipBlueprint);

        string blueprintFolderPath = XMLPathUtilities.blueprintPath;
        if(!Directory.Exists(blueprintFolderPath)) Directory.CreateDirectory(blueprintFolderPath);
        string blueprintFilePath = Path.Combine(blueprintFolderPath, "Blueprints.xml");
        if(!File.Exists(blueprintFilePath)) File.Create(blueprintFilePath);
        // 이 전에 blueprint 파일을 불러와서 거기의 root에 내용을 추가하는 방식으로 바꿔야 할 듯
        blueprint.Save(Path.Combine(blueprintFolderPath, "Blueprints.xml"));
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

    public static XElement DeserializeBlueprint()
    {
        return DeserializeBlueprintByName("playerShip");
    }

    public static XElement DeserializeBlueprintByName(string targetShipName)
    {
        XDocument blueprintFile = LoadFile();
        XElement blueprint = ReadXML(blueprintFile);
        return DeSerializeShip(blueprint, targetShipName);
    }

    private static XDocument LoadFile()
    {
        XDocument blueprintFile = XDocument.Load(Path.Combine(XMLPathUtilities.blueprintPath, "Blueprints.xml"));
        return blueprintFile;
    }
    private static XElement ReadXML(XDocument file)
    {
        XElement root = file.Root;
        if(root == null || !root.Name.LocalName.Equals("blueprint", System.StringComparison.OrdinalIgnoreCase))
            throw new Exception($"[ShipBlueprintSerializer] Invalid root in '{root.Name.LocalName}'. Expected <blueprint>.");
        return root;
    }

    private static XElement DeSerializeShip(XElement blueprints, string targetShipName)
    {
        List<XElement> ships = blueprints.Elements("ship").ToList();
        // bluprints에서 지정한 shipName을 찾아서 반환함
        foreach(XElement ship in ships)
        {
            XElement shipName = ship.Element("shipName");
            Debug.Log(shipName.Value);
            if(string.Equals(shipName.Value.Trim(), targetShipName)) return ship;
        }

        // 해당 shipName을 가진 blueprint를 찾지 못한 경우
        Debug.LogWarning($"[ShipBlueprintSerializer] Could not find ship : {targetShipName}");
        return null;
    }
}