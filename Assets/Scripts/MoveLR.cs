using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoveLR : MonoBehaviour
{

    float amount = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.fwd * Mathf.Sin(Time.time) * amount;
    }
}
