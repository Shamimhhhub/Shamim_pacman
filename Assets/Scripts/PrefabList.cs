using UnityEngine;

[CreateAssetMenu]
public class PrefabList : ScriptableObject
{
    [SerializeField] private GameObject[] prefabs;

    public GameObject GetPrefab(int id)
    {
        return prefabs[id];
    }
}
