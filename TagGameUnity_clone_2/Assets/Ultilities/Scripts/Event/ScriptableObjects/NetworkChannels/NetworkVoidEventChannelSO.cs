using UnityEngine;
using UnityEngine.Events;

namespace Ultility.Event.Network{

	[CreateAssetMenu(menuName = "Events/Network/ Void Event Channel")]
	public class NetworkVoidEventChannelSO : DescriptionBaseSO
	{
        /// <summary>
        /// receive userId, local time, value
        /// </summary>
		public UnityAction<ulong, double> OnEventRaised;
		
		public void RaiseEvent(ulong userId, double localTime)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(userId, localTime);
		}
	}

}

