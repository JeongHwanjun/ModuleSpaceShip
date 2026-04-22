using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public static class DefNameCache
{
    // defName -> Def Type
    private static Dictionary<string, Type> _defTypeByName;
    private static double _lastScanTime;

    public static string[] GetNames(string baseDefClassName = null)
    {
        EnsureCache();

        if (string.IsNullOrWhiteSpace(baseDefClassName))
        {
            return _defTypeByName.Keys
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        var baseType = FindDefType(baseDefClassName.Trim());
        if (baseType == null)
        {
            Debug.LogWarning($"[DefNameCache] Base type not found: {baseDefClassName}");
            return Array.Empty<string>();
        }

        return _defTypeByName
            .Where(kv => baseType.IsAssignableFrom(kv.Value))
            .Select(kv => kv.Key)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void EnsureCache()
    {
        if (_defTypeByName != null && EditorApplication.timeSinceStartup - _lastScanTime < 2.0)
            return;

        Refresh();
    }

    [MenuItem("Tools/Defs/Refresh DefName Cache")]
    public static void Refresh()
    {
        _lastScanTime = EditorApplication.timeSinceStartup;

        var dict = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        var folder = XMLPathUtilities.DefPath;
        if (!Directory.Exists(folder))
        {
            _defTypeByName = dict;
            return;
        }

        var files = Directory.GetFiles(folder, "*.xml", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            try
            {
                var doc = XDocument.Load(file);
                var root = doc.Root;
                if (root == null) continue;

                foreach (var defElem in root.Elements())
                {
                    // 1) defName
                    var defName = defElem.Element("defName")?.Value?.Trim();
                    if (string.IsNullOrEmpty(defName)) continue;

                    // 2) 이 엘리먼트의 Def 타입명 추론
                    // - Class 방식: <Def Class="GunDef"> ...
                    // - 태그 방식:  <GunDef> ...
                    var className = (string)defElem.Attribute("Class");
                    var typeName = !string.IsNullOrWhiteSpace(className)
                        ? className.Trim()
                        : defElem.Name.LocalName.Trim();

                    // 3) 타입 찾기
                    var defType = FindDefType(typeName);
                    if (defType == null)
                    {
                        Debug.LogWarning($"[DefNameCache] Unknown Def type '{typeName}' in {file} (defName={defName})");
                        continue;
                    }

                    // 4) 등록(중복 defName이면 마지막이 덮어쓰게 했음. 원하면 예외로 바꿔도 됨)
                    dict[defName] = defType;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[DefNameCache] Failed to parse '{file}': {e.Message}");
            }
        }

        _defTypeByName = dict;
    }

    private static Type FindDefType(string className)
    {
        // 1) 풀네임이면 바로 시도
        var t = Type.GetType(className, throwOnError: false);
        if (t != null && typeof(ModuleSpaceShip.Defs.Def).IsAssignableFrom(t) && !t.IsAbstract)
            return t;

        // 2) 모든 어셈블리에서 Name/FullName 매칭
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = asm.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types.Where(x => x != null).ToArray(); }

            foreach (var type in types)
            {
                if (type == null || type.IsAbstract) continue;
                if (!typeof(ModuleSpaceShip.Defs.Def).IsAssignableFrom(type)) continue;

                if (type.Name == className) return type;
                if (type.FullName != null && type.FullName == className) return type;
            }
        }

        return null;
    }
}
