using Unity.Netcode;
using UnityEngine;

namespace Treemals.Core
{
    // Place one instance of this in the Game scene with a NetworkObject component.
    // Holds all shared team progression state — EXP and level are visible to every client.
    public class TeamProgression : NetworkBehaviour
    {
        public static TeamProgression Instance { get; private set; }

        public NetworkVariable<int> Exp   = new();
        public NetworkVariable<int> Level = new();

        private const int ExpPerLevel = 100;

        public override void OnNetworkSpawn()
        {
            Instance = this;
        }

        public override void OnNetworkDespawn()
        {
            if (Instance == this) Instance = null;
        }

        // Called server-side by EnemyController when an enemy dies.
        public void AddExp(int amount)
        {
            if (!IsServer) return;

            Exp.Value += amount;
            Debug.Log($"[Progression] EXP {Exp.Value}/{ExpPerLevel}  Level {Level.Value}");

            if (Exp.Value >= ExpPerLevel)
            {
                Exp.Value -= ExpPerLevel;
                Level.Value++;
                Debug.Log($"[Progression] Level up → {Level.Value}");
                NotifyLevelUpClientRpc(Level.Value);
            }
        }

        [ClientRpc]
        private void NotifyLevelUpClientRpc(int newLevel)
        {
            Debug.Log($"[Progression] Team reached Level {newLevel}!");
            // Future: trigger upgrade selection UI here.
        }
    }
}
