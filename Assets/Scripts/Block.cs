using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public void Death()
    {
        GetComponent<ParticleSystem>().Play();
        GetComponent<Animator>().Play("Death");
        Destroy(gameObject,0.4f);
    }
}
