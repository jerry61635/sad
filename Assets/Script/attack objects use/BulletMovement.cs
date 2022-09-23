using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float _moveSpeed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {

        Destroy(this.gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(new Vector3(transform.rotation.x, transform.rotation.y, transform.position.z)*4f*Time.deltaTime,Space.World);
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }


}
