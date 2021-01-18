using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    [SerializeField] GameObject GameOverText;           //ゲームオーバーの文字が書いたテキスト
    [SerializeField] Text ScoreText;            //スコア
    [SerializeField] Image EnemySpawnTimeImage;          //敵がスポーンするまでのタイマーの画像
    [SerializeField] GameObject EnemyPref;           //敵のPrefab

    float enemySpawnTime = 15;          //敵がスポーンするまでにかかる時間
    float roundTime = 0;            //一つのフロアにいる時間
    bool enemySpawnTrigger = true;          //そのフロアで敵がスポーンするか

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Main");
        }

        roundTime += Time.deltaTime;            //フロアに居る時間を加算
        EnemySpawnTimeImage.fillAmount = roundTime / enemySpawnTime;            //敵がスポーンするタイマーのアニメーション

        if (roundTime > enemySpawnTime && enemySpawnTrigger)         //フロアに居る時間が敵がスポーンする時間を超える、かつ敵がスポーン出来ると
        {
            Instantiate(EnemyPref, new Vector3(100, 100, 100), Quaternion.identity);            //何も影響がない空に新しい敵をスポーンさせる
            enemySpawnTrigger = false;          //このフロアでこれ以上敵がスポーンしなくなる
        }
    }

    //ゲームオーバー
    public void PlayerDeath()
    {
        GameOverText.SetActive(true);           //ゲームオーバーのテキストを表示

    }

    //敵がスポーンするトリガーとタイマーをリセットする
    public void resetEnemySpawn()
    {
        enemySpawnTrigger = true;           //このフロアで敵がスポーンするようになる
        roundTime = 0;          //このフロアに居る時間をリセット
    }
}
