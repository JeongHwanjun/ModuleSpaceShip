using UnityEngine;
using ModuleSpaceShip.Defs;
using ModuleSpaceShip.Runtime;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace ModuleSpaceShip.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        private void Awake()
        {
            // 시작시 Def 로딩
            DefDatabase.Clear();
            DefLoader.LoadAllFromStreamingAssets();

            // ThingFactory 준비
            // ThingFactory.Warmup();

            SceneManager.LoadScene("MainScene");
        }
    }
}