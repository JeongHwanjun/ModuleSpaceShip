using UnityEngine;
using ModuleSpaceShip.Runtime;
using ModuleSpaceShip.Defs;
using Unity.VisualScripting;

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

        [SerializeField] private Thing_GameObject GameObjectSO;

        void Awake()
        {
            if(_instance == null) _instance = this;
            else Destroy(gameObject);
        }


        public GameObject CreateModuleFromDef(string DefName)
        {
            // BaseMonobehaviour나 Module에서 Init()을 구현한 다음, 여기서 사용해서 GO를 Instantiate한 다음 반환함;;
            ThingBase thing = ThingFactory.CreateFromDefName(DefName);
            // thing의 타입으로 어떤 GO를 Instantiate 할지 결정
            switch (thing)
            {
                case HullThing hull:
                    InstantiateNewModule(GameObjectSO.HullPrefab, thing);
                    break;
            }

            return null;
        }

        private GameObject InstantiateNewModule(GameObject prefab, ThingBase thing)
        {
            GameObject newModule = Instantiate(prefab);
            Module moduleScript = newModule.GetComponent<Module>();
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
