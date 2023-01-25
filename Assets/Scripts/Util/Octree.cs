using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class OctreeNode
{
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
    int MaxDepth = 5;
    public float maxSize = 0.3f;
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
        if(this.parent== null)
        {
            parent = new OctreeNode(new Bounds(transform.position, Vector3.one * 5));
        }


        Queue<OctreeNode> q = new Queue<OctreeNode>();

        q.Enqueue(parent);

        while(q.Count > 0)
        {
            var currentNode = q.Dequeue();

            //Base case.
            if(currentNode.bounds.size.sqrMagnitude < maxSize) 
            {
                continue;
            }
            else if(Physics.CheckBox(currentNode.bounds.center, currentNode.bounds.size / 2, Quaternion.identity))
            {
                currentNode.ConstructChildren();
                foreach (var item in currentNode.children)
                {
                    q.Enqueue(item);
                }
            }
        }

     }

    ////Not very performant because of rather large stack.
    //private void h_Insert<T>(OctreeNode parent, T inserted, Func<OctreeNode, T, bool> contains, int depth)
    //{
    //    if (depth <= 0 ||  parent.bounds.size.magnitude < maxSize)
    //    {
    //        return;
    //    }

    //    //Construct the children if they don't exist
    //    if(parent.children == null)
    //    {
    //        parent.ConstructChildren();
    //    }

    //    //TODO: Can this be optimized for tail recursion? Yes in theory but no because apparently C# doesn't support it.
    //    foreach (var child in parent.children)
    //    {
    //        if (contains(child, inserted))
    //        {
    //            //Recurse.
    //            h_Insert(child, inserted, contains ,depth-1);
    //        }
    //    }
    //}
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
        DrawOctreeRecursive(octree.parent);
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



        Handles.DrawWireCube(parent.bounds.center, (parent.bounds.size));

        if (parent.children == null || maxDepth == 0)
        {
            return;
        }

        else
        {
            for (int i = 0; i < parent.children.Length; i++)
            {
                DrawOctreeRecursive(parent.children[i], maxDepth-1);
            }
        }


    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }

}
