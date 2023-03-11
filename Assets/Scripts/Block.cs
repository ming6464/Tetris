using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public void Death(ValuesConst.Type type)
    {
        GameManager.Ins.RunBlockParticle(transform.position,type);
        GetComponent<Animator>().Play("Death");
    }

    private void DeathDestroy()
    {
        Destroy(gameObject);
    }
}
