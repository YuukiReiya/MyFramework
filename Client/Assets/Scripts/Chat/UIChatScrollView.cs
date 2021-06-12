using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.Chat;
using Server.gRPC;

namespace uGUI.Chat
{
    public class UIChatScrollView : ScrollView<UIChatCell>
    {
        private List<UIChatCell> cellList = new List<UIChatCell>();
        public override List<UIChatCell> Elements { 
            get => cellList;
            set => cellList = value;
        }
    }
}