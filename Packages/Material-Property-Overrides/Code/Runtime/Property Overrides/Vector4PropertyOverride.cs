using UnityEngine;

namespace MPO
{
	public class Vector4PropertyOverride : PropertyOverride<Vector4>
	{
		public override PropertyType PropertyType => PropertyType.Vector;

		public override void Apply(MaterialPropertyBlock mpb)
		{
			mpb.SetVector(id, Value);			
		}
	}
}