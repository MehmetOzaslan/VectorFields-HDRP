using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class OctreeBufferRenderer : MonoBehaviour
{

    public Octree octree;
    public VisualEffect visualEffect;
    int effectBufferID = Shader.PropertyToID("GraphicsBuffer");
    int countID = Shader.PropertyToID("Count");

    private void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();
    }

    public void SetupOctree()
    {
        visualEffect.SetGraphicsBuffer(effectBufferID, octree.GetGraphicsBuffer());
        visualEffect.SetUInt(countID, octree.GetCount());
    }
}

[CustomEditor(typeof(OctreeBufferRenderer))]
public class OctreeBufferRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        OctreeBufferRenderer renderer = (OctreeBufferRenderer)target;

        if(GUILayout.Button("Set Properties"))
        {
            renderer.SetupOctree();
        }
    }

}
