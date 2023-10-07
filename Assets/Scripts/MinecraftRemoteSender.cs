// Minecraftリモコンのメッセージ送信処理
// 別スクリプトのC#コードから、このスクリプト内のAPIがコールされる。
// mc.SendBlock(x, y, z, blockId)など。
// マイクラが受け取れる形式のメッセージを組み立て、送信バッファに貯める。
//
// Update()で、送信バッファから1行ずつ取り出して、TCPServer.csのSendMessage()で送信する。

// まだ、送信処理がマルチスレットになっていない。

/*
送信はどうするか。スレッドごとに送信するようにすればよい。現在は、複数接続しても、一番最後の接続のみをつかっている。
受信同様、接続できたときに、（コールバック関数内で）（不要かも）スレッドを立ち上げる。
そして、スレッドごとに受信クラスのインスタンスを生成、送信バッファを持たせる。

デモコードから一行ずつ送信するためには、
デモコードが自分の接続を使って、自分のバッファにメッセージを貯めていく必要がある。
デモから、Start()で受信クラスのインスタンス生成、そのメソッドで接続、Update（）でバッファに貯める、という流れになる。

デモコードのmc.SendBlock()のコードで、mcとしてMinecraftRemoteSenderを直接使うのではなく、接続で生成された受信クラスのインスタンスを使う必要がある。
senderscript.Connect()で、Server内で生成された受信クラスのインスタンスのハンドルをゲット、デモコードのmcとして使う。
これによって、mc.SendBlock()で自分のバッファにメッセージを貯めることができる。Update()で、mc.SendLine()などして、送信バッファから1行ずつ取り出して送信する。


TCPServerのSenderクラス
    送信バッファ（デモコードからは見えなくて良い。）
    初期化メソッド（コンストラクター？）
    接続メソッド
    送信メソッド
    バッファに貯めるメソッド　これをデモコードから呼び出す。

重要！　MinecraftRemote_Unityは、複数チャンネル同時受信ができるが、MinecraftRemoteController modは1チャンネルのみ。
昔のmodは、複数チャンネル受信可能だが。
よって、Unityから複数チャンネルで接続に行くと、最初のしか受け付けられない。
こうなると、MinecraftRemoteController modも改造したいな。。

一旦、複数チャンネル可能にして、Unity-Unityでやっておくか。
MinecraftRemoteController向けの送信は、単一チャンネルになるように注意すればよい。
今までのマイクラPythonと同じだ。




受信
    TCPServer：接続を待って確立。受信リングバッファは全接続で共通。
    TCPServer：（スレッド）受信して、ポート番号をつけ、受信リングバッファに貯める。
    Receiver：（1フレームごとに一行）受信リングバッファから取り出して、コマンド解釈し、
                ブロックデータベースとUnity 世界をチェックしてから、ブロックを置く。
                このブロックを置く部分が複雑になっている。現状は問題なく動いているが。

    受信クラスを作って、各接続に受信リングバッファを持たせれば、ポート番号を付加して貯める必要がなくなる。
    ブロックデータベースも、受信クラスごとに持たせればよい。空間中でブロックが重なるのは、現状も同じ。
    ただし、Receiverの動作部分で、各接続の受信リングバッファをチェックする必要がある。
    受信スレッド内でブロックを置こうとするとロックするので、また、別スレッドが必要。
    ＝＝＝＞　受信バッファは1つでいいか。。

送信
    デモコード：TCPServerの送信クラスをアドレス、ポート番号を指定してインスタンス化。
    TCPServer：インスタンス初期化で接続を試みる。受信リングバッファができる。送信スレッドが立ち上がる。
    デモコード：SetBlock（）など、SenderのAPIを使う。
    Sender：メッセージを整形し、接続ごとの送信リングバッファに貯める。
    Sender：（1フレームごとに一行）送信リングバッファから取り出して、送信する
    複数あるバッファをどうやって処理するか。インスタンスのリスト？

*/


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
