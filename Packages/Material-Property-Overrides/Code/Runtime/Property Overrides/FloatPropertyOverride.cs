using UnityEngine;

namespace MPO
{
	public class FloatPropertyOverride : PropertyOverride<float>
	{
		public override PropertyType PropertyType => PropertyType.Number;
		
		public override void Apply(MaterialPropertyBlock mpb)
		{
			mpb.SetFloat(id, Value);
		}
	}
}