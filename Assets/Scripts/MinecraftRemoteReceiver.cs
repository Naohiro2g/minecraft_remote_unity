// Minecraftリモコンのメッセージ受信処理
// Minecraftリモコンで流れてくるメッセージとデータベース登録例（Java版1.13以降用）
// "world.setBlock(5,71,10,gold_block)"
// +005_+071_+010:458399:gold_block
// x_y_z:ポート番号:ブロックID
// "world.setBlocks(5,71,10,6,72,11,gold_block)"
// "chat.post(kadai #2  golden column)"

using System;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftRemoteReceiver : MonoBehaviour {
    // public GameObject obj;
    public TCPServer serverscript;
    void Start() {
        // 同じオブジェクトにアタッチされたTCPServer.csを取得する。
        // このハンドルを使って、TCPServerのreceiveBufferを参照できる。
        serverscript = GetComponent<TCPServer>();
    }

    // ブロックのデータベースとして使う配列。+005_-071_+010:458399:gold_blockのようなデータ形式。
    private List <string> blockDb = new();
    // 同時接続による複数のストリームからのデータを記録するが、
    // 1行ごとの処理なので、競合は考えなくても良い。

    private void ReceiveMessage(string line) {
    // コマンドメッセージを1行処理して、コマンドごとの処理へ。
        string cmd = line.Split("(")[0];
        string msg = line.Split("(")[1].Split(")")[0];
        // print(line);
        string portNum = line.Split(":")[1];
        // ポート番号を名前とする、空の親オブジェクトを作る。
        if (cmd.Contains("PortNum")) {
            GameObject emptyGameObject = new(msg);
        }
        if (cmd.Contains("chat.post")) {
            ParseCommand("chat.post", msg, portNum);
        }
        if (cmd.Contains("setBlocks")) {
            ParseCommand("setBlocks", msg, portNum);
        }
        else if (cmd.Contains("setBlock")) {
            ParseCommand("setBlock", msg, portNum);
        }
    }

    // コマンドに続くパラメーターを解析して、各APIへ。
    private void ParseCommand(string cmd, string msg, string portNum) {
        if (cmd == "chat.post") {
            PostToChat(msg);
        }
        else if (cmd == "setBlocks") {
                int x0 = int.Parse(msg.Split(",")[0]);
                int y0 = int.Parse(msg.Split(",")[1]);
                int z0 = int.Parse(msg.Split(",")[2]);
                int x1 = int.Parse(msg.Split(",")[3]);
                int y1 = int.Parse(msg.Split(",")[4]);
                int z1 = int.Parse(msg.Split(",")[5]);
                string blockId = msg.Split(",")[6];
                // print("SetBlocks cmd: (" + x0 + ", " + y0 + ", " + z0 + ") - (" + x1 + ", " + y1 + ", " + z1 + "), " + blockId);
                SetBlocks(x0, y0, z0, x1, y1, z1, blockId, portNum);
        }
        else if (cmd == "setBlock") {
            int x = int.Parse(msg.Split(",")[0]);
            int y = int.Parse(msg.Split(",")[1]);
            int z = int.Parse(msg.Split(",")[2]);
            string blockId = msg.Split(",")[3];
            // print("SetBlock cmd: " + x + ", " + y + ", " + z + ", : " + blockId);
            SetBlock(x, y, z, blockId, portNum);
        }
    }

// API ======== ======== ======== ========
    private void PostToChat(string msg) {
    // チャットにメッセージを表示する。コンソールに表示するだけ。
        print("PostToChat: " + msg);
    }

    private void SetBlocks(int x0, int y0, int z0,
                           int x1, int y1, int z1,
                           string blockId, string portNum) {
    // 指定した直方体領域にblockIdのブロックを置く。範囲は、x0,y0,z0からx1,y1,z1まで。
        if (x0 > x1) {(x1, x0) = (x0, x1);}
        if (y0 > y1) {(y1, y0) = (y0, y1);}
        if (z0 > z1) {(z1, z0) = (z0, z1);}
        for (int x = x0; x < x1 + 1; x++) {
            for (int y = y0; y < y1 + 1; y++) {
                for (int z = z0; z < z1 + 1; z++) {
                    SetBlock(x, y, z, blockId, portNum);
                }
            }
        }
    }
    private void SetBlock(int x, int y, int z, string blockId, string portNum) {
    // ブロック情報としては、座標値:ポート番号:ブロックIDの形式だが、
    // Unity世界では、「ポート番号」の親に、「座標値:ブロックID」の子として配置。
        string coordKey = x.ToString("+000;-000") + "_"
                        + y.ToString("+000;-000") + "_"
                        + z.ToString("+000;-000");
        // その場所に、すでにブロックが置かれているかどうかを調べる。
        string oldBlockId = CheckBlockDb(coordKey, portNum);
        if (blockId == "air") {
            if (oldBlockId == "") {return;}  // 何もないなら何もしないで終わり。
            // すでにブロックがあるならブロック情報から削除、Destroysして終わり。
            RemoveBlockDb(coordKey, portNum, oldBlockId);
            DestroyBlock(coordKey, portNum, oldBlockId);
            return;
        } else if (oldBlockId == blockId) {  // 空気以外
            // 同じブロックなら何もしない。
            return;
        } else {
            if (oldBlockId != "") {
                // 異なるブロックなら、ブロック情報から削除、Destroy。
                RemoveBlockDb(coordKey, portNum, oldBlockId);
                DestroyBlock(coordKey, portNum, oldBlockId);
            }
            // 新しいブロックを置いて、ブロック情報に追記
            AddBlockDb(coordKey, portNum, blockId);
            GameObject newBlock = Instantiate(
                                      (GameObject)Resources.Load(blockId),
                                      new Vector3(x, y, z), Quaternion.identity);
            newBlock.name = coordKey + ":" + blockId;
            // ストリームごとに作った空の親オブジェクトの子どもにする。
            newBlock.transform.parent = GameObject.Find(portNum).transform;
        }
    }
// ======== ======== ======== ======== API
    private void DestroyBlock(string coordKey, string portNum, string blockId) {
    // 親オブジェクトの子どもの中から探してDestroyする。
    // 親オブジェクトの名前＝接続ごとのポート番号。
    // ブロックオブジェクトの名前は、座標値:ブロックIDの形式
        GameObject parent = GameObject.Find(portNum);
        string oldBlockName = coordKey + ":" + blockId;
        try {
            // 親オブジェクトの子どもの中から、oldBlockNameを探してDestroyする。
            GameObject oldBlock = parent.transform.Find(oldBlockName).gameObject;
            Destroy(oldBlock);
        } catch (NullReferenceException) {
            // 見つからない場合は、想定外。例外処理。
            print("DestroyBlock: " + oldBlockName + " not found under " + portNum + " : " + parent.name);
        }
    }
// データベースの管理
    private void AddBlockDb(string coordKey, string portNum, string blockId) {
    // ブロック情報を登録する。ブロック情報は、座標値:ポート番号:ブロックIDの形式。
        // 親オブジェクトの名前＝接続ごとのポート番号。
        blockDb.Add(coordKey + ":" + portNum + ":" + blockId);
    }
    private string CheckBlockDb(string coordKey, string portNum) {
    // その場所に、同じ接続によって、すでにブロックが置かれているかどうか、を調べる。
        string blockId = "";
        // 他の接続（＝ポート番号）で置かれたブロックは無視する。
        // blockIdは無視して、座標値:ポート番号を含むレコードを検索
        int index = blockDb.FindIndex(s => s.Contains(coordKey + ":" + portNum));
        if (index >= 0) {  // 見つかった
            blockId = blockDb[index].Split(":")[2];
        }
        // print("CheckBlockDb: " + coordKey + " : " + blockId + " : " + index.ToString());
        return blockId;
    }
    private void RemoveBlockDb(string coordKey, string portNum, string blockId) {
    // リストの要素削除だが、removeAt()はとても遅いので工夫している。登録順は変化しても良い。
    // 削除したいデータの場所に一番最後のデータを上書き＝移動。
        int index = blockDb.IndexOf(coordKey + ":" + portNum + ":" + blockId);
        if (index >= 0) {  // 見つかった
            blockDb[index] = blockDb[^1];  // 一番最後のデータで上書き。
            blockDb.RemoveAt(blockDb.Count - 1);  // 一番最後のデータを削除。
        }
    }


    // Update is called once per frame
    void Update() {
        // bufferにメッセージがあれば取り出して処理する。2行以上あっても、残りは次のフレームで処理する。
        // bufferは、TCPServer.csのreceiveBufferを参照している。
        if (serverscript.receiveBuffer.Count == 0) {return;}
        else if (serverscript.receiveBuffer.TryDequeue(out string line)) {
            ReceiveMessage(line);
        }
    }
}