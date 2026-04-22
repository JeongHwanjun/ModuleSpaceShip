using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;

namespace ModuleSpaceShip.Defs
{
    public static class DefLoader
    {
        public static void LoadAllFromStreamingAssets()
        {
            var folderPath = XMLPathUtilities.DefPath;

            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning($"[DefLoader] Folder not found: {folderPath}, creating new one.");
                Directory.CreateDirectory(folderPath);
                return;
            }

            var files = Directory.GetFiles(folderPath, "*.xml", SearchOption.AllDirectories);
            Debug.Log($"[DefLoader] Found {files.Length} xml file(s) in {folderPath}");

            foreach (var file in files)
            {
                // 각 xml 파일마다 읽어옴
                LoadFile(file);
            }
        }

        private static void LoadFile(string fullPath)
        {
            // 경로 확보
            Debug.Log($"[DefLoader] Loading: {fullPath}");
            var doc = XDocument.Load(fullPath);

            var root = doc.Root;
            if (root == null || !root.Name.LocalName.Equals("Defs", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"[DefLoader] Invalid root in '{fullPath}'. Expected <Defs>.");

            foreach (var elem in root.Elements())
            {
                // root는 항상 Defs - 비슷한 물건들의 def를 모두 포함
                // elem은 항상 Def, Class Attribute를 달고 있을 것으로 예상됨
                string className = (string)elem.Attribute("Class");
                if (string.IsNullOrWhiteSpace(className))
                {
                    // Class가 없다면 오류 발생
                    throw new Exception($"[DefLoader] Missing class attribute in '{fullPath}'");
                }

                // Def Class에 맞는 타입 찾기 
                Type defType = FindDefType(className.Trim());
                if(defType == null) throw new Exception($"[DefLoader] Unknown Def type '{className}' in '{fullPath}'.");            

                // def 객체 생성
                var def = (Def)Activator.CreateInstance(defType);
                // 필드 채우기
                FillDefFields(def, elem);
                // DB에 등록
                DefDatabase.Register(def);

                Debug.Log($"[DefLoader] Registered {def}");
            }
        }

        private static Type FindDefType(string className)
        {
            // className이 ModuleSpaceShip.Defs.HullDef같은 풀네임인 경우,
            // HullDef처럼 짧은 경우도 있음. 
            var defBase = typeof(Def);

            // 1) 풀네임이면 바로 시도
                var t = Type.GetType(className, throwOnError: false);
                if (t != null && defBase.IsAssignableFrom(t) && !t.IsAbstract)
                    return t;

                // 2) 현재 도메인의 모든 어셈블리에서 "이름 일치"로 탐색
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type[] types;
                    try { types = asm.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { types = e.Types.Where(x => x != null).ToArray(); }

                    foreach (var type in types)
                    {
                        if (type == null) continue;
                        if (type.IsAbstract) continue;
                        if (!defBase.IsAssignableFrom(type)) continue;

                        Debug.Log($"[DefLoader] Found Type : {type}");
                        Debug.Log($"[DefLoader] className : {className}");

                        // 짧은 이름 매칭
                        if (type.Name.Equals(className, StringComparison.Ordinal))
                            return type;

                        // 풀네임 매칭
                        if (type.FullName != null && type.FullName.Equals(className, StringComparison.Ordinal))
                            return type;
                    }
                }

                return null;
        }

        private static void FillDefFields(Def def, XElement elem)
        {
            // 각 def에 따로 구현된 파서에 xml 태그 넘김
            def.LoadFromXml(elem);
        }

        private static string GetString(XElement elem, string name, string defaultValue = "")
        {
            var child = elem.Element(name);
            if (child == null) return defaultValue;
            return (child.Value ?? "").Trim();
        }

        private static int GetInt(XElement elem, string name, int defaultValue = 0)
        {
            var s = GetString(elem, name, "");
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            return int.Parse(s, CultureInfo.InvariantCulture);
        }

        private static float GetFloat(XElement elem, string name, float defaultValue = 0f)
        {
            var s = GetString(elem, name, "");
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            return float.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
