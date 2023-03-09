using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public bool isGhost;
    private Tetromino ghost;
    [SerializeField] private bool _isShapeO;
    private bool m_isInActive;
    private Vector3 MOVE_RIGHT = Vector3.right,MOVE_LEFT = Vector3.left,MOVE_DOWN = Vector3.down,MOVE_UP = Vector3.up;
    private float m_timeMove,
        m_timeStartMove,
        m_timeDownMove,
        m_timeStartDownMove;
    private const int ROW = 18, COL = 11;
    private int m_beginRowClean, m_amountRowClean;
    private static Transform[,] m_board = new Transform[ROW, COL];
    private bool checkLimit;
    void Start()
    {
        m_timeMove = 0.15f;
        m_timeMove = 0.15f;
        m_timeDownMove = 0.7f;
        m_timeStartDownMove = m_timeDownMove;
        if (isGhost)
        {
            foreach (Transform block in transform)
            {
                Sprite sprite = GameManager.Ins.GhostTetrominoSprite;
                if(sprite) block.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }

            Drop();
        }
        else if(!ghost)
        {
            ghost = Instantiate(this, transform.position, quaternion.identity);
            ghost.isGhost = true;
        }
    }//
    void Update()
    {
        if (m_isInActive) return;
        Move();
        if (Input.GetKeyDown(KeyCode.UpArrow) && !_isShapeO)
            BlockRotate();
    }//

    private void LateUpdate()
    {
        if (isGhost) return;
        if (!m_isInActive && !CheckCanMove())
        {
            if(ghost) Destroy(ghost.gameObject);
            AddBoard();
            CleanRowFull();
            GameManager.Ins.spawnAble = true;
            if (m_amountRowClean > 0)
            {
                m_isInActive = true;
                StartCoroutine(MoveDownToEmpty());
            }
            else enabled = false;
        }
    }

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveWithDirect(MOVE_LEFT);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveWithDirect(MOVE_RIGHT);
        else if (!isGhost && Input.GetKey(KeyCode.DownArrow))
        {
            m_timeStartDownMove = m_timeDownMove;
            m_timeStartMove -= Time.deltaTime;
            if (m_timeStartMove <= 0)
            {
                MoveWithDirect(MOVE_DOWN);
                m_timeStartMove = m_timeMove;
            }
            
        }
        if(!isGhost && !Input.GetKey(KeyCode.DownArrow))
        {
            m_timeStartDownMove -= Time.deltaTime;
            m_timeStartMove = 0;
            if (m_timeStartDownMove <= 0)
            {
                MoveWithDirect(MOVE_DOWN);
                m_timeStartDownMove = m_timeDownMove;
            }
        }
            
    }//

    private void Drop()
    {
        if (!isGhost) return;
        int x;
        int y;
        bool check = true;
        while (!checkLimit)
        {
            foreach (Transform block in transform)
            {
                x = Mathf.RoundToInt(block.position.x);
                y = Mathf.RoundToInt(block.position.y);
                if (y <= 0 || x <= 0 || m_board[y-1, x])
                {
                    check = false;
                    break;
                }
            }

            if (!check) break;
            MoveWithDirect(MOVE_DOWN);
        }
    }//
    
    public void MoveWithDirect(Vector3 dir)
    {
        transform.position += dir;
        if (!CheckPos())
        {
            if (isGhost)
            {
                if (!checkLimit) MoveWithDirect(MOVE_UP);
                else transform.position -= dir;
                return;
            }
            transform.position -= dir;
            
        }
        else Drop();
    }//

    private void BlockRotate()
    {
        transform.Rotate(Vector3.forward,90);
        Drop();
        if (!CheckPos())
        {
            if (isGhost)
            {
                MoveWithDirect(MOVE_UP);
                //if(!checkLimit) transform.Rotate(Vector3.forward,-90);
                return;
            }
            transform.Rotate(Vector3.forward,-90);
        }
    }//

    private bool CheckPos()
    {
        checkLimit = false;
        bool check = true;
        foreach (Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            if (x < 0 || x > 10 || y < 0 || y > 18)
            {
                check = false;
                checkLimit = true;
            }
            else if (m_board[y, x]) check = false;

            if (!check) return false;
        }

        if (isGhost) return CheckEmptyStraightWay();
        return true;
    }//

    private bool CheckEmptyStraightWay()
    {
        foreach (Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            for (int i = y + 1; i < ROW; i++)
            {
                if (m_board[i, x]) return false;
            }
        }
        return true;
    }

    private bool CheckCanMove()
    {
        foreach (Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            if (y >= ROW - 2 && !CheckPos())
            {
                if(ghost) Destroy(ghost.gameObject);
                GameManager.Ins.isGameOver = true;
                enabled = false;
            }
            if (y <= 0) return false;
            if (m_board[y-1, x]) return false;
        }
        return true;
    }

    private void AddBoard()
    {
        foreach (Transform child in transform)
        {
            m_board[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x)] = child;
        }
    }

    private void CleanRowFull()
    {
        m_amountRowClean = 0;
        bool checkFull;
        bool checkEmpty;
        for (int row = 0; row < ROW - 3; row++)
        {
            checkEmpty = true;
            checkFull = true;
            for (int col = 0; col < COL; col++)
            {
                if (!m_board[row, col])
                {
                    checkFull = false;
                    continue;
                }
                checkEmpty = false;
                if (!checkFull) break;
            }
            if (checkEmpty) break;
            if (!checkFull) continue;
            for (int col = 0; col < COL; col++)
            {
                m_board[row, col].gameObject.GetComponent<Block>().Death();
                m_board[row, col] = null;
            }
            if(m_amountRowClean == 0)
                m_beginRowClean = row;
            m_amountRowClean++;
        }
    }

    private IEnumerator MoveDownToEmpty()
    {
        yield return new WaitForSeconds(m_timeMove);
        if (m_amountRowClean > 0)
        {
            MoveDownToEmpty1Row();
            m_amountRowClean--;
            StartCoroutine(MoveDownToEmpty());
        }
        else enabled = false;
    }

    private void MoveDownToEmpty1Row ()
    {
        for (int row = m_beginRowClean + 1; row < ROW - 3; row++)
        {
            for (int col = 0; col < COL; col++)
            {
                if (m_board[row, col])
                {
                    m_board[row, col].transform.position += Vector3.down;
                    m_board[row - 1, col] = m_board[row, col];
                    m_board[row, col] = null;
                }
            }
        }
    }
    
}
