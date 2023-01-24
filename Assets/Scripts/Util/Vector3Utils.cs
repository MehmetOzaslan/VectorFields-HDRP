using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class Vector3Utils
{
    public static Vector3 oct1 = new Vector3(1, 1, 1);
    public static Vector3 oct2 = new Vector3(-1, 1, 1);
    public static Vector3 oct3 = new Vector3(1, -1, 1);
    public static Vector3 oct4 = new Vector3(1, 1, -1);
    public static Vector3 oct5 = new Vector3(-1, -1, 1);
    public static Vector3 oct6 = new Vector3(-1, 1, -1);
    public static Vector3 oct7 = new Vector3(1, -1, -1);
    public static Vector3 oct8 = new Vector3(-1, -1, -1);




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

    public static Vector3 Reflect(Vector3 point, Vector3 relativeTo, Vector3 sign)
    {
        //Transform to relative space.
        Vector3 relativePoint = point - relativeTo;

        //Flip coordinates arounds.
        relativePoint.x = relativePoint.x * Mathf.Sign(sign.x);
        relativePoint.y = relativePoint.y * Mathf.Sign(sign.y);
        relativePoint.z = relativePoint.z * Mathf.Sign(sign.z);

        //Transform back to world space.
        return relativePoint + relativeTo;
    }
}
