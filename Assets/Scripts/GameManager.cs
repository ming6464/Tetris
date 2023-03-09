using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Sprite GhostTetrominoSprite;
    public bool isGameOver;
    private static GameManager m_ins;
    private Tetromino m_nextTetromino;
    private Vector3 m_posNextTetromino;
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
        m_posNextTetromino = new Vector3(0.5f, 19.5f, 0);
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
        if(m_nextTetromino)Destroy(m_nextTetromino.gameObject);
        m_nextTetromino = Instantiate(tetrominos[nextIndexTetromino],m_posNextTetromino,Quaternion.identity);
        m_nextTetromino.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        m_nextTetromino.enabled = false;
    }
}