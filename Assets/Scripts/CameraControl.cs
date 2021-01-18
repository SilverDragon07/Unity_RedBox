using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] GameObject PlayerObj;          //プレイヤーのオブジェクト
    PlayerControl playerScr;            //プレイヤーのスクリプト

    float nowAngle = 0;         //今向いている角度
    float targetAngle = 0;          //目標の角度
    float rotateSpeed = 0;          //回転する速度

    public Vector3 targetPos;           //プレイヤーの位置

    public bool cameraMove{get; set;}           //カメラが移動するか


    private void Start()
    {
        targetPos = PlayerObj.transform.position;           //プレイヤーの位置を保存する
        playerScr = PlayerObj.GetComponent<PlayerControl>();            //プレイヤーのスクリプト取得

    }


    private void LateUpdate()
    {
        //カメラの左右回転
        if (cameraMove)
        {
            if (nowAngle < targetAngle)         //目標の角度より左を向いていれば
            {
                rotateSpeed = 10;           //回転の速度に10をセット
                nowAngle += 10;         //現在の角度に10を足しておく
            }
            else if (nowAngle > targetAngle)            //目標の角度より右を向いていれば
            {
                rotateSpeed = -10;          //回転の速度に10をセット
                nowAngle += -10;            //現在の角度に-10を足しておく
            }
            else            //目標の角度と同じになれば        
            {
                rotateSpeed = 0;            //回転の速度は0        
                cameraMove = false;         //カメラを動かなくする
            }

            transform.RotateAround(PlayerObj.transform.position, Vector3.up, rotateSpeed);          //カメラをプレイヤー中心に回転する

        }

        //プレイヤーの保存している座標と現在の位置の差を見て、その分を移動させる
        if (!playerScr.isFloorMoving)           //フロア移動中でなければ
        {
            Vector3 diff = PlayerObj.transform.position - targetPos;
            transform.position = transform.position + new Vector3(diff.x, 0, diff.z);           //y軸以外付いていく
        }
        else            //フロア移動中なら
        {
            Vector3 diff = PlayerObj.transform.position - targetPos;
            transform.position = transform.position + new Vector3(diff.x, diff.y, diff.z);          //y軸も付いていく
        }
        targetPos = PlayerObj.transform.position;                   //もう一度プレイヤーの位置を保存する
    }


    //他の関数から、カメラの角度を調整する関数
    public void SetRotateAngle(float target)
    {
        targetAngle += target;
        cameraMove = true;
    }
}
