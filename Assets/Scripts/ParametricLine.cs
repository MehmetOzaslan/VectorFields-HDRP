﻿using B83.ExpressionParser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametricLine : VectorField
{

    public Function3D delegate3D;

    public float tMin = 10;
    public float tMax = -10;
    public int resolution = 100;

    public void Awake()
    {
        delegate3D.parameters = new List<string> {"t"};
    }


    //Positions are found in r(t) = <f(t),g(t),h(t)>
    //Velocity is calculated at r'(t) = <f'(t),g'(t),h'(t)>

    //TODO: Check for possible infinite loops from tMax and tMin.
    protected override void UpdatePositions()
    {
        positions = new List<Vector3>();
        float step = resolution / (tMax - tMin);//step is set so that we get the correct resolution.

        for (float i = tMin; i < tMax; i += step)
        {
            positions.Add(delegate3D.CalculateAtT(i));
        }
    }

    //Velocity is calculated at r'(t) = <f'(t),g'(t),h'(t)>
    protected override void UpdateForces()
    {
        forces = new List<Vector3>();
        float step = resolution / (tMax - tMin);

        for (float i = tMin; i < tMax; i += step)
        {
            Vector3 newVelocity = ExprUtils.numDerivative3D(i, delegate3D);
            forces.Add(newVelocity);
        }
    }

     
}
