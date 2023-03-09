using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager m_ins;
    public static GameManager Ins
    {
        get
        {
            m_ins = FindObjectOfType<GameManager>();
            if (!m_ins)
                m_ins = new GameObject().AddComponent<GameManager>();
            return m_ins;
        }
    }

    public bool spawnAble;
    [SerializeField]
    private Tetromino[] blocks;

    private void Start()
    {
        spawnAble = true;
    }

    private void Update()
    {
        if(spawnAble) SpawnBlock();
    }

    private void SpawnBlock()
    {
        if (blocks.Length > 0)
        {
            int xSpawn = Random.Range(0, blocks.Length);
            Instantiate(blocks[xSpawn], new Vector3(3,16,0),Quaternion.identity);
            spawnAble = false;
        }
    }
}