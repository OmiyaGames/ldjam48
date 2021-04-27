using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class InteractiveTimelineObject : MonoBehaviour
	{
		[SerializeField]
		ControllableTimeline parentTimeline;

		/// <summary>
		/// Pauses the parent timeline.
		/// </summary>
		public bool IsPaused
		{
			get
			{
				if(parentTimeline != null)
				{
					return parentTimeline.OverrideState.HasValue;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if((parentTimeline != null) && (value != parentTimeline.OverrideState.HasValue))
				{
					if(value)
					{
						parentTimeline.OverrideState = ControllableTimeline.PlayState.Paused;
					}
					else
					{
						parentTimeline.OverrideState = null;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void Reset()
		{
			// Go through the parents and find controllableTimeline script
			ControllableTimeline scriptCache;
			Transform checkForScript = transform;
			while(checkForScript != null)
			{
				// Attempt to grab the script from here
				scriptCache = checkForScript.GetComponent<ControllableTimeline>();
				if(scriptCache != null)
				{
					// If found, set member variable and halt loop
					parentTimeline = scriptCache;
					break;
				}

				// Go up the parent
				checkForScript = checkForScript.parent;
			}
		}
	}
}
