using UnityEngine;

namespace MPO
{
	public class IntPropertyOverride : PropertyOverride<int>
	{
		public override PropertyType PropertyType => PropertyType.Number;
		
		public override void Apply(MaterialPropertyBlock mpb)
		{
			mpb.SetInt(id, Value);
		}
	}
}