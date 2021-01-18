using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    StageControl stageScr;          //ステージのスクリプト

    [SerializeField] GameObject EnemyLight;         //敵を照らすライト
    Animator EnemyAnim;             //敵のアニメーター
    AudioSource EnemyAudioSource;           //敵のAudioSource
    [SerializeField] AudioClip enemyFootstepSE;         //敵の足音のSE

    [SerializeField]int EnemyPosX = 7;          //敵のX座標
    [SerializeField]int EnemyPosZ = 7;          //敵のZ座標

    int shortestDist = 99;          //移動先の最短距離
    float enemyMoveWait = 0;            //敵の移動までのウェイト
    float enemyMoveTime = 1.5f;         //敵の移動する間隔

    public bool ableToMove { get; set; }        //移動可能か
    public bool playerDitected { get; set; }            //プレイヤーを発見済みです
        

    private void Start()
    {
        stageScr = GameObject.Find("ScriptController").GetComponent<StageControl>();            //ステージのスクリプト取得
        EnemyAnim = GetComponent<Animator>();           //敵のアニメーター取得
        EnemyAudioSource = GetComponent<AudioSource>();         //敵のAudioSource取得

        ableToMove = true;          //敵を移動可能に
    }


    private void Update()
    {
        if (enemyMoveWait >= enemyMoveTime && ableToMove)           //敵のウェイトが溜まったかつ、敵が移動可能なら
        {          
            shortestDist = 99;          //最短経路の距離を初期化

            int xp = stageScr.GetDistance(EnemyPosX + 1, EnemyPosZ);
            int xm = stageScr.GetDistance(EnemyPosX - 1, EnemyPosZ);
            int zp = stageScr.GetDistance(EnemyPosX, EnemyPosZ + 1);
            int zm = stageScr.GetDistance(EnemyPosX, EnemyPosZ - 1);

            //上下左右のマスの距離を見て、最短経路を探す
            if (xp < shortestDist)
            {
                shortestDist = xp;
            }
            if (xm < shortestDist)
            {
                shortestDist = xm;
            }
            if (zp < shortestDist)
            {
                shortestDist = zp;
            }
            if (zm < shortestDist)
            {
                shortestDist = zm;

            }

            //最短経路の距離が3以下なら
            if(shortestDist <= 3)
            {
                 SetEnemyDetect(true);          //発見状態にする
            }
            else
            {
                SetEnemyDetect(false);          //発見状態を解除する
            }


            //プレイヤーが発見されている時
            if (playerDitected)
            {
                //上下左右のマスで最短経路のマスを見つけて移動する
                if (xp == shortestDist)
                {
                    EnemyMove(1, 0);
                }
                else if (xm == shortestDist)
                {
                    EnemyMove(-1, 0);
                }
                else if (zp == shortestDist)
                {
                    EnemyMove(0, 1);
                }
                else if (zm == shortestDist)
                {
                    EnemyMove(0, -1);
                }
            }

            //プレイヤーが発見されていない時
            else
            {
                //ランダムな方向に動く
                while(true)
                {
                    int x = 0;
                    int z = 0;

                    switch(Random.Range(0, 4))
                    {
                        case 0:
                            z = 1;
                            break;
                        case 1:
                            x = 1;
                            break;
                        case 2:
                            z = -1;
                            break;
                        case 3:
                            x = -1;
                            break;
                    }

                    //x方向に進むなら
                    if(x != 0)
                    {
                        if(stageScr.GetObject(EnemyPosX + x, EnemyPosZ) != null)            //移動先に足場があれば
                        {
                            EnemyMove(x, 0);
                            break;
                        }
                    }
                    //z方向に進むなら
                    if(z != 0)
                    {
                        if(stageScr.GetObject(EnemyPosX, EnemyPosZ + z) != null)            //移動先に足場があれば
                        {
                            EnemyMove(0, z);
                            break;
                        }
                    }
                }
            }
            enemyMoveWait = 0;          //敵の動くまでのウェイトをリセット
        }

        enemyMoveWait += Time.deltaTime;            //敵のウェイトを加算

        //デバッグ用、敵を消す
        /*
        if(Input.GetKeyDown(KeyCode.E))
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        */
    }

    //プレイヤーに当たったとき、プレイヤーを殺す関数
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerControl>().PlayerDeath();
            GameObject.Find("ScriptController").GetComponent<UIControl>().PlayerDeath();
        }
    }

    //敵を移動させる関数
    private void EnemyMove(int moveX, int moveZ)
    {
        //X方向
        if(moveX == 1)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);            //回転
            EnemyPosX += 1;
        }
        else if(moveX == -1)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);           //回転
            EnemyPosX += -1;
        }
        //Z方向
        if (moveZ == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);         //回転
            EnemyPosZ += 1;
        }
        else if(moveZ == -1)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);           //回転
            EnemyPosZ += -1;
        }

        EnemyAnim.Play("Anim_EnemyMove");           //敵の移動アニメーション再生

        ableToMove = false;         //敵を移動不可に
    }

    //敵の座標をセットする関数
    public void SetEnemyPos(int x, int z)
    {
        EnemyPosX = x;
        EnemyPosZ = z;
    }

    //アニメーターから呼び出す関数
    public void AnimEnd()
    {
        ableToMove = true;          //敵を移動可能に
    }
    public void EnemyStepEnd()
    {
        EnemyAudioSource.PlayOneShot(enemyFootstepSE);          //敵の足音の再生
    }

    //アニメーターをリセットする関数
    public void ResetEnemyAnimator()
    {
        EnemyAnim.enabled = false;
        EnemyAnim = GetComponent<Animator>();
        EnemyAnim.enabled = true;
        EnemyAnim.Play("Anim_EnemyIdle");
    }

    //プレイヤーの発見状態の変更
    public void SetEnemyDetect(bool b)
    {
        if(b)
        {
            EnemyLight.SetActive(true);         //ライトをつける
            enemyMoveTime = 1f;         //移動を早くする
            playerDitected = true;          
        }
        else
        {
            EnemyLight.SetActive(false);            //ライトを消す
            enemyMoveTime = 1.5f;           //移動を元に戻す
            playerDitected = false;
        }
    }
}
