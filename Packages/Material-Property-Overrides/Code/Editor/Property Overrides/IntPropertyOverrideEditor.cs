using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPO.Editor
{
	[CustomEditor(typeof(IntPropertyOverride))]
	public class IntPropertyOverrideEditor : PropertyOverrideEditor
	{
		private List<Material> materials = new List<Material>();

		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();

			DrawIDField();

			if (serializedObject.isEditingMultipleObjects)
				EditorGUILayout.PropertyField(valueProperty);
			else
			{
				var fpo = target as IntPropertyOverride;

				if (fpo != null)
				{
					int? min = null;
					int? max = null;

					bool multipleRanges = false;

					fpo.Manager.TargetRenderer.GetSharedMaterials(materials);
					foreach (var m in materials)
					{
						var propertyCount = ShaderUtil.GetPropertyCount(m.shader);
						for (int i = 0; i < propertyCount; i++)
						{
							if (ShaderUtil.GetPropertyName(m.shader, i) == fpo.id.Name)
							{
								if (ShaderUtil.GetPropertyType(m.shader, i) == ShaderUtil.ShaderPropertyType.Range)
								{
									int newMin = (int)ShaderUtil.GetRangeLimits(m.shader, i, 1);
									int newMax = (int)ShaderUtil.GetRangeLimits(m.shader, i, 2);

									if (min != null && min.Value != newMin)
									{
										multipleRanges = true;
										break;
									}

									if (max != null && max.Value != newMax)
									{
										multipleRanges = true;
										break;
									}

									min = newMin;
									max = newMax;
								}
							}
						}

						if (multipleRanges)
							break;
					}

					if (multipleRanges || min == null || !min.HasValue || max == null || !max.HasValue)
						EditorGUILayout.PropertyField(valueProperty);
					else
					{
						EditorGUILayout.IntSlider(valueProperty, min.Value, max.Value);
					}
				}
			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}