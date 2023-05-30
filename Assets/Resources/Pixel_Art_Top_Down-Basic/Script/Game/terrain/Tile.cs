using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class Tile : MonoBehaviour
{
    public GameObject map;
    public string type = "none"; // none, wall, obstacle, fire, lava, spa, cover point, heart, stage, summon point
    public int X, Y;        //則是記格子的XY值，由座左上為(0, 0)，向右X值增加，向下Y值增加
    public int occupied, state;          //是否被角色佔據， 選擇時 0 是範圍外。 1是可移動位置。 2是攻擊目標。 3是技能目標。
    public Prototype stander;
    protected Vector3 pre_pos;   //這是記點擊瞬間mouse的座標
    protected bool click;
    void Update()
    {
        if (click)
        {
            if ((Input.mousePosition - pre_pos).sqrMagnitude > 500)   //若點擊位置座標與當前位置產生超過定值誤差認定畫面拖曳
            {
                click = false;
            }
        }
    }

    public void set_pos(int x, int y)
    {
        X = x;
        Y = y;
        state = 0;
        map = GameObject.Find("map_frame");
    }


    public System.Func<int> action_select(int act_state, Color act_color)
    {                                //更改顏色，提供動作選擇(取消 0，移動 1，攻擊 2)
        if (type == "summon point")  //summon point不能走，也不會被攻擊，直接返回(同時不具任何阻擋效果，因此不在路徑探索時做處理)
        {
            return end_select;
        }
        gameObject.GetComponent<SpriteRenderer>().color = act_color;
        state = act_state;
        return end_select;
    }


    async void WaitUP()
    {            //點擊後等待mouse up，若其中滑鼠移動則為拖曳畫面，取消點擊任務
        await Task.Run(() =>
       {
           while (click) //只要滑鼠位置不變且保持點擊狀態，則持續監聽
           {
               Thread.Sleep(50);
           }
       }
       );
        gameObject.GetComponent<SpriteRenderer>().material = map.GetComponent<Tiles>().origin;
        if (state == 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);
        }
        else if (state == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0.35f);
        }
        return;
    }
    void OnMouseDown()           //滑鼠按下瞬間進入監聽，設置click為true，調整顏色
    {
        pre_pos = Input.mousePosition;
        gameObject.GetComponent<SpriteRenderer>().material = map.GetComponent<Tiles>().change;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0.7f);
        click = true;
        WaitUP();
        return;
    }

    void OnMouseUp()
    {
        //Tile的父物件是Tiles，管理地圖
        //click確認點擊後取消
        if (click)
        {
            click = false;
            map.GetComponent<Tiles>().click_up(this);
        }
        return;
    }


    int end_select()
    {                                               //回傳給Tiles，待呼叫的還原函式
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);
        state = 0;
        return 1;
    }

    public void clean_stander()  //所在位置角色移動走，清除data
    {
        occupied = 0;
        stander = null;
    }

    public void set_stander(Prototype visitor)
    {
        occupied = 1;
        stander = visitor;
    }
}
