using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DefNameAttribute))]
public class DefNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        var attr = (DefNameAttribute)attribute;
        var names = DefNameCache.GetNames(attr.baseDefClassName);
        if (names == null) names = Array.Empty<string>();

        // 현재 값의 인덱스 찾기
        var current = property.stringValue ?? "";
        int index = Array.FindIndex(names, n => string.Equals(n, current, StringComparison.OrdinalIgnoreCase));

        // UI 영역 나누기: Popup + Refresh 버튼
        const float btnWidth = 60f;
        var popupRect = new Rect(position.x, position.y, position.width - btnWidth - 4f, position.height);
        var btnRect = new Rect(popupRect.xMax + 4f, position.y, btnWidth, position.height);

        EditorGUI.BeginProperty(position, label, property);

        // 드롭다운 옵션에 "(None)" 추가
        string[] options = new string[names.Length + 1];
        options[0] = "(None)";
        for (int i = 0; i < names.Length; i++)
            options[i + 1] = names[i];

        int popupIndex = (index >= 0) ? index + 1 : 0;

        popupIndex = EditorGUI.Popup(popupRect, label.text, popupIndex, options);

        if (popupIndex == 0)
            property.stringValue = "";           // None 선택
        else
            property.stringValue = options[popupIndex];

        if (GUI.Button(btnRect, "Refresh"))
        {
            DefNameCache.Refresh();
        }

        // 만약 현재 값이 목록에 없으면 아래에 경고(선택)
        if (!string.IsNullOrEmpty(current) && index < 0)
        {
            var warnRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2f,
                                    position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.HelpBox(warnRect, $"DefName '{current}' not found in XML.", MessageType.Warning);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
