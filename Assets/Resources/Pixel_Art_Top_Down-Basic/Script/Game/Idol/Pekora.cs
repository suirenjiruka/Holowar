using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pekora : Prototype
{
    // Start is called before the first frame update
    void Start()
    {
        X = 5;
        Y = 5;
        HP = 1275;
        ATK = 300;
        dis = 2;
        dis_type = 0;
        throwable = 1;
        move = 2;
        move_type = 0;
        fly = 0;
        belong = 1;
        moving_rate = 0.01f;
    }

}
