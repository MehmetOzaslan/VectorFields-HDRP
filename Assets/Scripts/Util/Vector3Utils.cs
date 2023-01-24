using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class Vector3Utils
{

    public static Vector3Int RoundedPoint(Vector3 point)
    {
        return new Vector3Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), Mathf.RoundToInt(point.z));
    }

    public static Color V3Color(Vector3 color, float alpha = 1)
    {
        return new Color(color.x, color.y, color.z, alpha);
    }

    public static Vector3 ClampMinMax(Vector3 input,  Vector3 min, Vector3 max)
    {
        float x = Mathf.Clamp(input.x, min.x, max.x);
        float y = Mathf.Clamp(input.y, min.y, max.y);
        float z = Mathf.Clamp(input.z, min.z, max.z);

        return new Vector3(x, y, z);

    }


    public enum Axis
    {
        X, Y, Z
    }


    public static Vector3 Reflect(Vector3 point, Vector3 relativeTo, Axis axis)
    {
        Vector3 relativePoint = point - relativeTo;

        switch (axis)
        {
            case Axis.X:
                relativePoint.x = -relativeTo.x;
                break;
            case Axis.Y:
                relativePoint.y = -relativeTo.y;
                break;
            case Axis.Z:
                relativePoint.z = -relativeTo.z;
                break;
        }
        return relativePoint + relativeTo;
    }

    public static Vector3 Reflect(Vector3 point, Vector3 relativeTo, Axis[] reflectionSequence)
    {
        return h_Reflect(point, relativeTo, reflectionSequence);
    }

    private static Vector3 h_Reflect(Vector3 reflected, Vector3 relativeTo, Axis[] axes, int current_axis = 0) {
        //If there is no reflection
        if(axes == null || axes.Length == 0) { return reflected; }
        //If all reflections have been applied.
        if (current_axis == axes.Length) return reflected;
        //Recursively apply reflections.
        return h_Reflect(Reflect(reflected, relativeTo, axes[current_axis]), relativeTo, axes, current_axis + 1);
    }

}
