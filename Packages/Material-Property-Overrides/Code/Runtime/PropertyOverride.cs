using UnityEngine;

namespace MPO
{
	[ExecuteAlways, RequireComponent(typeof(PropertyOverrideManager)), DefaultExecutionOrder(PropertyOverrideManager.ExecutionOrder + 1)]
	public abstract class PropertyOverride : MonoBehaviour
	{
		public PropertyID id = new PropertyID();

		private PropertyOverrideManager manager;
		
		public abstract PropertyType PropertyType { get; }

		public PropertyOverrideManager Manager
		{
			get
			{
				if (manager == null)
					TryGetComponent(out manager);
				return manager;
			}
		}

		protected virtual void OnEnable()
		{
			Manager.Register(this);
			MarkAsNeedsToReapply();
		}

		private void OnValidate()
		{
			MarkAsNeedsToReapply();
		}

		protected virtual void OnDisable()
		{
			Manager.Unregister(this);
		}

		public void MarkAsNeedsToReapply()
		{
			Manager.ApplyAllOverrides();
		}
		
		public abstract void Apply(MaterialPropertyBlock mpb);
	}
	
	public abstract class PropertyOverride<T> : PropertyOverride
	{
		[SerializeField]
		private T value;

		public T Value
		{
			get => value;
			set
			{
				this.value = value;
				MarkAsNeedsToReapply();
			}
		}
	}
}