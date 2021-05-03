using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FancyScrollView;
using Server.gRPC;

namespace uGUI.Chat.FancyScrollView
{
    public class UIChatScrollView : FancyScrollRect<DuplexChatReceive>
    {
        [SerializeField] private float cellSize = 100f;
        [SerializeField] private GameObject cellPrefab = null;

        protected override float CellSize => cellSize;
        protected override GameObject CellPrefab => cellPrefab;
        public int ElementsCount => ItemsSource.Count;

        public void UpdateElements(IList<DuplexChatReceive> elements)
        {
            UpdateContents(elements);
        }
    }
}