using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sample : MonoBehaviour
{

    class info
    {
        public string msg;
        public string stc;

        public override bool Equals(object obj)
        {
            var o = obj as info;
            if (o == null) return false;

            return this.msg == o.msg
                && this.stc == o.stc;
        }

        public override int GetHashCode()
        {
            if (msg == null || stc == null) return 0;
            return msg.GetHashCode() ^ stc.GetHashCode();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("test")]
    void test()
    {
        info i = new info();
        i.msg = string.Empty;
        i.stc = string.Empty;
        Debug.Log("empty:" + i.GetHashCode());

        i.msg = null;
        i.stc = null;
        Debug.Log("null:" + i.GetHashCode());
    }
}
