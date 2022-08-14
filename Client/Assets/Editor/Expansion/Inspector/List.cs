using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

namespace Editor.Inspector
//namespace Inspector
{
    using Assert = UnityEngine.Assertions.Assert;
    public class List
    {
        SerializedProperty property = null;
        Vector2 scrollPosition = Vector2.zero;
        int elementIndexA;
        int elementIndexB;

        #region color
        Color backgroundColor;
        Color backgroundLightColor;
        Color backgroundDarkColor;
        Color elementColor;
        #endregion

        #region rect

        #endregion

        #region const
        const string PlusButton = @"+";
        const string MinusButton = @"-";
        const string DisableButton = @"Ξ";
        const string ChangeOrderButton = @"Ξ";
        const string ChangeUpOrderButton = @"↑";
        const string ChangeDownOrderButton = @"↓";
        const string RemoveButton = @"×";
        const int RightButton = 1;
        const float ScrollElementViewHeight = 144f;
        const int ScrollRequireElementCount = 4;
        #endregion

        #region property
        #endregion

        public event Action<Rect> onDrawHeader = null;
        public event Action<Rect, int> onDrawElement = null;
        public event Action onAddElement = null;
        public event Action onRemoveElement = null;

        /// <summary>
        /// 「↔」ボタン押下時のイベント.
        /// </summary>
        /// <br>入れ替え前の要素番号.</br>
        /// <br>入れ替え先の要素番号.</br>
        public event Action<int, int> onSwapElement = null;

        /// <summary>
        /// 「↑」ボタン押下時のイベント.
        /// </summary>
        /// <br>入れ替え前の要素番号.</br>
        /// <br>入れ替え先の要素番号.</br>
        public event Action<int,int> onSwapElementUpward = null;
        /// <summary>
        /// 「↓」ボタン押下時のイベント.
        /// </summary>
        /// <param1>
        /// 入れ替え前の要素番号.
        /// </param1>
        /// <param2>
        /// 入れ替え先の要素番号.
        /// </param2>
        public event Action<int,int> onSwapElementDownward = null;

        int Size { get { return property != null ? property.arraySize : 0; } }
        public float Height { get { return property.isExpanded ? (Size + 1) * LineHeight : LineHeight; } }
        private float ContentHeight { get; } = EditorGUIUtility.singleLineHeight;
        private float LineHeight { get; } = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 4;
        float FieldWidth = EditorGUIUtility.fieldWidth;

        public List(SerializedProperty property)
        {
            this.property = property;

            Assert.IsNotNull(property, $"property is null. {property.name}");

            if (property == null) return;

            onDrawHeader = (rect) =>
            {
                rect.xMin += 10;
                EditorGUI.PropertyField(rect, property);
            };

            onDrawElement = (rect, index) =>
            {
                GUIContent element = new GUIContent($"{property.name}_{index}");
                var tmp = GUI.contentColor;
                GUI.contentColor = Color.black;
                EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), element);
                GUI.contentColor = tmp;
            };

            onAddElement = () =>
            {
                property.arraySize++;
            };

            onRemoveElement = () =>
            {
                if (Size == 0) return;

                var index = Math.Max(0, Size - 1);
                property.DeleteArrayElementAtIndex(index);
            };

            // color
            backgroundColor = GUI.backgroundColor;
            backgroundLightColor = backgroundColor;
            backgroundDarkColor = backgroundLightColor * 0.6f;
            backgroundDarkColor.a = 1;
            ColorUtility.TryParseHtmlString($"D1D1D1", out elementColor);
        }

        ~List()
        {
            property = null;
            onDrawHeader = null;
            onDrawElement = null;
            onAddElement = null; 
            onRemoveElement = null;
        }

        public void Draw()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader();
            DrawElements();
            EditorGUILayout.EndVertical();
        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(property.name));

            // +
            var buttonRect = GUILayoutUtility.GetLastRect();
            {
                buttonRect.xMin = buttonRect.xMax - buttonRect.height;
                if (GUI.Button(buttonRect, PlusButton)) onAddElement?.Invoke();
            }

            // input
            var fieldRect = buttonRect;
            {
                fieldRect.xMin = buttonRect.xMin - EditorGUIUtility.fieldWidth;
                fieldRect.xMax = buttonRect.xMin;
                var style = GUI.skin.textField;
                style.alignment = TextAnchor.MiddleRight;
                property.arraySize = Math.Max(0, EditorGUI.IntField(fieldRect, property.arraySize, style));
            }

            // -
            {
                buttonRect.xMax = fieldRect.xMin;
                buttonRect.xMin = fieldRect.xMin - buttonRect.height;
                if (GUI.Button(buttonRect, MinusButton)) onRemoveElement?.Invoke();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        void DrawElements()
        {
            if (!property.isExpanded) return;

            if (Size == 0)
            {
                EditorGUILayout.HelpBox($"要素がありません。",MessageType.Info);
                return;
            }


            var rect = EditorGUILayout.BeginVertical(GUI.skin.window);
            // 要素の入れ替え.
            {
                var center = rect.center;
                var left = rect.xMin;
                var right = rect.xMax;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 4;
                rect.x += 4;
                EditorGUI.LabelField(rect, @"要素の入れ替え");

                EditorGUILayout.BeginHorizontal();
                // サイズ確保のためのダミー.
                GUILayout.Label(@"");
                EditorGUILayout.EndHorizontal();

                // Center Anchor.
                {
                    float centerLeft = center.x - EditorGUIUtility.singleLineHeight;
                    rect.xMin = centerLeft;
                    float centerRaight = center.x + EditorGUIUtility.singleLineHeight;
                    rect.xMax = centerRaight;
                    rect.y += EditorGUIUtility.singleLineHeight;
                    if(GUI.Button(rect, @"↔"))
                    {
                        property.MoveArrayElement(elementIndexA, elementIndexB);
                        onSwapElement?.Invoke(elementIndexA, elementIndexB);
                    }

                    const float offset = 2;
                    rect.xMax = centerLeft - offset;
                    rect.xMin = rect.xMax - EditorGUIUtility.fieldWidth;
                    elementIndexA = EditorGUI.IntField(rect, Math.Max(0, Math.Min(elementIndexA, (Size - 1))));
                    rect.xMax = rect.xMin - offset;
                    rect.xMin = left;
                    var style = GUI.skin.label;
                    style.alignment = TextAnchor.MiddleRight;
                    EditorGUI.SelectableLabel(rect, @"要素α", style);

                    rect.xMin = centerRaight + offset;
                    rect.xMax = rect.xMin + EditorGUIUtility.fieldWidth;
                    elementIndexB = EditorGUI.IntField(rect, Math.Max(0, Math.Min(elementIndexB, (Size - 1))));
                    rect.xMin = rect.xMax + offset;
                    rect.xMax = rect.xMin + EditorGUIUtility.fieldWidth;
                    style.alignment = TextAnchor.MiddleLeft;
                    EditorGUI.SelectableLabel(rect, @"要素β", style);

                    //var buttonLabel = @"入れ替え";
                    //rect.xMax = right - EditorGUIUtility.singleLineHeight/2;
                    //rect.xMin = rect.xMax - buttonLabel.Length * EditorGUIUtility.singleLineHeight;
                    //GUI.Button(rect, buttonLabel);
                }
            }
            var isScroll = ScrollRequireElementCount < Size;
            float height = isScroll ? ScrollElementViewHeight : 0;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(height), GUILayout.ExpandHeight(true));
            for (int i = 0; i < Size; ++i)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                rect = EditorGUILayout.GetControlRect();
                rect.xMax = rect.xMin + EditorGUIUtility.singleLineHeight;

                // ↑
                if (i != 0 && GUI.Button(rect, ChangeUpOrderButton))
                {
                    var index = Math.Max(0, Math.Min(i, i - 1));
                    property.MoveArrayElement(i, index);
                    onSwapElementUpward?.Invoke(i, index);
                }

                // index
                int MaxIndex = Math.Max(0, Size - 1);
                int digit = (int)Math.Log10(MaxIndex) + 1;
                rect.xMin = rect.xMax;
                rect.xMax = rect.xMin + Math.Max(0, 12 + (digit * EditorGUIUtility.singleLineHeight / 2));
                GUIStyle style = GUI.skin.box;
                style.alignment = TextAnchor.MiddleRight;
                style.stretchWidth = true;
                style.normal.textColor = elementColor;
                EditorGUI.SelectableLabel(rect, $"[{i}]", style);

                //↓
                rect.xMin = rect.xMax;
                rect.xMax = rect.xMin + EditorGUIUtility.singleLineHeight;
                {
                    EditorGUI.BeginDisabledGroup((Size - 1) <= i);
                    if (GUI.Button(rect, ChangeDownOrderButton))
                    {
                        var index = Math.Max(i, Math.Min(Size, i + 1));
                        property.MoveArrayElement(i, index);
                        onSwapElementDownward?.Invoke(i, index);
                    }
                    EditorGUI.EndDisabledGroup();
                }

                rect.xMin = rect.xMax;
                rect.xMax = rect.xMin + Math.Max(rect.height, EditorGUIUtility.labelWidth) ;
                EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(i), GUIContent.none);

                rect = GUILayoutUtility.GetLastRect();
                rect.xMin = rect.xMax - EditorGUIUtility.singleLineHeight;

                if(GUI.Button(rect, RemoveButton))
                {
                    property.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}