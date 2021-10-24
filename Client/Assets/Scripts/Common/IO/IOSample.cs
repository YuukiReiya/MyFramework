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
            //csv.Execute(path);
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("�C���|�[�g�m�F");
            if (DataStorage.Masters.Instance.IsComplete)
            {
                Debug.Log("�S�f�[�^�C���|�[�g����");
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach(var dict in DataStorage.Masters.Instance.CSV)
                {
                    var current = dict;
                    foreach (var list in current.Value.Dict)
                    {
                        Debug.Log($"{current.Key}:<color=green>{list.Key}</color>");
                        foreach(var line in list.Value)
                        {
                            Debug.Log($"{line}");
                        }
                    }
                }
            }
        }
    }
}