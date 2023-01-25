using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.VFX;


//For writing to structured buffer
//NOTES:
//LET 0 BE A NULL POINTER.


[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct OctreeNodeStruct
{
    public int id;//TODO: This can be removed.
    //public int[] children; Cannot use because non-blittable.
    public int o1;
    public int o2;
    public int o3;
    public int o4;
    public int o5;
    public int o6;
    public int o7;
    public int o8;
    public Vector3 data1;
    public Vector3 data2;
    public int[] GetOctants()
    {
        return new int[] { o1, o2, o3, o4, o5, o6, o7, o8 };
    }
    public OctreeNodeStruct(OctreeNode octreeNode) : this()
    {
        this.id = octreeNode.id;
        this.data1 = octreeNode.bounds.center;
        this.data2= octreeNode.bounds.extents;
        if (octreeNode.children != null)
        {
            o1 = octreeNode.children[0].id;
            o2 = octreeNode.children[1].id; 
            o3 = octreeNode.children[2].id;
            o4 = octreeNode.children[3].id;
            o5 = octreeNode.children[4].id;
            o6 = octreeNode.children[5].id;
            o7 = octreeNode.children[6].id;
            o8 = octreeNode.children[7].id;
        }

        else
        {
            o1 = -1;
            o2 = -1;
            o3 = -1;
            o4 = -1;
            o5 = -1;
            o6 = -1;
            o7 = -1;
            o8 = -1;
        }

    }
}

public class OctreeNode
{
    public int id;
    public OctreeNode[] children;
    public Bounds bounds;

    public OctreeNode(Bounds bounds)
    {
        this.bounds = bounds;
    }

    public void ConstructChildren()
    {
        Vector3 o1center = bounds.center + bounds.size/4;
        children = new OctreeNode[8];
        children[0] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct1), bounds.size / 2));
        children[1] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct2), bounds.size / 2));
        children[2] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct3), bounds.size / 2));
        children[3] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct4), bounds.size / 2));
        children[4] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct5), bounds.size / 2));
        children[5] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct6), bounds.size / 2));
        children[6] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct7), bounds.size / 2));
        children[7] = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, Vector3Utils.oct8), bounds.size / 2));
    }
}

[ExecuteInEditMode]
public class Octree : MonoBehaviour
{
    uint numNodes = 0;

    [SerializeField]
    [Range(0.1f, 1f)]
    public float minSize = 0.3f;
    public OctreeNode parent;
    public List<GameObject> surrounding;


    public void Start()
    {
        GenerateOctree();
    }

    private void Update()
    {
        GenerateOctree();
    }

    //Attempt to reduce stack space by using a queue.
    private void GenerateOctree()
    {

        if (parent == null)
        {
            parent = new OctreeNode(new Bounds(transform.position, Vector3.one * 5));
        }


        Queue<OctreeNode> q = new Queue<OctreeNode>();


        int curr_id = 0;
        numNodes = 0;

        q.Enqueue(parent);

        while(q.Count > 0)
        {

            var currentNode = q.Dequeue();
            currentNode.id = curr_id;
            curr_id++;
            numNodes++;

            //Base case.
            if(currentNode.bounds.size.sqrMagnitude < minSize) 
            {
                continue;
            }
            else if(Physics.CheckBox(currentNode.bounds.center, currentNode.bounds.size / 2, Quaternion.identity))
            {
                //Construct the children.
                currentNode.ConstructChildren();

                for (int i = 0; i < currentNode.children.Length; i++)
                {
                    var child = currentNode.children[i];

                    //Small optimization, prune children which don't collide.
                    if (!Physics.CheckBox(child.bounds.center, child.bounds.size / 2, Quaternion.identity))
                    {
                        currentNode.children[i] = null;
                    }

                    //Otherwise, continue BFS on the child.
                    q.Enqueue(child);
                }

            }
        }
    }


    public uint GetCount()
    {
        return this.numNodes;
    }

    public Texture3D GetTexture3D()
    {
        //Depth: 1 per node. (numnodes
        //Width: 8 for each node element.
        //Height:1
        //Creating texture with proper size
        //Pray that the texture format is good enough for indexing.
        Texture3D texture3d = new Texture3D(1, 8, (int)numNodes, TextureFormat.RGB565, false) ;
        texture3d.wrapMode = TextureWrapMode.Clamp;
        texture3d.filterMode = FilterMode.Point;   
        texture3d.anisoLevel = 0;

        OctreeNodeStruct[] nodes = this.ToStructArray();

        foreach (var node in nodes)
        {
            int[] octants = node.GetOctants();

            for (int i = 0; i < octants.Length; i++)
            {
                texture3d.SetPixel(
                    //The parent is at depth node.id
                    0, i, node.id,
                    //The pointer to the octant is at this DEPTH! Admittedly there is some wasted space here.
                    new Color(0, 0, octants[i])
                    );
            }
        }
        return texture3d;


    }

    public OctreeNodeStruct[] ToStructArray()
    {
        OctreeNodeStruct[] nodes = new OctreeNodeStruct[numNodes];

        //Get all of the nodes via BFS.
        //This also should be ideal in keeping the ordering proper.
        Queue<OctreeNode> q = new Queue<OctreeNode>();
        q.Enqueue(parent);
        while (q.Count > 0)
        {
            var currentNode = q.Dequeue();

            //Everything else is BFS except this
            nodes[currentNode.id] = new OctreeNodeStruct(currentNode);


            if (currentNode.children == null)
                continue;

            foreach (var item in currentNode.children)
            {
                q.Enqueue(item);
            }
        }

        return nodes;
    }

    public GraphicsBuffer GetGraphicsBuffer()
    {
        int size = Marshal.SizeOf(new OctreeNodeStruct(parent));//pass in parent because if the int[8] (dynamic size)

        GraphicsBuffer octree = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)numNodes, size);

        octree.SetData(this.ToStructArray());
        return octree;
    }

}


[CustomEditor(typeof(Octree))]
class OctreeEditor : Editor
{

    private BoxBoundsHandle boundsHandle = new BoxBoundsHandle();

    private void OnSceneGUI()
    {
        Octree octree = (Octree)target;

        // copy the target object's data to the handle
        boundsHandle.center = octree.parent.bounds.center;
        boundsHandle.size = octree.parent.bounds.size;

        EditorGUI.BeginChangeCheck();
        Handles.color = Color.cyan;
        DrawOctreeRecursive(octree.parent);
        Handles.color = Color.red;
        boundsHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            // record the target object before setting new values so changes can be undone/redone
            Undo.RecordObject(octree, "Change Bounds");

            // copy the handle's updated data back to the target object
            Bounds newBounds = new Bounds();
            newBounds.center = boundsHandle.center;
            newBounds.size = boundsHandle.size;
            octree.parent.bounds = newBounds;
        }


    }

    private void DrawOctreeRecursive(OctreeNode parent, int maxDepth = 5)
    {

        if(maxDepth == 5 || maxDepth == 4)
        {
            Handles.Label(parent.bounds.center, parent.id.ToString());
        }

        Handles.DrawWireCube(parent.bounds.center, parent.bounds.size);

        if (parent.children == null)
            return;

        else
        {

            for (int i = 0; i < parent.children.Length; i++)
            {
                if (parent.children[i] == null) continue;
                DrawOctreeRecursive(parent.children[i], maxDepth-1);
            }
        }


    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Octree octree = (Octree)target;

        if (GUILayout.Button("TryGenerateBuffer"))
        {
            octree.GetGraphicsBuffer();
        }

        if (GUILayout.Button("TryGenerateTexture3D"))
        {
            octree.GetTexture3D();
        }

    }

}
