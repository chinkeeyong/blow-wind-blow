using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowShip : MonoBehaviour
{

    public GameObject target;

    void Update()
    {
        if (!GamePauser.paused)
        {
            transform.position = target.transform.position;
        }
    }
}
