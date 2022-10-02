using UnityEngine;

namespace MPO
{
	public class Vector3PropertyOverride : PropertyOverride<Vector3>
	{
		public override PropertyType PropertyType => PropertyType.Vector;

		public override void Apply(MaterialPropertyBlock mpb)
		{
			mpb.SetVector(id, Value);			
		}
	}
}