using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject Bullet;
    public GameObject Gun;
    public float shootCD = 0.2f;




    void Start()
    {
        InvokeRepeating("shoot", 0.1f, shootCD);


    }

    void CreateBullet(Vector3 position, float angle)
    {
        Instantiate(Bullet, position, Quaternion.LookRotation(Gun.transform.forward));


    }
    void shoot()
    {
        CreateBullet(this.gameObject.transform.position, 90);


    }


}
