using System;
using UnityEngine;

namespace MPO
{
	public class DirectionPropertyOverride : PropertyOverride<Transform>
	{
		public enum Axis
		{
			[InspectorName("X+")]
			PositiveX,
			[InspectorName("Y+")]
			PositiveY,
			[InspectorName("Z+")]
			PositiveZ,
			[InspectorName("X-")]
			NegativeX,
			[InspectorName("Y-")]
			NegativeY,
			[InspectorName("Z-")]
			NegativeZ
		}
		public Axis axis = Axis.PositiveZ;

		public override PropertyType PropertyType => PropertyType.Vector;
		
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
				switch (axis)
				{
					case Axis.PositiveX:
						mpb.SetVector(id, Value.right);
						break;
					case Axis.PositiveY:
						mpb.SetVector(id, Value.up);
						break;
					case Axis.PositiveZ:
						mpb.SetVector(id, Value.forward);
						break;
					case Axis.NegativeX:
						mpb.SetVector(id, -Value.right);
						break;
					case Axis.NegativeY:
						mpb.SetVector(id, -Value.up);
						break;
					case Axis.NegativeZ:
						mpb.SetVector(id, -Value.forward);
						break;
				}
			}
		}
	}
}