using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    [SerializeField] private bool _isShapeO;
    private bool m_isInActive;
    private float m_timeMove,
        m_timeStartMove,
        m_timeDownMove,
        m_timeStartDownMove,
        m_timePauseCleaning,
        m_timeStartTimePauseCleanning;
    private const int ROW = 18, COL = 11;
    private int m_beginRowClean, m_amountRowClean;
    private static Transform[,] m_board = new Transform[ROW, COL];
    void Start()
    {
        m_timePauseCleaning = 0.2f;
        m_timeMove = 0.2f;
        m_timeDownMove = 0.8f;
        m_timeStartDownMove = m_timeDownMove;
        m_timeStartTimePauseCleanning = m_timePauseCleaning;
    }
    void Update()
    {
        if (m_amountRowClean > 0)
        {
            m_timeStartTimePauseCleanning -= Time.deltaTime;
            if (m_timeStartTimePauseCleanning <= 0)
            {
                m_amountRowClean--;
                m_timeStartTimePauseCleanning = m_timePauseCleaning;
                MoveEmptyRowDown();
            }

            return;
        }
        if (m_isInActive) return;
        Move();
        if (Input.GetKeyDown(KeyCode.UpArrow) && !_isShapeO)
            BlockRotate();
    }

    private void LateUpdate()
    {
        if (!m_isInActive && !CheckCanMove())
        {
            AddBoard();
            CleanRowFull();
            GameManager.Ins.spawnAble = true;
            if (m_amountRowClean > 0) m_isInActive = true;
            else enabled = false;
        }
    }

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveWithDirect(ValuesConst.MOVE_LEFT);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveWithDirect(ValuesConst.MOVE_RIGHT);
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            m_timeStartDownMove = m_timeDownMove;
            m_timeStartMove -= Time.deltaTime;
            if (m_timeStartMove <= 0)
            {
                MoveWithDirect(ValuesConst.MOVE_DOWN);
                m_timeStartMove = m_timeMove;
            }
            
        }
        if(!Input.GetKey(KeyCode.DownArrow))
        {
            m_timeStartDownMove -= Time.deltaTime;
            m_timeStartMove = 0;
            if (m_timeStartDownMove <= 0)
            {
                MoveWithDirect(ValuesConst.MOVE_DOWN);
                m_timeStartDownMove = m_timeDownMove;
            }
        }
            
    }

    public void MoveWithDirect(int state)
    {
        Vector3 dir = Vector3.zero;
        switch (state)
        {
            case ValuesConst.MOVE_DOWN:
                dir = Vector3.down;
                break;
            case ValuesConst.MOVE_LEFT:
                dir = Vector3.left;
                break;
            case ValuesConst.MOVE_RIGHT:
                dir = Vector3.right;
                break;
        }
        transform.position += dir;
        if (!CheckPos()) transform.position -= dir;
    }

    private void BlockRotate()
    {
        transform.Rotate(Vector3.forward,90);
        if(!CheckPos()) transform.Rotate(Vector3.forward,-90);
    }

    private bool CheckPos()
    {
        bool check = true;
        int index = 0;
        foreach (Transform block in transform)
        {
            index++;
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            if (x < 0 || x > 10) check = false;
            else if (m_board[y, x]) check = false;
        }
        if(index == 0) Destroy(gameObject);
        return check;
    }

    private bool CheckCanMove()
    {
        foreach (Transform block in transform)
        {
            int x = Mathf.RoundToInt(block.position.x);
            int y = Mathf.RoundToInt(block.position.y);
            if (y >= ROW - 2 && !CheckPos())
            {
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

    private void MoveEmptyRowDown ()
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
