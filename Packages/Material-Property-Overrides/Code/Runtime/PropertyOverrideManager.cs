using System.Collections.Generic;
using UnityEngine;

namespace MPO
{
    [RequireComponent(typeof(Renderer)), ExecuteAlways, DefaultExecutionOrder(ExecutionOrder)]
    public class PropertyOverrideManager : MonoBehaviour
    {
        public const int ExecutionOrder = 1;

        private HashSet<PropertyOverride> propertyOverrides = new HashSet<PropertyOverride>();
        private List<Material> materials = new List<Material>();
        
        private MaterialPropertyBlock mpb;

        public MaterialPropertyBlock MPB
        {
            get
            {
                if (mpb == null)
                    mpb = new MaterialPropertyBlock();
                return mpb;
            }
        }

        private Renderer targetRenderer;
        public Renderer TargetRenderer
        {
            get
            {
                if (targetRenderer == null)
                    TryGetComponent(out targetRenderer);
                return targetRenderer;
            }
        }

        public void Register(PropertyOverride propertyOverride)
        {
            propertyOverrides.Add(propertyOverride);
            ApplyAllOverrides();
        }
        public void Unregister(PropertyOverride propertyOverride)
        {
            propertyOverrides.Remove(propertyOverride);
            ApplyAllOverrides();
        }

        public void ApplyAllOverrides()
        {
            TargetRenderer.GetSharedMaterials(materials);
            
            MPB.Clear();

            foreach (var property in propertyOverrides)
            {
                property.Apply(MPB);
            }
            
            targetRenderer.SetPropertyBlock(MPB);
        }
    }
}