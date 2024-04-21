using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Enemy;

public class EnemyList : MonoBehaviour
{
    private const int MAXIMUM_LEVEL = 20;
    private const int MINIMUM_ENEMY_SPAWNING_LEVEL = 1;
    private static List<GameObject> enemyList = new List<GameObject>();
    private GameObject[][] enemyListTiered = new GameObject[MAXIMUM_LEVEL][];
    public int MinimumEnemySpawningLevel { get => MINIMUM_ENEMY_SPAWNING_LEVEL; }
    private void Start()
    {
        enemyList.AddRange(Resources.LoadAll<GameObject>("Prefabs/Enemies"));

        for (int i = 0; i < enemyListTiered.Length; i++) // initialize array
        {
            enemyListTiered[i] = new GameObject[10];
        }
        SortLists();
    }
    public GameObject GetRandomEnemy(int difficulty)
    {
        if (difficulty < MINIMUM_ENEMY_SPAWNING_LEVEL) return null; // don't spawn anything if level isn't high enough
        GameObject enemy;
        int n = 0;
        int tier = UnityEngine.Random.Range(MINIMUM_ENEMY_SPAWNING_LEVEL, difficulty);

        while (true)
        {
            n = FindEmptySlot(enemyListTiered[tier]);
            enemy = enemyListTiered[tier][UnityEngine.Random.Range(0, n)];
            if (enemy == null)
            {
                tier--;
                continue;
            }
            return enemy;
        }
    }
    private void SortLists()
    {
        foreach (GameObject obj in enemyList) {
            foreach (IEnemy ie in obj.GetComponents<MonoBehaviour>().OfType<IEnemy>())
            {
                if (ie.Tier < 0) continue; // there are some enemies that have a tier lower than 0. These enemies are not meant to be spanwed naturally.
                // just find one and break the loop
                int freeSlot = FindEmptySlot(enemyListTiered[ie.Tier]);
                enemyListTiered[ie.Tier][freeSlot] = obj;
                break;
            }
        }
    }
    /// <summary>
    /// finds an empty slot in the given variant dimension of a given tier 
    /// </summary>
    /// <param name="array">the tier </param>
    /// <returns>the index of the free slot; -1 if there are no free slots</returns>
    private int FindEmptySlot(GameObject[] array)
    {
        int i = 0;
        foreach (GameObject obj in array)
        {
            if (obj == null) return i;
            i++;
        }
        return -1;
    }
    public GameObject GetPrefab(int difficulty, int random)
    {
        if (random >= enemyListTiered[difficulty].Length)
        {
            random = enemyListTiered[difficulty].Length - 1;
        }
        return enemyListTiered[difficulty][random];
    }
    public int GetTierCount(int tier)
    {
        return FindEmptySlot(enemyListTiered[tier]);
    }
}
