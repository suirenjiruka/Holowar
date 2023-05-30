using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_controller : MonoBehaviour    //此程式碼會負責運行所有button所需函數以及處理場景切換的數據傳輸
{
    // Start is called before the first frame update
    public Dictionary<string, object> data;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void game_start()
    {
        SceneManager.LoadScene(1);
        data = new Dictionary<string, object>();//data包含傳遞給遊戲初始的數值，map_num, player1's name, player2's name 
        data.Add("0", 0);
        //para.Add("0",(Random.Range(0, 200000) % 20));
        data.Add("1", "player1");
        data.Add("2", "player2");
        data.Add("3", 15);
        data.Add("4", 20);
    }
}
