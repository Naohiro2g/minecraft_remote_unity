using System.Collections;
using UnityEngine;

public class send_demo_01 : MonoBehaviour {

    public MinecraftRemoteSender mc;
    // Start is called before the first frame update
    void Start() {
        // 同じオブジェクトにアタッチされたTCPServer.csを取得する。
        mc = GetComponent<MinecraftRemoteSender>();

        Invoke("Connect", 1.0f);  // サーバーが立ち上がるまで待つ

    }

    private void Connect() {
        // サーバーに接続を試み、できなかったら終了。
        if (mc.ConnectServer() == -1) {
            print("Couldn't connect to the server, good-bye.");
        } else {
            // Invoke("Main", 2.0f);
            StartCoroutine(Main());
        }
    }

    private IEnumerator Main() {
        yield return new WaitForSeconds(2);  // 2秒間待つ

        int x, y, z;

        // mc.SetBlocks(5, 5, 5, 10, 10, 10, "gold_block");
        mc.PostToChat("Hello, Unity!");


        for (x=0; x<5; x++) {
            for (y=0; y<5; y++) {
                for (z=0; z<5; z++) {
                    mc.SetBlock(x, y, z, "gold_block");
                    // 10フレーム待つ
                    for (var i = 0; i < 10; i++) {yield return null;}
                }
            }
        }

        for (var i = 0; i < 120; i++) {yield return null;}

        for (x=-5; x<0; x++) {
            for (y=0; y<5; y++) {
                for (z=-5; z<0; z++) {
                    mc.SetBlock(x, y, z, "iron_block");
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }


        mc.SetBlocks(5, 5, -5, 10, 10, -10, "sea_lantern");

        yield return new WaitForSeconds(2.5f);  // 2.5秒間待つ

        int size = 19;
        string blockTypeId = "gold_block";
        x = 10;
        y = 0;
        z = 0;
        while (size > 0) {
            mc.SetBlocks(x, y, z, x + size - 1, y, z + size - 1, blockTypeId);
            x += 1;
            z += 1;
            size -= 2;
            y += 1;
            for (var i = 0; i < 10; i++) {yield return null;}
        }

        mc.PostToChat("good-bye!");
    }

    // Update is called once per frame
    // void Update()
    // {
    // }
}
