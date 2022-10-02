using UnityEngine;
using UnityEditor;

namespace MPO.Editor
{
	[CustomPropertyDrawer(typeof(PropertyID))]
	public class PropertyIDDrawer : PropertyDrawer
	{
		public const string IDControlName = "PropertyIDField";
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			DrawProperty(rect, property, label);
		}

		public static void DrawProperty(Rect rect, SerializedProperty property, GUIContent label)
		{
			SerializedProperty nameProperty = property.FindPropertyRelative("name");
			SerializedProperty idProperty = property.FindPropertyRelative("id");
			using (var check = new EditorGUI.ChangeCheckScope())
			{
				if (label != null)
					nameProperty.stringValue = EditorGUI.TextField(rect, label, nameProperty.stringValue);
				else
					nameProperty.stringValue = EditorGUI.TextField(rect, nameProperty.stringValue);

				if (check.changed)
					SetProperty(nameProperty, idProperty, FormatID(nameProperty.stringValue));
			}
		}

		private static void SetProperty(SerializedProperty name, SerializedProperty id, string propertyName)
		{
			name.stringValue = propertyName;
			id.intValue = Shader.PropertyToID(propertyName);
		}

		public static string FormatID(string id)
		{
			var words = id.Split(' ');
			id = string.Empty;
			foreach (var word in words)
			{
				if (string.IsNullOrWhiteSpace(word))
					continue;
				id += ObjectNames.NicifyVariableName(word);
			}
			
			if (id.Length > 1 && id[0] != '_')
				id = id.Insert(0, "_");
			
			return id;
		}
	}
}