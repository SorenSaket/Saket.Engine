using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Saket.Navigation;

namespace Saket.Navigation.VectorField
{
    public class AffectorHandler : IDataListProvider<AffectorInstance>
	{
		[NonSerialized]
		public List<AffectorInstance> Affectors;

		public AffectorHandler()
		{
			Affectors = new List<AffectorInstance>();
		}

		public Action<List<AffectorInstance>> OnDataChanged { get; set; }
		public Action<AffectorInstance> OnItemChanged { get; set; }

		public bool AddAffector(VectorFieldAffector affector)
		{
			if (Affectors.Exists((x) => x.Affector == affector))
				return false;
			
			AffectorInstance af = new AffectorInstance(affector);
			af.Affector.OnValuesChanged += () => { OnItemChanged(af);  };
			
			OnItemChanged?.Invoke(af);

			Affectors.Add(af);

			OnDataChanged?.Invoke(Affectors);
			return true;
		}

		public List<AffectorInstance> GetData()
		{
			return Affectors;
		}

		public void RegenerateData()
		{
			
		}
	}
}