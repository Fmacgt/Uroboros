using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Hierarchy))]
public class BaseDrawer : PropertyDrawer {

	int size;
	
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		EditorGUIUtility.labelWidth = 45f;
		label = EditorGUI.BeginProperty(position, label, property);
		label.text = "";
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);
		EditorGUI.indentLevel = 0;
		contentPosition.width /= 4f;
		//contentPosition.y += contentPosition.height;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("name"), GUIContent.none);

		contentPosition.x += contentPosition.width + 8f;  
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("parent"), new GUIContent("Parent"));

		contentPosition.x += contentPosition.width + 2f;
		contentPosition.width *= 4f; 
		//contentPosition.y += contentPosition.height;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("function"), new GUIContent("Func"));
		EditorGUI.EndProperty();
	}
}
