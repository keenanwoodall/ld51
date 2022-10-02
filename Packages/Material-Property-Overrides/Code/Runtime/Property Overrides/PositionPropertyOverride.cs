using UnityEngine;

namespace MPO
{
	public class PositionPropertyOverride : PropertyOverride<Transform>
	{
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
				mpb.SetVector(id, Value.position);
			}
		}
	}
}