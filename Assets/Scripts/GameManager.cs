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
    [SerializeField] private GameObject blockParticle;
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

    public void RunBlockParticle(Vector3 pos, ValuesConst.Type type)
    {
        Color color;
        switch (type)
        {
            case ValuesConst.Type.I:
                color = new Color(0.643f, 0.353f, 0.863f, 1f);
                break;
            case ValuesConst.Type.J:
                color = new Color(0.858f,0.353f,0.859f);
                break;
            case ValuesConst.Type.L:
                color = new Color(0.918f,0.322f,0.478f);
                break;
            case ValuesConst.Type.O:
                color = new Color(0.933f, 0.510f, 0.643f);
                break;
            case ValuesConst.Type.S:
                color = new Color(0.945f, 0.000f, 0.506f);
                break;
            case ValuesConst.Type.T:
                color = new Color(0.784f, 0.055f, 0.600f);
                break;
            default:
                color = new Color(0.444f, 0.310f, 0.871f);
                break;
        }

        GameObject newPart = Instantiate(blockParticle, pos, quaternion.identity);
        ParticleSystem partComp = newPart.GetComponent<ParticleSystem>();
        partComp.startColor = color;
        Destroy(newPart,partComp.startLifetime);
    }
}