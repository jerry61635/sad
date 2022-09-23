using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destory : MonoBehaviour
{
    public float Disable = 3f;

    void Start()
    {
        Destroy(this.gameObject, Disable);
    }


}
