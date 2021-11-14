using UnityEngine;
using UnityEngine.Events;

namespace Ultility.Event{
	/// <summary>
	/// This class is used for Events that have one string argument.
	/// Example: An Achievement unlock event, where the ulong is the Achievement ID.
	/// </summary>

	[CreateAssetMenu(menuName = "Events/String Event Channel")]
	public class StringEventChannelSO : DescriptionBaseSO
	{
		public UnityAction<string> OnEventRaised;
		
		public void RaiseEvent(string value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(value);
		}
	}

}

