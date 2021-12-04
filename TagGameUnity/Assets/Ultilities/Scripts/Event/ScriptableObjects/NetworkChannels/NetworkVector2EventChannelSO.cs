using UnityEngine;
using UnityEngine.Events;

namespace Ultility.Event.Network{

	[CreateAssetMenu(menuName = "Events/Network/ Vector2 Event Channel")]
	public class NetworkVector2EventChannelSO : DescriptionBaseSO
	{
        /// <summary>
        /// receive userId, local time, value
        /// </summary>
		public UnityAction<ulong, double, Vector2> OnEventRaised;
		
		public void RaiseEvent(ulong userId, double localTime, Vector2 value)
		{
			if (OnEventRaised != null)
				OnEventRaised.Invoke(userId, localTime, value);
		}
	}

}

