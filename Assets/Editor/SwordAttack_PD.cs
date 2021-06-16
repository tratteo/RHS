using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Sword.Attack))]
internal class SwordAttack_PD : PropertyDrawer
{
    private int fields = 0;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return fields * EditorGUIUtility.singleLineHeight + fields * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        fields = 0;
        EditorGUI.BeginProperty(position, label, property);
        Rect currentPos = new Rect(position.x, position.y, EditorGUIUtility.currentViewWidth - EditorGUIUtility.pixelsPerPoint * 24F, EditorGUIUtility.singleLineHeight);
        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        IncrementPos(ref currentPos);
        EditorGUI.indentLevel = 2;
        property.FindPropertyRelative("duration").floatValue = EditorGUI.FloatField(currentPos, "Duration", property.FindPropertyRelative("duration").floatValue);
        IncrementPos(ref currentPos);

        property.FindPropertyRelative("damageMultiplier").floatValue = EditorGUI.FloatField(currentPos, "Damage multiplier", property.FindPropertyRelative("damageMultiplier").floatValue);
        IncrementPos(ref currentPos);

        property.FindPropertyRelative("range").floatValue = EditorGUI.FloatField(currentPos, "Range", property.FindPropertyRelative("range").floatValue);
        IncrementPos(ref currentPos);

        property.FindPropertyRelative("isParriable").boolValue = EditorGUI.Toggle(currentPos, "Parriable", property.FindPropertyRelative("isParriable").boolValue);
        IncrementPos(ref currentPos);

        property.FindPropertyRelative("offset").vector3Value = EditorGUI.Vector3Field(currentPos, "Offset", property.FindPropertyRelative("offset").vector3Value);
        IncrementPos(ref currentPos);

        bool res = property.FindPropertyRelative("overrideEffect").boolValue = EditorGUI.Toggle(currentPos, "Override Effect", property.FindPropertyRelative("overrideEffect").boolValue);
        IncrementPos(ref currentPos);
        if (res)
        {
            EditorGUI.PropertyField(currentPos, property.FindPropertyRelative("effectOverride"));
            IncrementPos(ref currentPos);
        }

        res = property.FindPropertyRelative("overrideEffectScale").boolValue = EditorGUI.Toggle(currentPos, "Override Effect Scale", property.FindPropertyRelative("overrideEffectScale").boolValue);
        IncrementPos(ref currentPos);
        if (res)
        {
            EditorGUI.PropertyField(currentPos, property.FindPropertyRelative("effectScale"));
            IncrementPos(ref currentPos);
        }

        res = property.FindPropertyRelative("overrideColliderAngle").boolValue = EditorGUI.Toggle(currentPos, "Override Collider Angle", property.FindPropertyRelative("overrideColliderAngle").boolValue);
        IncrementPos(ref currentPos);
        if (res)
        {
            EditorGUI.PropertyField(currentPos, property.FindPropertyRelative("colliderAngle"));
            IncrementPos(ref currentPos);
        }
        EditorGUI.EndProperty();
    }

    private void IncrementPos(ref Rect position)
    {
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        fields++;
    }
}