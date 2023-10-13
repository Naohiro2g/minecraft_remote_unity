// Minecraftリモコンのメッセージ送信処理
// 別スクリプトのC#コードから、このスクリプト内のAPIがコールされる。
// mc.SendBlock(x, y, z, blockId)など。
// マイクラが受け取れる形式のメッセージを組み立て、送信バッファに貯める。
//
// Update()で、送信バッファから1行ずつ取り出して、TCPServer.csのSendMessage()で送信する。

// まだ、送信処理がマルチスレットになっていない。



using UnityEngine;

public class MinecraftRemoteSender : MonoBehaviour {
    public TCPServer serverscript;

    // Start is called before the first frame update
    void Start() {
        // 同じオブジェクトにアタッチされたTCPServer.csを取得する。
        // TCPServerのsendBufferをserverscript.sendBufferとして参照できる。
        // 違うオブジェクト分けるときは、GameObject.Find()する。
        serverscript = GetComponent<TCPServer>();
    }

    public int ConnectServer() {
    // サーバーに接続を試みる。
        return serverscript.ConnectServer();
    }


// API ==================================================
// これらのメソッドは、他のスクリプトから呼び出される。
// コマンドメッセージを組み立てて、送信バッファに貯める。
    public void PostToChat(string msg) {
    // チャットにメッセージを表示する。
        msg = "chat.post(" + msg + ")";
        serverscript.sendBuffer.Enqueue(msg);
    }

    public void SetBlocks(int x0, int y0, int z0,
                          int x1, int y1, int z1,
                          string blockId) {
    // 指定した直方体領域にblockIdのブロックを置く。範囲は、x0,y0,z0からx1,y1,z1まで。
        // ex. setBlock(0,-100,23,43,-2,-4,gold_block)
        if (x0 > x1) {(x1, x0) = (x0, x1);}
        if (y0 > y1) {(y1, y0) = (y0, y1);}
        if (z0 > z1) {(z1, z0) = (z0, z1);}
        for (int x = x0; x < x1 + 1; x++) {
            for (int y = y0; y < y1 + 1; y++) {
                for (int z = z0; z < z1 + 1; z++) {
                    SetBlock(x, y, z, blockId);
                }
            }
        }
    }
    public void SetBlock(int x, int y, int z, string blockId) {
        // ex. setBlock(0,-100,23,gold_block)
        string msg = "setBlock("
                    + x.ToString() + ","
                    + y.ToString() + ","
                    + z.ToString() + ","
                    + blockId + ")";
        // print(msg);
        serverscript.sendBuffer.Enqueue(msg);
    }

// ================================================== API


    // Update is called once per frame
    void Update() {
        // バッファから取り出して、1行ずつ送信する。メッセージは加工済み。
        serverscript.SendMessage();
    }
}
