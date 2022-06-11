using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleIceShaderController : MonoBehaviour
{
    public Vector3 euler = new Vector3(0, 1, 0);
    public float speed = 0.8f;
    IEnumerator Start()
    {
        while (true)
        {
            var norm = euler.normalized;
            this.transform.Rotate(norm * speed);
            yield return null;
        }
    }
}
