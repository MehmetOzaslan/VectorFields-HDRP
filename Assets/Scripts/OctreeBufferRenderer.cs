using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OctreeBufferRenderer : MonoBehaviour
{

    public Octree octree;
    public VisualEffect visualEffect;
    int effectBufferID = Shader.PropertyToID("GraphicsBuffer");


    void SetupOctree()
    {
        
        visualEffect.SetGraphicsBuffer(effectBufferID, octree.ToGraphicsBuffer());


    }
}
