using UnityEngine;
using UnityEngine.Events;

namespace Ultility.Event{
	/// <summary>
	/// This class is used for Events that have one ulong argument.
	/// Example: An Achievement unlock event, where the ulong is the Achievement ID.
	/// </summary>

	[CreateAssetMenu(menuName = "Events/Ulong Event Channel")]
	public class UlongEventChannelSO : DescriptionBaseSO
	{
		public UnityAction<ulong> OnEventRaised;
		
		public void RaiseEvent(ulong value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(value);
		}
	}

}

