using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] GameObject ItemGetEffect;          //アイテムを取ったエフェクトのPrefab
    [SerializeField] GameObject ScriptController;           //スクリプトコントローラのオブジェクト

    StageControl stageScr;          //ステージのスクリプト
    UIControl UIScr;            //UIのスクリプト

    int nowDepth = 1;           //現在のフロア数

    private void Start()
    {
        ScriptController = GameObject.Find("ScriptController");         //スクリプトコントローラを探す
        stageScr = ScriptController.GetComponent<StageControl>();           //ステージのスクリプトを取得
        UIScr = ScriptController.GetComponent<UIControl>();         //UIのスクリプトを取得
    }

    //プレイヤーに当たった時の処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(ItemGetEffect, transform.position, Quaternion.identity);            //エフェクトを生成
            stageScr.ResetStage();          //フロアをリセット
            UIScr.resetEnemySpawn();            //敵のスポーンまでの時間をリセット
            
            other.GetComponent<PlayerControl>().itemGet = true;         //プレイヤーの下に降りる為の変数をセット
            
            //アイテムの再配置
            while (true)
            {
                //ランダムで座標を出す
                int randX = Random.Range(1, 8);
                int randZ = Random.Range(1, 8);

                //出した座標にプレイヤーが居ない、かつそこに足場があるか
                if (stageScr.GetCharactor(randX, randZ) != "Player" && stageScr.GetObject(randX, randZ) != null)
                {
                    if (stageScr.GetDistance(randX, randZ) >= 5)
                    {
                        transform.position = new Vector3(randX * 2, 0.75f - nowDepth * 2, randZ * 2);

                        nowDepth += 1;
                        break;
                    }
                }
            }
            
        }
    }

}
