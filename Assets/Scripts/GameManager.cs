using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public bool isGameOver;
    private static GameManager m_ins;
    private Tetromino nextTetromino;
    private Vector3 posNextTetromino;
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
    private Tetromino[] tetrominos;

    private int nextIndexTetromino;

    private void Start()
    {
        spawnAble = true;
        nextIndexTetromino = Random.Range(0, tetrominos.Length);
        posNextTetromino = new Vector3(0.5f, 19.5f, 0);
    }

    private void Update()
    {
        if (isGameOver) return;
        if(spawnAble) SpawnBlock();
    }

    private void SpawnBlock()
    {
        if (tetrominos.Length > 0)
        {
            Instantiate(tetrominos[nextIndexTetromino], new Vector3(5,16,0),Quaternion.identity);
            nextIndexTetromino = Random.Range(0, tetrominos.Length);
            spawnAble = false;
            ShowNextTetromino();
        }
    }

    private void ShowNextTetromino()
    {
        if(nextTetromino)Destroy(nextTetromino.gameObject);
        nextTetromino = Instantiate(tetrominos[nextIndexTetromino],posNextTetromino,Quaternion.identity);
        nextTetromino.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        nextTetromino.enabled = false;
    }
}