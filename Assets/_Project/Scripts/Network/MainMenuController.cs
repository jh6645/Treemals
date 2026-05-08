using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Treemals.Network
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField ipInputField;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;

        private void Start()
        {
            hostButton.onClick.AddListener(OnHostClicked);
            joinButton.onClick.AddListener(OnJoinClicked);
        }

        private void OnHostClicked()
        {
            NetworkManager.Singleton.StartHost();

            // Host drives scene loading for all connected clients via NGO
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        private void OnJoinClicked()
        {
            string ip = ipInputField.text.Trim();
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, 7777);

            NetworkManager.Singleton.StartClient();
            // Client will be moved to Game scene automatically when host loads it
        }
    }
}
