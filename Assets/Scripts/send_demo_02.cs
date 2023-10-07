using System.Collections;
using UnityEngine;

public class send_demo_02 : MonoBehaviour {
    private IEnumerator Main() {
        yield return new WaitForSeconds(2);  // 2 seconds wait

        mc.PostToChat("this is send_demo_02.cs");

        int w;  // waitFrame
        int x, y, z;
        int i, j, k;

        x = -36;
        z = 4;
        for (i = 0; i < 10; i++) {
            y = 0;
            for (j = 0; j < 15; j++) {
                mc.SetBlock(x, y, z,  param.IRON_BLOCK);
                for (w = 0; w < 10; w++) {yield return null;}
                y += 1;
            }
            x += 2;
        }

        yield return new WaitForSeconds(2.0f);

        x = -25;
        z = 8;
        for (i = 0; i < 10; i++) {
            y = 0;
            for (j = 0; j < 5; j++) {
                mc.SetBlock(x, y, z,  param.GLOWSTONE);
                for (k = 0; k < 30; k++) {yield return null;}
                 y += 1;
            }
            x += 2;
        }

        yield return new WaitForSeconds(1.0f);

        for (x=-10; x<-5; x++) {
            for (y=15; y<20; y++) {
                for (z=0; z<5; z++) {
                    mc.SetBlock(x, y, z, param.STONE);
                    // 10フレーム待つ
                    for (w = 0; w < 10; w++) {yield return null;}
                }
            }
        }

        yield return new WaitForSeconds(2.5f);  // 2.5秒間待つ

        mc.SetBlocks(5, 5, -5, 10, 10, -10, param.SEA_LANTERN);


        mc.PostToChat("good-bye! (send_demo_02.cs)");
    }  // end of Main()
// ======================================================================
    public MinecraftRemoteSender mc;
    // Start is called before the first frame update
    void Start() {
        // 自分がアタッチされているのと違うオブジェクトServerにアタッチされた、TCPServer.csを取得する。
        // 同じオブジェクトの場合は、Findしなくて良いので、
        // mc = GetComponent<MinecraftRemoteSender>();
        mc = GameObject.Find("Server").GetComponent<MinecraftRemoteSender>();

        // サーバーが立ち上がるまで待ってから接続。
        Invoke(nameof(Connect), 1.5f);
    }
    private void Connect() {
        // サーバーに接続を試み、できなかったら終了。
        if (mc.ConnectServer() == -1) {
            print("Couldn't connect to the server, good-bye.");
        } else {
            // Run Main()
            StartCoroutine(Main());
        }
    }
    // Update is called once per frame
    // void Update()
    // {
    // }
}
