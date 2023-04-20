using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Events;



public class ServerTCP : MonoBehaviour
{
    public class StateSocket
    {
        public const int BufferSize = 4 * 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public Socket workSocket;
    }

    public int Port;
    public string IP;

    private Socket _listener;
    private Socket _client;

    public enum ServerStatus { Off, Started, Connected }
    private ServerStatus status = ServerStatus.Off;
    public ServerStatus Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }


    public void Start()
    {
        StartServer();
    }

    [ContextMenu("Start")]
    public void StartServer()
    {
        if (_listener != null) return;

        _listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(IPAddress.Parse(IP), Port));
        _listener.Listen(100);
        _listener.BeginAccept(new System.AsyncCallback(AcceptCallback), _listener);

        Status = ServerStatus.Started;
        Debug.Log("server started - begin accept");

    }

    [ContextMenu("test")]
    public void test()
    {
        Debug.Log(CheckConnexion());
    }

    public bool CheckConnexion()
    {
        try
        {
            if (null != _client && _client.Poll(1, SelectMode.SelectRead) && _client.Available == 0)
            {
                Status = ServerStatus.Started;
                return false;
            }
            else return true;
        }
        catch (SocketException)
        {
            Status = ServerStatus.Started;
            return false;
        }
    }

    public void AcceptCallback(System.IAsyncResult result)
    {
        Socket listener = (Socket)result.AsyncState;
        _client = listener.EndAccept(result);
        Debug.Log("server - accept callback");

        StateSocket state = new StateSocket();
        state.workSocket = _client;
        _client.BeginReceive(state.buffer, 0, StateSocket.BufferSize, 0, new System.AsyncCallback(ReadCallback), state);
        Status = ServerStatus.Connected;
    }

    public void ReadCallback(System.IAsyncResult result)
    {
        StateSocket state = (StateSocket)result.AsyncState;
        Socket handler = state.workSocket;

        int bytesRead = handler.EndReceive(result);
        //Debug.Log("read callback" + bytesRead);
        if (bytesRead > 0)
        {
            //   Parser.ReceiveData(state.buffer, bytesRead);
        }

        handler.BeginReceive(state.buffer, 0, StateSocket.BufferSize, 0, new System.AsyncCallback(ReadCallback), state);
    }

    [ContextMenu("Send")]
    public void Send()
    {
        //byte[] data = Encoding.ASCII.GetBytes(message);
        //_client.BeginSend(data, 0, data.Length, 0, new System.AsyncCallback(SendCallback), _client);
    }

    public void Send(string msg)
    {
        byte[] data = Encoding.ASCII.GetBytes(msg);
        _client.BeginSend(data, 0, data.Length, 0, new System.AsyncCallback(SendCallback), _client);
    }

    private void SendCallback(System.IAsyncResult result)
    {
        Socket listener = (Socket)result.AsyncState;

        int bytesSent = listener.EndSend(result);
        Debug.Log(bytesSent);
        Debug.Log("server - sent data");
    }

    public void OnDisable()
    {
        Close();
    }

    [ContextMenu("close")]
    public void Close()
    {
        _listener.Close();
    }
}


