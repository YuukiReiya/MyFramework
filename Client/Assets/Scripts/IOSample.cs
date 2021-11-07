using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample
{
    public class IOSample : MonoBehaviour
    {
        public string path;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Masters.MasterData.Instance.IsComplete)
            {
                Debug.Log("Complete import task.");
            }
        }
    }
}