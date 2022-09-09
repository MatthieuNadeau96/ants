using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAnt : MonoBehaviour
{
    public Vector3 offsetP = new Vector3(0, 2f, -1.6f);
    public Quaternion offsetR = Quaternion.Euler(30, 0,0);

    public Transform target;


    void LateUpdate()
    {
        transform.LookAt(target.position);
        transform.position = target.position + offsetP;
        //transform.rotation = target.rotation * offsetR;
    }
}
