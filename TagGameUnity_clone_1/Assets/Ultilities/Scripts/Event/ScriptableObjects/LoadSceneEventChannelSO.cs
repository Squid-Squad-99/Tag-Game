
using UnityEngine;
using UnityEngine.Events;

using Ultility.Scene;

namespace Ultility.Event{

	/// <summary>
	/// This class is used for Load Scene event
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Load Scene Event Channel")]
	public class LoadSceneEventChannelSO : DescriptionBaseSO
	{
		/// <summary>
		///  passing the scene SO to be load and whether want loading scene
		/// </summary>
		public UnityAction<SceneSO, bool> OnEventRaised;

		public void RaiseEvent(SceneSO sceneSO, bool withLoadingScene)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(sceneSO, withLoadingScene);
		}
	}

}
