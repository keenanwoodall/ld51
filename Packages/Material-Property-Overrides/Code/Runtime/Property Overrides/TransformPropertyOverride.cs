using System;
using UnityEngine;

namespace MPO
{
	public class TransformPropertyOverride : PropertyOverride<Transform>
	{
		public enum Transformation { WorldToLocal, LocalToWorld }
		public Transformation transformation = Transformation.WorldToLocal;

		public override PropertyType PropertyType => PropertyType.Matrix;
		
		private void LateUpdate()
		{
			if (Value != null && Value.hasChanged)
			{
				MarkAsNeedsToReapply();
				Value.hasChanged = false;
			}
		}

		public override void Apply(MaterialPropertyBlock mpb)
		{
			if (Value != null)
			{
				switch (transformation)
				{
					case Transformation.WorldToLocal:
						mpb.SetMatrix(id, Value.worldToLocalMatrix);
						break;
					case Transformation.LocalToWorld:
						mpb.SetMatrix(id, Value.localToWorldMatrix);
						break;
				}
			}
		}
	}
}