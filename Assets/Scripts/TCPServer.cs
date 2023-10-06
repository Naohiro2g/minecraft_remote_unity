// 複数同時接続可能なTCP Server + Minecraftリモコンのメッセージ処理
// 将来的には、送信機能も実装する。

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;

public class TCPServer : MonoBehaviour {

// 課題は、他のPCから接続できるようにすること。送信機能も実装すること。
    public string serverIpAddress = "127.0.0.1";
    private readonly int serverPortNumber = 14712;
    private TcpListener server;
    // メッセージを1行ごとに貯めるFIFOのリングバッファ。サイズは指定できない。
    public ConcurrentQueue<string> receiveBuffer = new();
    public ConcurrentQueue<string> sendBuffer = new();

    void Start() {
    // 受信ソケット接続準備、待機
        var ip = IPAddress.Parse(serverIpAddress);
        // var ip = IPAddress.Any;
        print("Server is listening at: " + ip + ":" + serverPortNumber);

        server = new TcpListener(ip, serverPortNumber);
        server.Start();  // サーバー開始

        // コールバックを設定して、接続を待つ
        server.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), server);

        // 送信clientをサーバーに接続
        // ConnectServer();
    }

    private void DoAcceptTcpClientCallback(IAsyncResult ar) {
    // コールバック処理。クライアントからの接続を受け付けたら呼ばれる。
        var listener = ar.AsyncState as TcpListener;
        TcpClient client = listener.EndAcceptTcpClient(ar);
        string remoteEndndPoint = client.Client.RemoteEndPoint.ToString();
        print("Remote client: " + remoteEndndPoint);
        // 次の接続を待つ。複数チャンネルの通信を同時にできる。
        listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);

        Task.Factory.StartNew(() => {  // 接続ごとに別スレッドで処理する
            // ネットワークストリームを確立
            var stream = client.GetStream();
            // この接続のポート番号を受信バッファに保存
            string portNum = remoteEndndPoint.Split(":")[1];
            receiveBuffer.Enqueue("PortNum(" + portNum + "):" + portNum);
            var reader = new StreamReader(stream, Encoding.UTF8);
            // 接続が切れるまで
            while (client.Connected) {
                // ストリームが終了するまで
                while (!reader.EndOfStream) {
                    // 一行分の文字列を受け取る
                    var line = reader.ReadLine() + ":" + portNum;
                    // print(line);
                    // 受信バッファに保存
                    receiveBuffer.Enqueue(line);
                    // print("Buffer Count is: " + mBuffer.Count);  // バッファに溜まっている行数
                }
                // このクライアントとは接続終了
                // クライアント側で、正常あるいは異常に切断された場合。
                print("Disconnect: " + client.Client.RemoteEndPoint);
                client.Close();
            }
        });
    }

    TcpClient client;
    NetworkStream stream;
    StreamWriter writer;

    // Senderスクリプトから呼び出される。
    public int ConnectServer() {
        // サーバーに接続する
        client = new TcpClient();
        // client.Connect(ip, port);

        try {
            // client.Connect(IPAddress.Parse(serverIpAddress), 12345);
            client.Connect(IPAddress.Parse(serverIpAddress), serverPortNumber);
        } catch (Exception e) {
            print("Server is not responding. " + e.Message);
            return -1;
        }

        print("サーバーと接続しました。"
                + ((IPEndPoint)client.Client.RemoteEndPoint).Address + ":"
                + ((IPEndPoint)client.Client.RemoteEndPoint).Port + " <--> "
                + ((IPEndPoint)client.Client.LocalEndPoint).Address + ":"
                + ((IPEndPoint)client.Client.LocalEndPoint).Port);
        // 接続したら、ストリームを確立
        stream = client.GetStream();
        writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        return 0;
    }
    public void DisconnectServer() {
        client.Close();
    }
    public int SendMessage() {
        // 送信バッファに溜まっているメッセージを、1行ずつ送信する。
        if (stream != null) {
            if (sendBuffer.TryDequeue(out string line)) {
                writer.WriteLine(line);
            }
            return 0;
        } else {
            // print("no stream to send message to.");
            return -1;
        }
    }


    protected virtual void OnApplicationQuit() {
        if (server != null) { server.Stop(); }  // サーバーを停止
        if (client != null) { client.Close(); }  // クライアントを停止
        CancelInvoke();
    }

    // Update is called once per frame
    // void Update() {
    // }
}