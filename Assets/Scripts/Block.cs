using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private ValuesConst.Type type;

    public void Death()
    {
        GameManager.Ins.RunBlockParticle(transform.position,type);
        GetComponent<Animator>().Play("Death");
    }

    private void DeathDestroy()
    {
        Destroy(gameObject);
    }
}
