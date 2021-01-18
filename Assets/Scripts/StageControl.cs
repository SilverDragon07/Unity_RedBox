using UnityEngine;

public class StageControl : MonoBehaviour
{
    //足場の構造体
    public struct MapTile
    {
        public int x;               //x座標
        public int z;               //z座標
        public string charactorName;    //上に乗っているオブジェクト名
        public GameObject obj;      //足場のゲームオブジェクト
        public int distFromPlayer;            //プレイヤーからの距離
    }

    public MapTile[,] MapTiles = new MapTile[9, 9];             //9*9のマップを作成

    //このマップの1の場所に足場を生成する
    //マップの端を0以外にしてはいけない
    int[,] drawMap = {  {0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        {0, 1, 1, 1, 1, 1, 1, 1, 0 },
                        {0, 1, 0, 0, 1, 0, 0, 1, 0 },
                        {0, 1, 0, 1, 1, 1, 0, 1, 0 },
                        {0, 1, 1, 1, 1, 1, 1, 1, 0 },
                        {0, 1, 0, 1, 1, 1, 0, 1, 0 },
                        {0, 1, 0, 0, 1, 0, 0, 1, 0 },
                        {0, 1, 1, 1, 1, 1, 1, 1, 0 },
                        {0, 0, 0, 0, 0, 0, 0, 0, 0 }};


    [SerializeField] GameObject ItemPref;            //アイテムのPrefab
    [SerializeField] GameObject TilePref;           //足場のPrefab
    [SerializeField] GameObject TileParent;         //足場の親オブジェクト
    [SerializeField] GameObject UnderTile;          //下に見える見せかけの足場
    [SerializeField] GameObject Ground;         //二フロア以上下を見せない為の地面

    public int nowDepth { get; set; }           //現在のフロア数


    void Awake()
    {
		//足場の生成
		for (int z = 0; z < 9; z++)
        {
            for (int x = 0; x < 9; x++)
            {
                Vector3 tilePos = new Vector3(x * 2, 0, z * 2);         //足場を生成する位置

                if (drawMap[x, z] == 1)         //上のマップで1を書いた所なら
                {
                    MapTiles[x, z].obj = Instantiate(TilePref, tilePos, Quaternion.identity, TileParent.transform);         //足場を生成する
                }

                MapTiles[x, z].x = x;           //生成した足場にx, z座標を入れる
                MapTiles[x, z].z = z;

            }
        }

        MapTiles[1, 1].charactorName = "Player";                    //プレイヤーの初期位置の足場に、プレイヤーがいる事をセット
        StageDistanceSet();                 //全足場のプレイヤーからの距離をセット
        nowDepth = 1;           //現在のフロア数を1に

	}

    //使わない、ステージの端かどうかチェックする
    /*
    public bool StageEdgeCheck(int x, int z)
    {
        if(x == 0 || z == 0)
        {
            return false;
        }
        else if(x == 8 || z == 8)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    */

    //全足場にプレイヤーからの距離をセットする関数
    public void StageDistanceSet()
    {
		//足場が無いタイルに移動距離99をセット
		for (int z = 0; z < MapTiles.GetLength(0); z++)
        {
            for(int x = 0; x < MapTiles.GetLength(1); x++)
            {
                if(MapTiles[x, z].obj == null)
                {
                    MapTiles[x, z].distFromPlayer = 99;
                }
            }
        }

        //プレイヤーのいる位置に距離0をセット、他の移動できるマスに距離-1をセット
        for (int z = 0; z < MapTiles.GetLength(0); z++)
        {
            for (int x = 0; x < MapTiles.GetLength(1); x++)
            {
                if(MapTiles[x, z].charactorName == "Player")
                {
                    MapTiles[x, z].distFromPlayer = 0;
                }
                else if(MapTiles[x, z].distFromPlayer != 99)
                {
                    MapTiles[x, z].distFromPlayer = -1;
                }
            }
        }

        //0から順番に探す距離を増やしていく
        for (int distance = 0; distance < 20; distance++)
        {
            for (int z = 0 + 1; z < MapTiles.GetLength(0) - 1; z++)
            {
                for (int x = 0 + 1; x < MapTiles.GetLength(1) - 1; x++)
                {
                    //探す条件に一致した距離のマスを見つけたら、進行不可のマス以外の、上下左右方向のマスの距離を+1する
                    if (MapTiles[x, z].distFromPlayer == distance)
                    {
                        if (MapTiles[x + 1, z].distFromPlayer != 99 && MapTiles[x + 1, z].distFromPlayer == -1)
                        {
                            MapTiles[x + 1, z].distFromPlayer = distance + 1;
                        }
                        if (MapTiles[x - 1, z].distFromPlayer != 99 && MapTiles[x - 1, z].distFromPlayer == -1)
                        {
                            MapTiles[x - 1, z].distFromPlayer = distance + 1;

                        }
                        if (MapTiles[x, z + 1].distFromPlayer != 99 && MapTiles[x, z + 1].distFromPlayer == -1)
                        {
                            MapTiles[x, z + 1].distFromPlayer = distance + 1;

                        }
                        if (MapTiles[x, z - 1].distFromPlayer != 99 && MapTiles[x, z - 1].distFromPlayer == -1)
                        {
                            MapTiles[x, z - 1].distFromPlayer = distance + 1;

                        }
                    }
                }
            }
        }
    }

    //指定した足場に乗っているキャラクター名を取得
    public string GetCharactor(int x, int z)
    {
        return MapTiles[x, z].charactorName;
    }
    //指定した足場に乗っているキャラクター名をセット
    public void SetCharactor(int x, int z, string name)
    {
        MapTiles[x, z].charactorName = name;
    }

    //指定した足場のオブジェクトを取得
    public GameObject GetObject(int x, int z)
    {
        return MapTiles[x, z].obj;
    }

    //指定した足場のプレーヤーからの距離を取得
    public int GetDistance(int x, int z)
    {
        return MapTiles[x, z].distFromPlayer;
    }

    //フロアの作り直し
    public void ResetStage()
    {
        int playerX = 0;
        int playerZ = 0;

        //マップの足場を全部消す
        for (int z = 0; z < 9; z++)
        {
            for (int x = 0; x < 9; x++)
            {
                if(MapTiles[x, z].charactorName == "Player")            //プレーヤーが乗っている足場を探して保存しておく
                {
                    playerX = x;
                    playerZ = z;
                }

                Destroy(MapTiles[x, z].obj);
            }
        }

        MapTiles = new MapTile[9, 9];                   //まっさらのマップデータで上書きする

        //足場を生成
        for (int z = 0; z < 9; z++)
        {
            for (int x = 0; x < 9; x++)
            {
                Vector3 tilePos = new Vector3(x * 2, -nowDepth * 2, z * 2);         //足場を生成する位置

                if (drawMap[x, z] == 1)         //上のマップで1を書いた所場所なら
                {
                    MapTiles[x, z].obj = Instantiate(TilePref, tilePos, Quaternion.identity, TileParent.transform);         //足場を生成
                }
                if(x == playerX && z == playerZ)            //プレーヤーが乗っていた場所なら
                {
                    MapTiles[x, z].charactorName = "Player";            //プレーヤーが乗っている事をセット
                }

                MapTiles[x, z].x = x;           //生成した足場に座標をセット
                MapTiles[x, z].z = z;
            }
        }

        StageDistanceSet();         //全足場のプレーヤーからの距離をセット

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");          //存在する全ての敵を取得

        //敵を再配置
        foreach(GameObject enemy in enemies)
        {
            while (true)
            {
                //ランダムで座標を出す
                int randX = Random.Range(1, 8);
                int randZ = Random.Range(1, 8);

                //出した座標にプレイヤーが居ない、かつそこに足場があるか
                if (GetCharactor(randX, randZ) != "Player" && GetObject(randX, randZ) != null)
                {
                    if (MapTiles[randX, randZ].distFromPlayer >= 5)         //プレーヤーから5マス以上離れていれば
                    {
                        enemy.GetComponent<EnemyControl>().ResetEnemyAnimator();            //敵のアニメーションをリセット
                        enemy.transform.position = new Vector3(randX * 2, 0.75f - nowDepth * 2, randZ * 2);         //敵を移動
                        enemy.GetComponent<EnemyControl>().SetEnemyPos(randX, randZ);           //敵にその座標をセット
                        enemy.GetComponent<EnemyControl>().SetEnemyDetect(false);           //敵の発見状態をリセット

                        break;
                    }
                }
            }
        }

        UnderTile.transform.position = new Vector3(0, -2 - nowDepth * 2, 0);            //一つ下の足場を動かす
        Ground.transform.position = new Vector3(0, -4 - nowDepth * 2, 0);           //演出用の地面を動かす

        nowDepth += 1;          //今いるフロア数を1増やす
    }
}
