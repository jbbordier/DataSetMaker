using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class CustomModelLoader
{
    Tensor output;
    private Model model;
    private IWorker engine;
    public CustomModelLoader(List<Quaternion>lRot, List<Quaternion> rRot, List<Vector3> lPos, List<Vector3> rPos, string filePath)
    {

        model = ModelLoader.Load(filePath);
        engine = WorkerFactory.CreateWorker(model, WorkerFactory.Device.GPU);
        Tensor input = createInput(lRot, rRot,lPos , rPos);
        
        generateAnim(input);
    }

    public void generateAnim(Tensor input)
    {
        output = engine.Execute(input).PeekOutput();
    }

    public Tensor createInput(List<Quaternion> lRot, List<Quaternion> rRot, List<Vector3> lPos, List<Vector3> rPos)
    {
        Tensor input = new Tensor(new TensorShape(1, lRot.Count, 14));
        for (int i = 0; i < lRot.Count; i++)
        {
            float[] x = new float[14] { lRot[i].x, lRot[i].y, lRot[i].z, lRot[i].w,lPos[i].x, lPos[i].y,
                lPos[i].z,rRot[i].x, rRot[i].y, rRot[i].z, rRot[i].w,rPos[i].x, rPos[i].y, rPos[i].z};

            //input[1, i] = x;
        }

        return input;
    }
    
}
