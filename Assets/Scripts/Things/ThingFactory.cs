using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModuleSpaceShip.Defs;
using UnityEngine;

namespace ModuleSpaceShip.Runtime
{
    public static class ThingFactory
    {
        // Def => Thing 딕셔너리
        private static Dictionary<Type, Type> _defToThing;

        // 존재하는 Extension을 감지해 Thing type을 결정하는 List
        // 현재는 쓸모 없음
        private static readonly List<(Func<ModuleDef, bool> match, Type thingType)> ModuleResolvers
            = new()
        {
            (def => def is TurretDefBase, typeof(TurretThingBase)),
            (def => true, typeof(HullThing)),
        };

        public static void Warmup() => _ = DefToThingMap;

        private static Dictionary<Type, Type> DefToThingMap
            => _defToThing ??= BuildMapFromGenericBase();

        private static Dictionary<Type, Type> BuildMapFromGenericBase()
        {
            var dict = new Dictionary<Type, Type>();
            var thingBase = typeof(ThingBase);
            var genericBase = typeof(ThingBase<>);
            
            // 모든 Thing
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm =>
                {
                    try { return asm.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(t => t != null && !t.IsAbstract && thingBase.IsAssignableFrom(t));

            foreach (var thingType in allTypes)
            {
                var closedGeneric = FindClosedGenericBase(thingType, genericBase);
                if (closedGeneric == null)
                    continue;

                var defType = closedGeneric.GetGenericArguments()[0];

                if (!typeof(Def).IsAssignableFrom(defType))
                    throw new Exception($"[ThingFactory] {thingType.Name} maps non-Def type '{defType.Name}'.");

                // ModuleDef는 내부 데이터로 분기하므로 자동 매핑에서 제외
                //if (defType == typeof(ModuleDef))
                //    continue;

                if (dict.TryGetValue(defType, out var existing))
                    throw new Exception($"[ThingFactory] Duplicate Thing mapping for Def '{defType.Name}': {existing.Name} and {thingType.Name}");

                dict[defType] = thingType;
            }

            return dict;
        }

        private static Type FindClosedGenericBase(Type type, Type baseGenericDefinition)
        {
            var cur = type;
            while (cur != null && cur != typeof(object))
            {
                if (cur.IsGenericType && cur.GetGenericTypeDefinition() == baseGenericDefinition)
                    return cur;

                cur = cur.BaseType;
            }
            return null;
        }

        public static ThingBase CreateFromDef(Def def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));

            Type thingType = def.thingType;
            if(thingType == null)
                throw new Exception($"{def.defName} ThingType is null");
            if (!typeof(ThingBase).IsAssignableFrom(thingType))
                throw new Exception($"{def.defName} ThingType {thingType.Name} is not a ThingBase");

            var created = (ThingBase)Activator.CreateInstance(thingType);
            created.Init(def);
            return created;
        }

        private static ThingBase CreateModuleThing(ModuleDef def)
        {
            foreach (var (match, thingType) in ModuleResolvers)
            {
                if (match(def))
                    return (ThingBase)Activator.CreateInstance(thingType);
            }

            throw new Exception($"No ModuleThing resolver matched Def '{def.defName}'.");
        }

        public static ThingBase CreateFromDefName(string defName)
        {
            var def = DefDatabase.GetAny(defName);
            return CreateFromDef(def);
        }
    }
}