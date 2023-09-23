using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownies : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0.0f, Mathf.Sin(Time.time) * 0.01f, 0.0f);
    }
}
