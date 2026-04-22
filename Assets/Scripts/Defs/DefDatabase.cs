using System;
using System.Collections.Generic;

namespace ModuleSpaceShip.Defs
{
    public static class DefDatabase
    {
        private static readonly Dictionary<Type, Dictionary<string, Def>> _byTypeAndName = new();

        public static void Clear() => _byTypeAndName.Clear();

        public static void Register(Def def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            if (string.IsNullOrWhiteSpace(def.defName))
                throw new ArgumentException($"DefName is empty for {def.GetType().Name}");

            var t = def.GetType();
            if (!_byTypeAndName.TryGetValue(t, out var dict))
            {
                dict = new Dictionary<string, Def>(StringComparer.OrdinalIgnoreCase);
                _byTypeAndName[t] = dict;
            }

            if (dict.ContainsKey(def.defName))
                throw new Exception($"Duplicate defName '{def.defName}' for type {t.Name}");

            dict[def.defName] = def;
        }

        public static T Get<T>(string defName) where T : Def
        {
            var t = typeof(T);
            if (_byTypeAndName.TryGetValue(t, out var dict) &&
                dict.TryGetValue(defName, out var def))
            {
                return (T)def;
            }

            throw new KeyNotFoundException($"Def not found: {t.Name}('{defName}')");
        }

        /// <summary>
        /// 타입을 모를 때(예: ThingFactory에서 defName만 받는 경우) 모든 Def에서 탐색.
        /// 규모가 커지면 (type,defName)으로 받는 걸 권장.
        /// </summary>
        public static Def GetAny(string defName)
        {
            foreach (var pair in _byTypeAndName)
            {
                if (pair.Value.TryGetValue(defName, out var def))
                    return def;
            }
            throw new KeyNotFoundException($"Def not found: any('{defName}')");
        }
    }
}
