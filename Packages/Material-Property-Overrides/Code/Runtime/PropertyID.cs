using UnityEngine;

namespace MPO
{
	[System.Serializable]
	public class PropertyID
	{
		[SerializeField]
		private string name;
		[SerializeField]
		private int id;

		public string Name
		{
			get => name;
			set
			{
				name = value;
				id = Shader.PropertyToID(name);
			}
		}

		public int Id
		{
			get => id;
		}

		public static implicit operator int(PropertyID id) => id.Id;
	}
}