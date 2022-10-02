using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MPO.Editor
{
	[CustomEditor(typeof(PropertyOverride), true), CanEditMultipleObjects]
	public class PropertyOverrideEditor : UnityEditor.Editor
	{
		public static List<string> SuggestedPropertyNames = new List<string>();
		public static List<int> SuggestedPropertyIDs = new List<int>();

		protected SerializedProperty idProperty;
		protected SerializedProperty valueProperty;


		protected virtual void OnEnable()
		{
			idProperty = serializedObject.FindProperty("id");
			valueProperty = serializedObject.FindProperty("value");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			
			DrawIDField();
			DrawPropertiesExcluding(serializedObject, "id", "m_Script");
			
			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawIDField()
		{
			if (!serializedObject.isEditingMultipleObjects)
			{
				var po = target as PropertyOverride;

				if (po != null)
				{
					var idRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
					var searchRect = idRect;
					idRect.xMax -= EditorGUIUtility.singleLineHeight + 1;
					searchRect.xMin = searchRect.xMax - EditorGUIUtility.singleLineHeight;

					EditorGUI.PropertyField(idRect, idProperty);

					SuggestedPropertyNames.Clear();
					SuggestedPropertyIDs.Clear();

					UpdateSuggestedPropertyCollections(po);

					if (GUI.Button(searchRect, "", EditorStyles.miniPullDown))
					{
						var popupActivatorRect = idRect;
						popupActivatorRect.xMin = popupActivatorRect.xMax - SearchablePopup.POPUP_WIDTH + EditorGUIUtility.singleLineHeight;
						SearchablePopup.Show(popupActivatorRect,
							SuggestedPropertyNames.ToArray(), -1,
							i =>
							{
								Undo.RecordObject(po, "Changed ID");
								po.id.Name = SuggestedPropertyNames[i];
								po.MarkAsNeedsToReapply();
							});
					}
				}
			}
			else
			{
				EditorGUILayout.PropertyField(idProperty);
			}
		}

		public static List<Material> CurrentMaterials = new List<Material>();
		private void UpdateSuggestedPropertyCollections(PropertyOverride po)
		{
			po.Manager.TargetRenderer.GetSharedMaterials(CurrentMaterials);
			foreach (var material in CurrentMaterials)
			{
				var shader = material.shader;
				var propertyCount = shader.GetPropertyCount();

				for (int i = 0; i < propertyCount; i++)
				{
					ShaderPropertyType t = shader.GetPropertyType(i);

					PropertyType currentType = 0;

					switch (t)
					{
						case ShaderPropertyType.Float:
						case ShaderPropertyType.Range:
							currentType = PropertyType.Number;
							break;
						case ShaderPropertyType.Vector:
							currentType = PropertyType.Vector;
							break;
						case ShaderPropertyType.Color:
							currentType = PropertyType.Color;
							break;
						case ShaderPropertyType.Texture:
							currentType = PropertyType.Texture;
							break;
					}
							
					PropertyType requiredType = po.PropertyType;

					if ((currentType & requiredType) > 0)
					{
						SuggestedPropertyIDs.Add(shader.GetPropertyNameId(i));
						SuggestedPropertyNames.Add(shader.GetPropertyName(i));
					}
				}
			}
		}
	}
}