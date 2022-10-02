using UnityEngine;

namespace MPO
{
	public class Vector2PropertyOverride : PropertyOverride<Vector2>
	{
		public override PropertyType PropertyType => PropertyType.Vector;
		
		public override void Apply(MaterialPropertyBlock mpb)
		{
			mpb.SetVector(id, Value);			
		}
	}
}