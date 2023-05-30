using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class Prototype : MonoBehaviour
{
    // Start is called before the first frame update
    protected int HP, ATK, X, Y, belong, action, LV;
    protected int dis, dis_type, throwable;    //dis_type = 0 表示cross, = 1 表示square, throwable = 0 表示無投擲能力
    protected int move, move_type, fly;  //move_type = 0 表示cross, = 1 表示square, fly = 0 表示步行
    public string skill_1_name;
    public string skill_2_name;
    public GameObject Map_controller;  //連接遊戲的中心管理器
    public Tile current_pos;           //所在格子的物件
    protected int X_move, Y_move, step;
    protected float moving_rate, X_acc, Y_acc;
    int Abs(int a, int b)
    {
        if (a > b)
        {
            return a - b;
        }
        else
        {
            return b - a;
        }
    }

    int max(int a, int b)
    {
        if (a >= b)
        {
            return a;
        }
        else
        {
            return b;
        }
    }

    void Awake()
    {
        Map_controller = GameObject.Find("map"); //取的MAP controller所在物件
        X_move = 0;
        Y_move = 0;
    }

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (X_move == 1 && Y_move == 0)     //X移動模式，先水平移動(所有位移都是先以X軸為先)
        {
            Vector3 move_range = new Vector3(0, 0, 0);
            if (current_pos.X > X)
            {
                move_range += new Vector3(moving_rate, 0, 0);
            }
            else if (current_pos.X < X)
            {
                move_range += new Vector3(-moving_rate, 0, 0);
            }
            ///////
            if (current_pos.Y > Y && move_type == 1)    //square的移動方式會優先斜線移動，因此追加Y軸移動
            {
                move_range += new Vector3(0, -moving_rate, 0);
            }
            else if (current_pos.Y < Y && move_type == 1)
            {
                move_range += new Vector3(0, moving_rate, 0);
            }
            //////
            gameObject.transform.localPosition += move_range;  //位移動作
            X_acc += moving_rate;
        }
        if (X_move == 0 && Y_move == 1)               //Y軸移動模式
        {
            Vector3 move_range = new Vector3(0, 0, 0);
            if (current_pos.Y > Y)
            {
                move_range += new Vector3(0, -moving_rate, 0);
            }
            else if (current_pos.Y < Y)
            {
                move_range += new Vector3(0, moving_rate, 0);
            }
            ///////
            if (current_pos.X > X && move_type == 1)            //square的移動方式會優先斜線移動，同理追加X軸移動
            {
                move_range += new Vector3(moving_rate, 0, 0);
            }
            else if (current_pos.X < X && move_type == 1)
            {
                move_range += new Vector3(-moving_rate, 0, 0);
            }
            ////////
            gameObject.transform.localPosition += move_range;
            Y_acc += moving_rate;
        }
        if (X_acc >= 1)      //X動滿一格開始下一格的重置
        {
            X_acc = 0;
            Y_acc = 0;
            if (current_pos.Y != Y)    //若接下來也有Y軸移動，轉為Y軸移動模式(X -> Y -> X)的交錯方式
            {
                Y_move = 1;
                X_move = 0;
            }
            ////計算新位置/////
            if (current_pos.X > X)
            {
                X += 1;
            }
            else if (current_pos.X < X)
            {
                X -= 1;
            }
            if (current_pos.Y > Y && move_type == 1)
            {
                Y += 1;
            }
            else if (current_pos.Y < Y && move_type == 1)
            {
                Y -= 1;
            }
            ////////
            step -= 1;        //消耗一步
        }
        if (Y_acc >= 1)       //Y動滿一格開始下一格的重置
        {
            X_acc = 0;
            Y_acc = 0;
            if (current_pos.X != X)    //若接下來也有X軸移動，轉為X軸移動模式(X -> Y -> X)的交錯方式
            {
                Y_move = 0;
                X_move = 1;
            }
            ////計算新位置/////
            if (current_pos.Y > Y)
            {
                Y += 1;
            }
            else if (current_pos.Y < Y)
            {
                Y -= 1;
            }
            if (current_pos.X > X && move_type == 1)
            {
                X += 1;
            }
            else if (current_pos.X < X && move_type == 1)
            {
                X -= 1;
            }
            //////
            step -= 1;  //消耗一步
        }
        if (step == 0 && (X_move == 1 || Y_move == 1))
        {
            X_move = 0;
            X_acc = 0;
            Y_move = 0;
            Y_acc = 0;
        }
    }

    public void set_Tile(GameObject[][] tile_array)
    {
        current_pos = tile_array[Y][X].GetComponent<Tile>();
        current_pos.set_stander(this);
    }

    public void set_pos(Tile target)
    {
        current_pos = target;
        if (move_type == 0)   //根據movetype鑑定步數
        {
            // cross藉由兩軸絕對值和
            step = Abs(current_pos.X, X) + Abs(current_pos.Y, Y);
        }
        else if (move_type == 1)
        {
            //square則取兩軸絕對值較大者
            step = max(Abs(current_pos.X, X), Abs(current_pos.Y, Y));
        }
        if (X != current_pos.X)    //X有要動先動X，不然則Y
        {
            X_move = 1;
            X_acc = 0;
        }
        else
        {
            Y_move = 1;
            Y_acc = 0;
        }

    }
    void OnMouseDown()  //點擊後顯示動作範圍，從map_controller呼叫
    {
        Debug.Log("move");
        if (X_move == 0 && Y_move == 0)
        {  //移動中不能被調用
            Map_controller.GetComponent<MAP_controller>().range(X, Y, move, move_type, fly, dis, dis_type, throwable, belong, this); //調用Map_cintroller內移動範圍函數
        }
    }

    public int which_side()
    {
        return belong;
    }
}
