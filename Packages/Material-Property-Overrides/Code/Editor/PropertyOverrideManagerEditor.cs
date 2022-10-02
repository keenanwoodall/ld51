using UnityEngine;
using UnityEditor;

namespace MPO.Editor
{
	[CustomEditor(typeof(PropertyOverrideManager)), CanEditMultipleObjects]
	public class PropertyOverrideManagerEditor : UnityEditor.Editor
	{
		public static readonly string[] Labels = new[]
		{
			"float", "int", "Color",
			"Vector 2", "Vector 3", "Vector 4",
			"bool", "Direction", "Position",
			"Texture", "Transform"
		};

		public static System.Type[] Types = new[]
		{
			typeof(FloatPropertyOverride), typeof(IntPropertyOverride), typeof(ColorPropertyOverride),
			typeof(Vector2PropertyOverride), typeof(Vector3PropertyOverride), typeof(Vector4PropertyOverride),
			typeof(BoolPropertyOverride), typeof(DirectionPropertyOverride), typeof(PositionPropertyOverride),
			typeof(TexturePropertyOverride), typeof(TransformPropertyOverride)
		};
		
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			/*using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("float", EditorStyles.miniButtonLeft))
				{
					Undo.SetCurrentGroupName("Add Override");
					AddComponent<FloatPropertyOverride>();
				}

				if (GUILayout.Button("int", EditorStyles.miniButtonMid))
				{
					Undo.SetCurrentGroupName("Add Override");
					AddComponent<IntPropertyOverride>();
				}

				if (GUILayout.Button("color", EditorStyles.miniButtonRight))
				{
					Undo.SetCurrentGroupName("Add Override");
					AddComponent<ColorPropertyOverride>();
				}
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("v2", EditorStyles.miniButtonLeft))
				{
					Undo.SetCurrentGroupName("Add Override");
					AddComponent<Vector2PropertyOverride>();
				}
				if (GUILayout.Button("v3", EditorStyles.miniButtonMid))
				{
					Undo.SetCurrentGroupName("Add Override");
					AddComponent<Vector3PropertyOverride>();
				}
				if (GUILayout.Button("v4", EditorStyles.miniButtonRight))
				{
					Undo.SetCurrentGroupName("Add Override");
					AddComponent<Vector4PropertyOverride>();
				}
			}*/
			var i = GUILayout.SelectionGrid(-1, Labels, 3);
			if (i > -1)
			{
				AddComponent(Types[i]);
			}
		}

		private void AddComponent(System.Type type)
		{
			foreach (var t in targets)
			{
				var pom = t as PropertyOverrideManager;
				if (pom != null)
				{
					Undo.AddComponent(pom.gameObject, type);
				}
			}
		}
	}
}