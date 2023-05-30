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
    protected Tile[] stack;
    protected int stack_point;
    protected float moving_rate, acc_dis; //移動速率跟紀錄是否動滿一格的累計距離
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
        acc_dis = 0;
        stack = new Tile[3];
    }

    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if ((current_pos.X != X || current_pos.Y != Y))
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
            if (current_pos.Y > Y)
            {
                move_range += new Vector3(0, -moving_rate, 0);
            }
            else if (current_pos.Y < Y)
            {
                move_range += new Vector3(0, moving_rate, 0);
            }
            //////
            gameObject.transform.localPosition += move_range;  //位移動作
            acc_dis += moving_rate;
            if (acc_dis >= 1)
            {
                Debug.Log("stack: " + stack_point + X + Y);
                X = current_pos.X;
                Y = current_pos.Y;
                acc_dis = 0;
                if (stack_point != 0)
                {
                    stack_point -= 1;
                    current_pos = stack[stack_point];
                    stack[stack_point] = null;
                }
            }
        }

    }

    public void set_Tile(GameObject[][] tile_array)
    {
        current_pos = tile_array[Y][X].GetComponent<Tile>();  //這段似乎會比START更早執行??導致current 跟後續XY失調
        current_pos.set_stander(this);
    }

    public void set_pos(Tile target)
    {
        stack_point = 0;
        while (current_pos != target)
        {
            stack[stack_point] = target;
            stack_point += 1;
            target = target.get_pre();
        }
        stack_point -= 1;
        current_pos = stack[stack_point];
    }
    void OnMouseDown()  //點擊後顯示動作範圍，從map_controller呼叫
    {
        Debug.Log("move");
        if (current_pos.X == X && current_pos.Y == Y)
        {  //移動中不能被調用
            Map_controller.GetComponent<MAP_controller>().range(X, Y, move, move_type, fly, dis, dis_type, throwable, belong, this); //調用Map_cintroller內移動範圍函數
        }
    }

    public int which_side()
    {
        return belong;
    }
}
