using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
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

    public int MaxDepth = 5;
    public OctreeNode parent;
    public List<GameObject> surrounding;



    public void Start()
    {
        Reconstruct();
    }

    public void Reconstruct()
    {
        parent = new OctreeNode(new Bounds(transform.position, Vector3.one));

        foreach (var item in surrounding)
        {
            parent.bounds.Encapsulate(item.GetComponent<Collider>().bounds);
        }

        foreach (var item in surrounding)
        {
            InsertObject(item);
        }

    }

    public void InsertObject(GameObject obj)
    {
        if (obj.GetComponent<Collider>())
        {

            Insert<Bounds>(new Bounds(), (node, bounds) => { return BoundsIntersecting(node.bounds); });
        }
    }

    bool BoundsIntersecting(Bounds bounds)
    {
        return Physics.CheckBox(bounds.center, bounds.size/2, Quaternion.identity);
    }

    public void InsertPoint(Vector3 point) {
        Insert(point, (node, point) => { return node.bounds.Contains(point); });
    }

    //Insert a point.
    public void Insert<T>(T inserted, Func<OctreeNode, T, bool> contains)
    {
        h_Insert(parent, inserted, contains, MaxDepth);
    }

    private void h_Insert<T>(OctreeNode parent, T inserted, Func<OctreeNode, T, bool> contains, int depth)
    {
        Debug.Log(depth);
        if (depth == 0)
        {
            return;
        }

        //Construct the children if they don't exist
        if(parent.children == null)
        {
            parent.ConstructChildren();
        }

        //Optimize potentially
        foreach (var child in parent.children)
        {
            if (contains(child, inserted))
            {
                //Recurse.
                h_Insert(child, inserted, contains ,depth-1);
            }
        }
    }
}

[CustomEditor(typeof(Octree))]
class OctreeEditor : Editor
{

    private void OnSceneGUI()
    {
        Octree octree = (Octree)target;
        DrawOctreeRecursive(octree.parent);
    }


    private void DrawOctreeRecursive(OctreeNode parent)
    {
        Handles.DrawWireCube(parent.bounds.center, (parent.bounds.size));

        if (parent.children == null)
        {
            return;
        }

        else
        {
            for (int i = 0; i < parent.children.Length; i++)
            {
                DrawOctreeRecursive(parent.children[i]);
            }
        }


    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (GUILayout.Button("Reconstruct"))
        {

            ((Octree)target).Reconstruct();
        }

    }

}
