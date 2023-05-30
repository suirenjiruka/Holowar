using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;


public class Camera_move : MonoBehaviour
{
    public enum camera_mode
    {
        stop, moving
    }
    private Transform camera_transform;
    private camera_mode mode;

    private Vector3 pre_pos;
    public float scale_rate;
    public float max_scale;
    public float min_scale;
    public float move_speed;
    public float move_max;


    // Start is called before the first frame update
    void Start()
    {
        camera_transform = transform;
        mode = camera_mode.moving;    //目前未用
        pre_pos = Input.mousePosition;
        move_speed = -0.003f;  //相機拖動速率(將mouse移動反向化為畫面移動，mouse移動數值很大，故該比率極小)
        scale_rate = -5f;       //相機縮放速率
        max_scale = 30f;       //相機縮小極限
        min_scale = 15f;       //相機放大極限
        move_max = 1.5f;         //畫面拖動極限
        Camera.main.orthographicSize = max_scale;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //拖動指令
        {
            pre_pos = Input.mousePosition;  //mouse按下瞬間紀錄mouse位置
        }
        if (Input.GetMouseButton(0))
        {
            move_fun();                     //進入拖動指令
            pre_pos = Input.mousePosition;  //刷新mouse位置
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)   //縮放指令
        {
            scale_fun();
        }
    }

    private void move_fun()
    {
        if (mode == camera_mode.moving)
        {
            Vector3 move_range = (Input.mousePosition - pre_pos) * move_speed; //移動量計算
            Vector3 newpos = camera_transform.localPosition + move_range;      //移動量加上就座標的新座標

            if (newpos[0] >= move_max)                                                             //刷新座標
            {
                newpos[0] = move_max;
            }
            else if (newpos[0] <= -move_max)
            {
                newpos[0] = -move_max;
            }
            if (newpos[1] >= move_max)                                                             //刷新座標
            {
                newpos[1] = move_max;
            }
            else if (newpos[1] <= -move_max)
            {
                newpos[1] = -move_max;
            }
            camera_transform.localPosition = newpos;
        }
        return;
    }

    private void scale_fun()  //拖動是調整相機Z軸
    {
        float scale_up = Input.GetAxis("Mouse ScrollWheel") * scale_rate;    //計算Z軸高度改變
        float new_size = Camera.main.orthographicSize + scale_up;  //取得新座標
        if (new_size >= max_scale)                                                             //刷新座標
        {
            new_size = max_scale;
        }
        if (new_size <= min_scale)
        {
            new_size = min_scale;
        }
        Camera.main.orthographicSize = new_size;
        return;
    }

    public void set_stop()
    {
        mode = camera_mode.stop;
    }
    public void set_moving()
    {
        mode = camera_mode.moving;
    }
}
