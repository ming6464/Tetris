using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public void Death()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject,0.5f);
    }
}
