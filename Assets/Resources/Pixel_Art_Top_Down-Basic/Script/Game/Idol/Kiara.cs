using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiara : Prototype
{
    // Start is called before the first frame update
    void Start()
    {
        X = 7;
        Y = 2;
        HP = 1500;
        ATK = 275;
        dis = 1;
        dis_type = 1;
        throwable = 0;
        move = 2;
        move_type = 0;
        fly = 1;
        belong = 1;
        moving_rate = 0.01f;
    }
}
