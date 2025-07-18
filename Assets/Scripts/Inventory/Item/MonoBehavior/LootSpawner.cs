using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;
        [Range(0, 1)]
        public float weight;
    }

    public LootItem[] lootItems; // 掉落物品列表

    public void Spawnloot()
    {
        float currentValue = Random.value;
        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)
            {
                // 如果当前随机值小于等于物品的权重，则生成该物品
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position + Vector3.one * 2.5f; // 设置生成位置为当前物体前方的位置
                return; // 生成一个物品后退出
            }
        }
    }
}
