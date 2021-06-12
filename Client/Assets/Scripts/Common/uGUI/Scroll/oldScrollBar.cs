﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if false
namespace uGUI
{
    [RequireComponent(typeof(UnityEngine.UI.Scrollbar))]
    [DisallowMultipleComponent]
    public class ScrollBar : UIBehaviour
    {
        [SerializeField]
        private Scrollbar scrollbar;
        public Scrollbar Scrollbar => scrollbar;

        protected override void Reset()
        {
            scrollbar = GetComponent<Scrollbar>();
        }
    }

}
#endif