using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

public class SocketDemo : MonoBehaviour
{
    public int Port;
    public string Ip;


    private Socket _client;

    public enum ClientStatus { OFF, Connected }
    private ClientStatus status = ClientStatus.OFF;
    public ClientStatus Status
    {
        get { return status; }
        set
        {
            status = value;
        }
    }

    [ContextMenu("Connect")]
    public void Connect()
    {
        Debug.Log("try connection");
        _client = new Socket(SocketType.Stream, ProtocolType.Tcp);

        _client.BeginConnect(new IPEndPoint(IPAddress.Parse(Ip), Port), new System.AsyncCallback(ConnectCallback), _client);

    }

    private void ConnectCallback(System.IAsyncResult result)
    {
        Socket client = (Socket)result.AsyncState;
        client.EndConnect(result);

        Debug.Log("client - connect callback");
        ServerTCP.StateSocket state = new ServerTCP.StateSocket();
        state.workSocket = client;
        Status = ClientStatus.Connected;

        //client.BeginReceive(state.buffer, 0, ServerTCP.StateSocket.BufferSize, 0, new System.AsyncCallback(ReadCallback), state);
    }


    private void ReadCallback(System.IAsyncResult result)
    {
        ServerTCP.StateSocket state = (ServerTCP.StateSocket)result.AsyncState;
        Socket handler = state.workSocket;

        int bytesRead = handler.EndReceive(result);
        if (bytesRead > 0)
        {
            Debug.Log("client - " + Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
        }

        handler.BeginReceive(state.buffer, 0, ServerTCP.StateSocket.BufferSize, 0, new System.AsyncCallback(ReadCallback), state);
    }

    public void Send()
    {
        _client = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _client.Connect(new IPEndPoint(IPAddress.Parse(Ip), Port));
      //  byte[] data = Encoding.ASCII.GetBytes(Parser.GetPathString());
       // _client.BeginSend(data, 0, data.Length, 0, new System.AsyncCallback(SendCallback), _client);
    }

    private void SendCallback(System.IAsyncResult result)
    {
        Socket client = (Socket)result.AsyncState;

        int bytesSent = client.EndSend(result);
        Debug.Log("client - sent data");
        client.Disconnect(true);
    }

    [ContextMenu("Send file")]

    public void SendFile()
    {
        //Debug.Log("begin send");
        //FileInfo info = new FileInfo(path);
        //long size = info.Length;
        //_client.Send(Encoding.ASCII.GetBytes(size.ToString()));
        //_client.BeginSendFile(path, new System.AsyncCallback(SendFileCallback), _client);
    }

    private void SendFileCallback(System.IAsyncResult result)
    {
        Socket client = (Socket)result.AsyncState;
        client.EndSendFile(result);
        Debug.Log("client - sent file");

        //SendFile2();

    }

    [ContextMenu("Disconnect")]
    public void Deco()
    {
        _client.Shutdown(SocketShutdown.Both);
        _client.BeginDisconnect(true, Disconnect, _client);
    }

    public void Disconnect(System.IAsyncResult result)
    {
        Socket client = (Socket)result.AsyncState;
        client.EndDisconnect(result);
        Debug.Log("client - disconnected");
        Status = ClientStatus.OFF;
    }

    public void Close()
    {
        _client.Close();
    }

    public void OnDisable()
    {
        Close();
    }

}
