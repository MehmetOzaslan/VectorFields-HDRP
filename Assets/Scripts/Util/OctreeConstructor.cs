using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;
//void createNode(depth, polygons, box)   
//    for all children (i, j, k) within(N, N, N)       
//    if (depth + 1 == painting depth)       
//            // painting depth reached?
//            // setChildColor(i, j, k, white)      
//            // child is at depth+1       else            c
//            // hildbox = computeSubBox(i, j, k, box)          if (childbox intersect polygons)             child = createChild(i, j, k)             
//            // recurse             createNode(depth + 1, polygons, childbox)          else            setChildAsEmpty(i, j, k) 


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

        OctreeNode octant1 = new OctreeNode(new Bounds(o1center, bounds.size/2));
        OctreeNode octant2 = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, new Vector3Utils.Axis[] { Vector3Utils.Axis.X} ), bounds.size/2));
        OctreeNode octant3 = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, new Vector3Utils.Axis[] { Vector3Utils.Axis.Y}), bounds.size / 2));
        OctreeNode octant4 = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, new Vector3Utils.Axis[] { Vector3Utils.Axis.Y}), bounds.size / 2)); ;
        OctreeNode octant5 = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, new Vector3Utils.Axis[] { Vector3Utils.Axis.Y }), bounds.size / 2)); ;
        OctreeNode octant6 = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, new Vector3Utils.Axis[] { Vector3Utils.Axis.Y }), bounds.size / 2)); ;
        OctreeNode octant7 = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, new Vector3Utils.Axis[] { Vector3Utils.Axis.Y }), bounds.size / 2)); ;
        OctreeNode octant8 = new OctreeNode(new Bounds(Vector3Utils.Reflect(o1center, bounds.center, new Vector3Utils.Axis[] { Vector3Utils.Axis.Y }), bounds.size / 2)); ;



    }

}

public class OctreeConstructor : MonoBehaviour
{

    //Input -> A mesh
    //Output -> An octree or structured buffer octree.
    void GetMeshOctree(Mesh mesh)
    {
        mesh.RecalculateBounds();
        OctreeNode parent = new OctreeNode(mesh.bounds);
        


    }

    //Input -> An Iterable<Vector3> of positions.
    //      -> Some weights for force direction
    //      -> Whether or not there should be vectors along the mesh normals.

    //Output -> An octree structured buffer containing the vector field.
    void GenerateVectorField(IEnumerable<Vector3> positions, IEnumerable<Vector3> forces)
    {

    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
