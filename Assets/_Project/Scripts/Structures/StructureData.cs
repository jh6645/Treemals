using UnityEngine;

namespace Treemals.Structures
{
    [CreateAssetMenu(fileName = "NewStructureData", menuName = "Treemals/Structure Data")]
    public class StructureData : ScriptableObject
    {
        public string structureName;
        public GameObject prefab;
        public float placementDistance = 1.0f;
    }
}
