using UnityEngine;
using ModuleSpaceShip.Runtime;
using ModuleSpaceShip.Defs;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;

namespace ModuleSpaceShip.Runtime
{
    public class ModuleFactory : MonoBehaviour
    {
        private static ModuleFactory _instance = null;
        public static ModuleFactory instance {
            get
            {
                if(_instance == null) return null;
                return _instance;
            }
        }

        [SerializeField] private readonly Dictionary<string, GameObject> prefabCache;

        void Awake()
        {
            if(_instance == null) _instance = this;
            else Destroy(gameObject);
        }


        public GameObject CreateModuleFromDef(string defName)
        {
            /*
            // BaseMonobehaviour나 Module에서 Init()을 구현한 다음, 여기서 사용해서 GO를 Instantiate한 다음 반환함;;
            ThingBase thing = ThingFactory.CreateFromDefName(DefName);
            // thing의 타입으로 어떤 GO를 Instantiate 할지 결정
            switch (thing)
            {
                case HullThing hull:
                    InstantiateNewModule(GameObjectSO.HullPrefab, thing);
                    break;
            }
            */

            ModuleDef def = DefDatabase.Get<ModuleDef>(defName);
            if(def == null)
            {
                Debug.LogError($"[ModuleFactory] Finding def Failed. defName : {defName}");
                return null;
            }
            GameObject prefab = GetPrefab(def);
            InstantiateNewModule(prefab, def);

            return null;
        }

        private GameObject GetPrefab(ModuleDef def)
        {
            if (string.IsNullOrWhiteSpace(def.prefabPath))
            {
                Debug.LogError($"[ModuleFactory] Prefab path is empty. defName : {def.defName}");
                return null;
            }

            if(prefabCache.TryGetValue(def.prefabPath, out GameObject cachedPrefab)) return cachedPrefab;

            GameObject prefab = Resources.Load<GameObject>(def.prefabPath);
            if(prefab == null)
            {
                Debug.LogError($"[ModuleFactory] Prefab load failed. defName : {def.defName}, path : {def.prefabPath}");
                return null;
            }

            prefabCache.Add(def.prefabPath, prefab);
            return prefab;
        }

        private GameObject InstantiateNewModule(GameObject prefab, ModuleDef def)
        {
            GameObject newModule = Instantiate(prefab);
            Module moduleScript = newModule.GetComponent<Module>();
            ThingBase thing = ThingFactory.CreateFromDef(def);
            if(moduleScript != null)
            {
                moduleScript.Init(thing);
                return newModule;
            }
            Debug.LogError($"[ModuleFactory] Cannot find component 'Module' : {newModule}");
            return null;
        }
    }
}
