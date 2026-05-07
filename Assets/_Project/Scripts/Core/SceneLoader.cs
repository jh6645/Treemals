using UnityEngine;
using UnityEngine.SceneManagement;

namespace Treemals.Core
{
    public static class SceneLoader
    {
        public const string BOOT      = "Boot";
        public const string MAIN_MENU = "MainMenu";
        public const string GAME      = "Game";

        public static void LoadMainMenu()
        {
            GameManager.Instance.SetState(GameState.MainMenu);
            SceneManager.LoadScene(MAIN_MENU);
        }

        public static void LoadGame()
        {
            GameManager.Instance.SetState(GameState.Playing);
            SceneManager.LoadScene(GAME);
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
