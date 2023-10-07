using System.Collections;
using UnityEngine;

public class send_demo_01 : MonoBehaviour {

    private IEnumerator Main() {
        yield return new WaitForSeconds(2);  // 2 seconds wait

        int x, y, z;

        mc.PostToChat("Hello, Unity! (demo_01.cs)");

        for (x=0; x<5; x++) {
            for (y=0; y<5; y++) {
                for (z=0; z<5; z++) {
                    mc.SetBlock(x, y, z, param.IRON_BLOCK);
                    // 10 frames wait
                    for (var i = 0; i < 10; i++) {yield return null;}
                }
            }
        }

        for (var i = 0; i <  60; i++) {yield return null;}

        for (x=-5; x<0; x++) {
            for (y=0; y<5; y++) {
                for (z=-5; z<0; z++) {
                    mc.SetBlock(x, y, z, param.GOLD_BLOCK);
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        yield return new WaitForSeconds(2.5f);

        int size = 15;
        string blockTypeId = param.GOLD_BLOCK;
        x = 10;
        y = 0;
        z = 0;
        while (size > 0) {
            mc.SetBlocks(x, y, z, x + size - 1, y, z + size - 1, blockTypeId);
            x += 1;
            z += 1;
            size -= 2;
            y += 1;
            yield return new WaitForSeconds(2.0f);
        }

        mc.PostToChat("good-bye! (demo_01.cs)");
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