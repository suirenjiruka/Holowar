using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class summon_list : MonoBehaviour
{
    // Start is called before the first frame update
    MAP_controller Map_controller;
    Camera_move mycamera;
    GameObject character_list;
    List<int> deck_idol;
    int deck_num, left_max, right_max;
    bool scroll;
    private Vector3 pre_pos;
    void Awake()
    {
        Map_controller = GameObject.Find("map").GetComponent<MAP_controller>();
        mycamera = GameObject.Find("Main Camera").GetComponent<Camera_move>();
        character_list = GameObject.Find("character_list");
        deck_num = Map_controller.player1_deck_num;
        gameObject.transform.localScale = new Vector3(deck_num > 7 ? (360 * deck_num) - 20 : 1260, 270, 0);  //至少至少7個角色，少則補上summon_list的長度
        //暫時不知如何調正square軸心，先將scroll bar尺寸拉成兩倍解決(軸心在中心)
        left_max = 0;   //左側滾動邊界
        right_max = deck_num > 7 ? -40 - 180 * (deck_num - 6) : 40; //右側滾動邊界同理至少保持7個角色的距離
    }
    void Start()
    {
        activate(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (scroll)
        {
            move_fun();                     //進入拖動指令
            pre_pos = Input.mousePosition;  //刷新mouse位置
        }
        if (Input.GetMouseButtonUp(0))    //拖動指令
        {
            scroll = false;
        }
    }
    public void activate(bool on_off)
    {
        gameObject.SetActive(on_off);
        character_list.SetActive(on_off);
    }
    void OnMouseDown()  //點擊後顯示動作範圍，從map_controller呼叫
    {
        mycamera.set_stop();
        scroll = true;
        pre_pos = Input.mousePosition;
    }
    void OnMouseUp()
    {
        mycamera.set_moving();
    }

    private void move_fun()
    {
        Vector3 move_range = (Input.mousePosition - pre_pos) * 1f; //移動量計算
        Vector3 newpos = character_list.transform.localPosition + new Vector3(move_range[0], 0, 0);      //移動量加上就座標的新座標

        if (newpos[0] <= right_max)                                                             //刷新座標
        {
            newpos[0] = right_max;
        }
        else if (newpos[0] >= left_max)
        {
            newpos[0] = left_max;
        }
        character_list.transform.localPosition = newpos;
        return;
    }
}
