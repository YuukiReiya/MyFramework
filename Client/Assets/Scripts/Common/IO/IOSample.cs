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
            var csv = new IO.CSVImporter();
        }

        // Update is called once per frame
        void Update()
        {
            if (MasterData.Masters.Instance.IsComplete)
            {
                Debug.Log("Complete import task.");
            }
        }
    }
}