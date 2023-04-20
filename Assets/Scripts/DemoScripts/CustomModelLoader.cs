using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class CustomModelLoader
{
    Tensor output;
    private Model model;
    private IWorker engine;
    AnimationClip clip;
    public CustomModelLoader(NNModel filePath, AnimationClip clip)
    {
        model = ModelLoader.Load(filePath);
        engine = WorkerFactory.CreateWorker(model, WorkerFactory.Device.GPU);
        this.clip = clip;
    }

    public void generateAnim(List<Quaternion> lRot, List<Quaternion> rRot, List<Vector3> lPos, List<Vector3> rPos)
    {
        Tensor[] inputs = CreateInputTensor(lRot, rRot, lPos, rPos);
        Tensor hands = inputs[0];
        Tensor noise = inputs[1];
        engine.SetInput(hands);
        engine.SetInput(noise);
        Tensor output = engine.Execute().PeekOutput();
        ModifyAnimation(output);
        output.Dispose();
        hands.Dispose();
        noise.Dispose();
        
    }

    // i want to create a tensor with the following shape: 1, 120, 14 from lRot, rRot, lPos, rPos
    public Tensor[] CreateInputTensor(List<Quaternion> lRot, List<Quaternion> rRot, List<Vector3> lPos, List<Vector3> rPos)
    {
        float[,,] data = createInput(lRot, rRot, lPos,rPos);
        float[,,,] noise = createNoise(lRot.Count);
        ComputeBuffer dataBuffer = new ComputeBuffer(lRot.Count*14,sizeof(float));
        ComputeBuffer noiseBuffer = new ComputeBuffer(lRot.Count*68*4,sizeof(float));
        dataBuffer.SetData(data);
        noiseBuffer.SetData(noise);
        Tensor tensorData = new Tensor(new TensorShape(1,lRot.Count,14),dataBuffer);
        Tensor tensoNoise = new Tensor(new TensorShape(1,lRot.Count,68,4),noiseBuffer);
        dataBuffer.Dispose();
        noiseBuffer.Dispose();
        return new Tensor[2] { tensorData, tensoNoise };
    }   
    public float[,,] createInput(List<Quaternion> lRot, List<Quaternion> rRot, List<Vector3> lPos, List<Vector3> rPos)
    {
        float[,,] input = new float[1, lRot.Count, 14];
        Tensor previous = null;
        for (int i = 0; i < lRot.Count; i++)
        {
            float[] x = new float[14] { lRot[i].x, lRot[i].y, lRot[i].z, lRot[i].w,lPos[i].x, lPos[i].y,
                lPos[i].z,rRot[i].x, rRot[i].y, rRot[i].z, rRot[i].w,rPos[i].x, rPos[i].y, rPos[i].z};

            for (int j = 0; j < x.Length; j++)
            {
                input[0, i, j] = x[j];
            } 
        }
        return input;
    }

    private float[,,,] createNoise(int frames)
    {
        float[,,,] noise = new float[1, frames, 68, 4];
        for (int i = 0; i < frames; i++)
        {
            for (int j = 0; j <= 68; j++)
            {
                noise[0, i, j, 0] = Random.Range(-1f, 1f);
                noise[0, i, j, 1] = Random.Range(-1f, 1f);
                noise[0, i, j, 2] = Random.Range(-1f, 1f);
                noise[0, i, j, 3] = Random.Range(-1f, 1f);
            }
        }
        return noise;
    }

    private void ModifyAnimation(Tensor generatedAnim)
    {
        for(int i = 0; i < generatedAnim.shape[1]; i++)
        {
            for(int j = 0; j < generatedAnim.shape[2]; i++)
            {
                float x = generatedAnim[0, i, j,0];
                float y = generatedAnim[0, i, j,1];
                float z = generatedAnim[0, i, j,2];
                float w = generatedAnim[0, i, j,3];
                Debug.Log(x + " " + y + " " + z + " " + w);
            }
        }

    }

    public void OnDisable()
    {
        engine?.Dispose();
    }
    
}
