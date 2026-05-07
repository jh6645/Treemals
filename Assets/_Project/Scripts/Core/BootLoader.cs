using System.Collections;
using UnityEngine;

namespace Treemals.Core
{
    // Attach to a GameObject in the Boot scene.
    // Waits one frame for all systems to initialize, then transitions to MainMenu.
    public class BootLoader : MonoBehaviour
    {
        private IEnumerator Start()
        {
            GameManager.Instance.SetState(GameState.Boot);

            // Placeholder: initialize audio, network check, save load, etc. here as systems are added.
            yield return null;

            Debug.Log("[BootLoader] Boot complete.");
            SceneLoader.LoadMainMenu();
        }
    }
}
