using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;


public class Tiles : MonoBehaviour
{
    public class Link_list<T>
    {                 //Link_list structure，使用泛型，用於保存回傳的Tile還原函數
        public T content;
        public Link_list<T> next;
        public Link_list(T c, Link_list<T> n)
        {
            this.content = c;
            this.next = n;
        }
    }

    // Start is called before the first frame update
    public GameObject[][] tile_array;
    public GameObject map_controller;
    public Material change;  //提供Tile共同讀取的材質(邊框粗細，點擊高量)
    public Material origin;  //提供Tile共同讀取的材質(邊框粗細，初始)
    public Prototype waiter;
    protected int row, column;
    private bool reacted;  //range偵測時設置為false，mouseUP接受Tile設true，地圖還原後後重設false
    private Tile target;
    Link_list<System.Func<int>> R_func;
    Link_list<System.Func<int>> R_func_tail;
    void Awake()
    {
        row = 9;
        column = 11;   //預設地圖大小(此為小圖size)，從map_controller調用參數
        map_controller = GameObject.Find("map");    //取的MAP controller所在物件
        R_func = null;
        R_func_tail = null;
        map_controller.GetComponent<MAP_controller>().Send_tiles_para(ref row, ref column);
        tile_array = new GameObject[row][];        //生成所有tile的物件陣列
        for (int i = 0; i < row; i++)
        {
            tile_array[i] = new GameObject[column];
        }
        int index = 0;
        foreach (Transform tile in GameObject.Find("map_frame").transform)    // 讀取所有tile進入陣列
        {
            tile_array[index / column /*Y*/][index % column /*X*/] = tile.gameObject;
            tile_array[index / column /*Y*/][index % column /*X*/].GetComponent<Tile>().set_pos(index % column, index / column);  //座標是先Y在X
            index += 1;
            //座標是意圖   0  1  2  3  4 ....
            //            11 12 13 14 15 ....
            //            22 23 24 25 26 ....
        }
    }
    void Start()
    {
        gameObject.SetActive(false);
        change = Resources.Load<Material>("Pixel_Art_Top_Down-Basic/Material/高亮點擊");
        origin = Resources.Load<Material>("Pixel_Art_Top_Down-Basic/Material/格子框");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getTile(GameObject character)
    {
        character.GetComponent<Prototype>().set_Tile(tile_array);
    }

    public void click_up(Tile click_target)  //接受Tile回傳確認已經click
    {
        target = click_target;
        reacted = true;
        return;
    }

    //選擇目標的路徑探索
    private void road_seek(int X, int Y, int dis, int type, int fly_or_throw, int belong, int x_now, int y_now, int step, Tile previous)  //fly_or_throw , 0 = 步行， 1 = 飛行, 2 = 非投擲, 3 = 投擲
    {    //角色初始XY座標， 可動/攻擊距離、種類， 移動或攻擊， 行走(路徑探索當前位置)位置， 回傳所需還原函數列表
        if (x_now < column && x_now >= 0 && y_now < row && y_now >= 0)
        {
            Tile current_tile = tile_array[y_now][x_now].GetComponent<Tile>();   //先取得目標Tile
            if ((current_tile.type == "wall" || current_tile.type == "obstacle") && (fly_or_throw % 2) == 0 /*步行或非投擲*/)  //牆或障礙物對步行或非投擲不可通過則直接返回
            {
                return;
            }
            if (current_tile.state == 0 /*表示在範圍外，尚未選入*/)
            {
                if (fly_or_throw < 2 && current_tile.occupied == 0)   //fly_or_throw < 2 表示為移動判定，且格子未被佔領則可通過
                {
                    Link_list<System.Func<int>> temp = new Link_list<System.Func<int>>(current_tile.action_select(1, new Color(255f, 255f, 255f, 0.35f), previous), null);
                    // 1表示行為種類是可移動的格子，後接其相對的淺白色格子色
                    if (R_func == null)
                    {                                                 // 建立還原函數序列，結束時執行        
                        R_func = temp;                                                  //null 表示建立Link list的頭，
                        R_func_tail = R_func;
                    }
                    else
                    {                                                               //後續接在尾巴
                        R_func_tail.next = temp;
                        R_func_tail = temp;
                    }
                }
                else if (fly_or_throw == 0 /*步行怪*/ && current_tile.occupied == 1 && belong != current_tile.stander.which_side()) //fly_or_throw < 2 表示為移動判定，被敵方佔領則步行不可穿越
                {
                    return;
                }
                else if (fly_or_throw >= 2 && current_tile.occupied == 1 && belong != current_tile.stander.which_side()) //fly_or_throw > 2 表示為攻擊判定，且格子被敵方佔領則可攻擊
                {
                    Link_list<System.Func<int>> temp = new Link_list<System.Func<int>>(current_tile.action_select(2, new Color(0.6792f, 0.1249f, 0.1249f, 0.5f), previous), null);
                    // 2表示行為種類是可攻擊的格子，後接其相對的紅色格子色
                    if (R_func == null)
                    {                                                 // 建立還原函數序列，結束時執行        
                        R_func = temp;                                                  //null 表示建立Link list的頭，
                        R_func_tail = R_func;
                    }
                    else
                    {                                                               //後續接在尾巴
                        R_func_tail.next = temp;
                        R_func_tail = temp;
                    }
                    if (fly_or_throw == 2)    //非投擲無法進續向前探路，返回
                    {
                        return;
                    }

                }
            }
            if (step < dis)
            {

                road_seek(X, Y, dis, type, fly_or_throw, belong, x_now - 1, y_now, step + 1, current_tile);
                road_seek(X, Y, dis, type, fly_or_throw, belong, x_now, y_now - 1, step + 1, current_tile);
                road_seek(X, Y, dis, type, fly_or_throw, belong, x_now + 1, y_now, step + 1, current_tile);
                road_seek(X, Y, dis, type, fly_or_throw, belong, x_now, y_now + 1, step + 1, current_tile);
                if ((type % 2) == 1)
                {
                    road_seek(X, Y, dis, type, fly_or_throw, belong, x_now - 1, y_now - 1, step + 1, current_tile);
                    road_seek(X, Y, dis, type, fly_or_throw, belong, x_now - 1, y_now + 1, step + 1, current_tile);
                    road_seek(X, Y, dis, type, fly_or_throw, belong, x_now + 1, y_now - 1, step + 1, current_tile);
                    road_seek(X, Y, dis, type, fly_or_throw, belong, x_now + 1, y_now + 1, step + 1, current_tile);
                }
            }
        }
        return;
    }

    public async void show_range(int X, int Y, int move, int move_type, int fly, int dis, int dis_type, int throwable, int belong, Prototype character)  //顯示角色可以進行動作範圍的函數
    {
        gameObject.SetActive(true);
        waiter = character;
        if (R_func != null)
        {
            while (R_func != null)                        //執行還原函式，還原地圖，並inactive，作用在不同角色行動選擇切換時
            {
                R_func.content();
                R_func = R_func.next;
            }
        }
        reacted = false;
        R_func = null;       //回傳後續還原Tile狀態的函數列
        R_func_tail = null;
        Debug.Log(X + ", " + Y);
        road_seek(X, Y, move, move_type, fly, belong, X, Y, 0, tile_array[Y][X].GetComponent<Tile>()); //fly_or_throw , 0 = 步行， 1 = 飛行, 2 = 非投擲, 3 = 投擲， 同時用來鑑別攻擊或移動
        road_seek(X, Y, dis, dis_type, throwable + 2, belong, X, Y, 0, tile_array[Y][X].GetComponent<Tile>());
        await Task.Run(() =>
        {                     //等待格子點擊的響應
            while (!reacted && waiter == character)   //如果當前等待者(被點擊響應中的角色)不是函數中等待的character，表示被取代了，退出thread並返回
            {
                Thread.Sleep(50);
            }
        }
        );
        if (waiter != character)
        {
            return;
        }
        gameObject.SetActive(false);
        //開始移動
        if (target.state == 1)  //state 0 是範圍外。 1是可移動位置。 2是攻擊目標。 3是技能目標。
        {
            character.current_pos.clean_stander();      //清除原先站的Tile上的角色資料
            character.set_pos(target);                  //角色重設鎖在新Tile
            target.set_stander(character);              //新Tile重設所站角色
        }
        target = null;
        while (R_func != null)                        //執行還原函式，還原地圖，並inactive
        {
            R_func.content();
            R_func = R_func.next;
        }
        reacted = false;
        return;
    }
}
