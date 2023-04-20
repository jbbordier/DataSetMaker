using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEditor.Compilation;
using UnityEngine;


public class Server : MonoBehaviour
{

    private class Data
    {
        public List<Quaternion> leftHandRotations;
        public List<Quaternion> rightHandRotations;
        public List<Vector3> leftHandPositions;
        public List<Vector3> rightHandPositions;
    }
    [Serializable]
    class JsonAnim
    {
        public int numFrames;
        public int numJoints;
        public List<JsonListQuaternion> rotations;


        public List<List<Quaternion>> FloatToQuaternion()
        {
            List<List<Quaternion>>quaternions = new List<List<Quaternion>>();
            foreach (JsonListQuaternion q in rotations)
            {
                quaternions.Add(q.getListQuaternion());
            }
            return quaternions;
        }
    }
    [Serializable]
    class JsonQuaternion
    { 
        public List<float> quat;

        public Quaternion FloatToQuaternions()
        {
            return new Quaternion(quat[0], quat[1], quat[2], quat[3]);
        }
    }

    [Serializable]
    class JsonListQuaternion
    {
        public List<JsonQuaternion> joints;

        public List<Quaternion> getListQuaternion()
        {
            List<Quaternion> quaternions = new List<Quaternion>();
            foreach (JsonQuaternion q in joints)
            {
                quaternions.Add(q.FloatToQuaternions());
            }
            return quaternions;
        }
    }

    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;

    // The IP address and port of the Python server
    private string ipAddress = "127.0.0.1";
    private int port = 5000;
    internal bool hasReceiveData;

    private void OnEnable()
    {
        // Connect to the server
        client = new TcpClient(ipAddress, port);
        client.SendBufferSize = 500000;
        client.ReceiveBufferSize = 500000;
        stream = client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream,System.Text.Encoding.Default,500000);

        Debug.Log("Connected to server");
        hasReceiveData = false;
    }
    void Update()
    {

        // Wait for the response from the server
        if (stream.DataAvailable)
        {
            string response = reader.ReadToEnd();
            Debug.Log($"Received response: {response}");
            // here i want to convert the json to a JsonAnim object
            JsonAnim anim = JsonUtility.FromJson<JsonAnim>(response);
            List<List<Quaternion>> quaternions = anim.FloatToQuaternion();
            // here i want to send the data to the animator
            gameObject.GetComponent<GameManagerDemo>().SetAnimation(quaternions);
            hasReceiveData = true;
        }
    }
    public void SendData(List<Quaternion> lRot, List<Quaternion> rRot, List<Vector3> lPos, List<Vector3> rPos)
    {
        Data datas = new Data() { leftHandPositions=lPos,leftHandRotations=lRot,rightHandPositions=rPos,rightHandRotations=rRot};
        string data = JsonUtility.ToJson(datas);
        int sizeInBytes = System.Text.Encoding.UTF8.GetByteCount(data);
        Debug.Log(sizeInBytes);
        writer.Write(data);
        writer.Flush();
    }
    private void OnDisable()
    {
        // Close the connection when the application quits
        reader.Close();
        client.Close();
        writer.Close();
    }



}
