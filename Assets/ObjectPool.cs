using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public enum PoolObjects // all possible objects in pools
    {
        Wall,
        PlayerBullet,
        EnemyBullet,
        PhantomBullet,
        Missile,
        Bomb,
        GrapeBullet,
        MiniGrapeBullet,
        BounceBullet,
        MiniStar,
        LichBullet,
    };
    [Serializable]
    public struct PoolObject
    {
        [SerializeField] private int maxSpawn;
        [SerializeField] private GameObject prefab;
        [SerializeField] private PoolObjects poolObjectID;
        public int MaxSpawn { get => maxSpawn; }
        public GameObject Prefab { get => prefab; }
        public PoolObjects PoolObjectID { get => poolObjectID; }
    }
    [SerializeField] private List<PoolObject> poolObjects;
     private Dictionary<PoolObjects, Queue<GameObject>> poolDict = new Dictionary<PoolObjects, Queue<GameObject>>();
    public static ObjectPool instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        foreach (var poolObject in poolObjects)
        {
            Queue<GameObject> newPool = new Queue<GameObject>(); // create a new queue for this object
            for (int i = 0; i < poolObject.MaxSpawn; i++)
            {
                GameObject objInstance = Instantiate(poolObject.Prefab); // instantiate MaxSpawn amount of instances
                objInstance.SetActive(false); // set them inactive
                newPool.Enqueue(objInstance); // enqueue it
            }
            poolDict.Add(poolObject.PoolObjectID, newPool); // finally add the queue to the dictionary
        }
    }
    public GameObject PoolSpawn(PoolObjects obj, Vector2 pos, Quaternion rotation)
    {
        GameObject inst = poolDict[obj].Dequeue();
        inst.SetActive(true);
        inst.transform.position = pos;
        inst.transform.rotation = rotation;
        poolDict[obj].Enqueue(inst);
        return inst;
    }
}
