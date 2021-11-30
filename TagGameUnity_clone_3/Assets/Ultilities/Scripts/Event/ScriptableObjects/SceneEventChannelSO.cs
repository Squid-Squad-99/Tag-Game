
using UnityEngine;
using UnityEngine.Events;

using Ultility.Scene;

namespace Ultility.Event{

	/// <summary>
	/// This class is used for Events have sceneSO as agument
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Scene Event Channel")]
	public class SceneEventChannelSO : DescriptionBaseSO
	{
		public UnityAction<SceneSO> OnEventRaised;

		public void RaiseEvent(SceneSO sceneSO)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(sceneSO);
		}
	}

}
