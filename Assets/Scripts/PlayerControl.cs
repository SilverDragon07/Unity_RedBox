using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Animator playerAnim;            //プレイヤーのアニメーション
    AudioSource playerAudiosource;          //プレイヤーのAudioSource
    [SerializeField] AudioClip playerFootstepSE;            //プレイヤーの足音のSE
    [SerializeField] GameObject PlayerDeathEff;         //プレイヤーの死亡時のエフェクト

    CameraControl cameraScr;            //カメラのスクリプト
    StageControl stageScr;          //ステージのスクリプト

    int playerAngle = 0;            //プレイヤー向いている向き
    int playerPosX = 1;         //プレイヤーのステージの中でのX座標
    int playerPosZ = 1;         //プレイヤーのステージの中でのZ座標

    public bool AbleToControl{get; set;}            //操作を受け付けるか
    public bool itemGet{get; set;}          //アイテムを拾ったか
    public bool isFloorMoving{get; set;}            //フロアを移動中か
   

    private void Start()
    {
        cameraScr = GameObject.Find("Main Camera").GetComponent<CameraControl>();           //カメラのスクリプト取得
        stageScr = GameObject.Find("ScriptController").GetComponent<StageControl>();            //ステージのスクリプト取得

        playerAnim = GetComponent<Animator>();          //プレイヤーのAnimatorを取得
        playerAudiosource = GetComponent<AudioSource>();            //プレイヤーのAudioSourceを取得

        AbleToControl = true;           //プレイヤーの操作を受け付ける
    }

    private void LateUpdate()
    {
        if (AbleToControl && !cameraScr.cameraMove)                 //行動出来て、カメラが動いていなければ
        {
            //左回転
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                playerAngle -= 1;           //プレイヤーの向きをセット
                if (playerAngle == -1)          //プレイヤーの向きが-1なら3に
                {
                    playerAngle = 3;
                }

                cameraScr.SetRotateAngle(-90);      //カメラを-90度回転
                playerAnim.Play("Anim_PlayerTurnLeft");         //プレイヤーの左に向くアニメーションを再生

                AbleToControl = false;          //操作を受け付けなくする
            }
            //右回転
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                playerAngle += 1;           //プレイヤーの向きをセット
                if (playerAngle == 4)           //プレイヤーの向きが4なら0に
                {
                    playerAngle = 0;
                }

                cameraScr.SetRotateAngle(90);       //カメラを90度回転
                playerAnim.Play("Anim_PlayerTurnRight");            //プレイヤーの右を向くアニメーションを再生

                AbleToControl = false;          //操作を受け付けなくする
            }
            //前進
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                //プレイヤーの向きに応じた座標に移動する関数呼び出し
                switch (playerAngle)
                {
                    case 0:
                        PlayerMove(playerPosX, playerPosZ + 1);         //Z座標を+1
                        break;

                    case 1:
                        PlayerMove(playerPosX + 1, playerPosZ);         //X座標を+1
                        break;

                    case 2:
                        PlayerMove(playerPosX, playerPosZ - 1);         //Z座標を-1
                        break;

                    case 3:
                        PlayerMove(playerPosX - 1, playerPosZ);         //X座標を-1
                        break;
                }
            }
            //後退
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                //プレイヤーの向きに応じた座標に移動する関数呼び出し
                switch (playerAngle)
                {
                    case 0:
                        PlayerBack(playerPosX, playerPosZ - 1);         //Z座標を-1
                        break;

                    case 1:
                        PlayerBack(playerPosX - 1, playerPosZ);         //X座標を-1
                        break;

                    case 2:
                        PlayerBack(playerPosX, playerPosZ + 1);         //Z座標を+1
                        break;

                    case 3:
                        PlayerBack(playerPosX + 1, playerPosZ);         //X座標を+1
                        break;
                }
            }
        }
    }

    //前進の関数、引数の座標に移動する
    private void PlayerMove(int x, int z)
    {
        if(stageScr.GetObject(x, z) != null)                //移動先の地面があれば
        {
            //上に何が乗っているかの入れ替え
            stageScr.SetCharactor(playerPosX, playerPosZ, "");
            stageScr.SetCharactor(x, z, "Player");
            stageScr.StageDistanceSet();        //全足場のプレイヤーからの距離を再セット

            //プレイヤーの座標を新しい座標に更新
            playerPosX = x;
            playerPosZ = z;

            playerAnim.Play("Anim_PlayerMove");         //プレイヤーの前進アニメーションを再生
            AbleToControl = false;          //操作を受け付けなくする
        }
    }

    //後退の関数、引数の座標に移動する
    private void PlayerBack(int x, int z)
    {
        //移動先の地面があれば
        if (stageScr.GetObject(x, z) != null)
        {
            //上に何が乗っているかの入れ替え
            stageScr.SetCharactor(playerPosX, playerPosZ, "");
            stageScr.SetCharactor(x, z, "Player");
            stageScr.StageDistanceSet();        //全足場のプレイヤーからの距離を再セット

            //プレイヤーの座標を新しい座標に更新
            playerPosX = x;
            playerPosZ = z;

            playerAnim.Play("Anim_PlayerBack");         //プレイヤーの後退アニメーションを再生
            AbleToControl = false;          //操作を受け付けなくする
        }
    }

    //アニメーションが終わった時、アニメーターから呼び出す関数
    public void AnimEnd()
    {
        if (!itemGet)           //アイテムを拾ってなければ
            AbleToControl = true;           //操作を受け付ける
        else            //アイテムを拾っていれば
            PlayerDown();               //プレイヤーが下に降りる  
    }
    //前進後退のアニメーションが終わった時呼び出される関数
    public void AnimStepEnd()
    {
        playerAudiosource.Play();           //足音のSEを再生
    }
    //プレイヤーのフロア移動が終わった時呼び出される関数
    public void AnimDownEnd()
    {
        itemGet = false;            //アイテム取得状態をリセット
        isFloorMoving = false;          //フロア移動中状態をリセット
    }

    //死亡処理
    public void PlayerDeath()
    {
        Instantiate(PlayerDeathEff, transform.position, Quaternion.identity);           //死亡エフェクトを生成
        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true;            //AudioListenerを切り替える

        gameObject.SetActive(false) ;           //プレイヤーを非アクティブに
    }

    //フロア移動
    public void PlayerDown()
    {
        playerAnim.Play("Anim_PlayerDown");         //フロア移動のアニメーションを再生
        isFloorMoving = true;           //フロア移動中状態をセット
    }
}
