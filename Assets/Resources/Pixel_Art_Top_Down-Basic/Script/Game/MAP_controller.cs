using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MAP_controller : MonoBehaviour
{

    // Start is called before the first frame update
    //public GameObject map_object = GameObject.Find("map")
    public GameObject map;
    public Camera_move mycamera;
    public string player1_name, player2_name; //玩家1 與 玩家2名稱
    public int player1_idol_num, player1_deck_num; //玩家1 場上idol數，牌組深度
    public int player2_idol_num, player2_deck_num; //玩家2 場上idol數，牌組深度
    private int turn;
    private Dictionary<string, GameObject> player1_idol = new Dictionary<string, GameObject>() { };
    private Dictionary<string, GameObject> player2_idol = new Dictionary<string, GameObject>() { };
    public int map_num;
    void get_para(ref int map_num)
    {
        GameObject sce_mg = GameObject.Find("Button controller");
        Dictionary<string, object> D = sce_mg.GetComponent<Button_controller>().data;
        map_num = (int)D["0"];
        player1_name = (string)D["1"];
        player2_name = (string)D["2"];
        player1_deck_num = (int)D["3"];
        player2_deck_num = (int)D["4"];
        Debug.Log(map_num + " " + player1_name + " " + player2_name);
        return;
    }
    void Awake()   //map size = 0 為小圖, 1為大圖
    {                              //map num 表示指定特定地圖
        get_para(ref map_num);
        map = GameObject.Find("map_frame");
        mycamera = GameObject.Find("Main Camera").GetComponent<Camera_move>();
    }

    void Start()
    {
        turn = 1;
        player1_idol_num = 0;
        player2_idol_num = 0;
        player1_idol.Add("Kiara", GameObject.Find("Kiara"));
        player1_idol.Add("Ina", GameObject.Find("Ina"));
        player1_idol.Add("Pekora", GameObject.Find("Pekora"));
        player2_idol.Add("Kuroe", GameObject.Find("Kuroe"));
        player2_idol.Add("Suisei", GameObject.Find("Suisei"));
        map.GetComponent<Tiles>().getTile(player1_idol["Kiara"]);
        map.GetComponent<Tiles>().getTile(player1_idol["Ina"]);
        map.GetComponent<Tiles>().getTile(player1_idol["Pekora"]);
        map.GetComponent<Tiles>().getTile(player2_idol["Kuroe"]);
        map.GetComponent<Tiles>().getTile(player2_idol["Suisei"]);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Send_tiles_para(ref int row, ref int coulmn)
    {
        if (map_num == 0)
        {
            row = 9;
            coulmn = 11;
        }
        else if (map_num == 1)
        {
            row = 11;
            coulmn = 11;
        }
        return;
    }

    //公有函數區
    public static int ABS(int a, int b)
    {
        if (a - b >= 0)
        {
            return (a - b);
        }
        else
        {
            return -(a - b);
        }
    }
    //

    //Map controller管理函數
    public void range(int X, int Y, int move, int move_type, int fly, int dis, int dis_type, int throwable, int belong, Prototype character)
    {
        map.GetComponent<Tiles>().show_range(X, Y, move, move_type, fly, dis, dis_type, throwable, belong, character);
    }
    //

}
