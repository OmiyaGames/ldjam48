using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class GlobalTimelineController : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		public enum Key : byte
		{
			None,
			Play,
			Rewind
		}

		bool isPlayKeyDown = false, isRewindKeyDown = false;

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public bool IsPlayKeyDown
		{
			get => isPlayKeyDown;
			private set
			{
				if(isPlayKeyDown != value)
				{
					isPlayKeyDown = value;
					if(isPlayKeyDown == true)
					{
						LastKeyPressed = Key.Play;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsRewindKeyDown
		{
			get => isRewindKeyDown;
			private set
			{
				if(isRewindKeyDown != value)
				{
					isRewindKeyDown = value;
					if(isRewindKeyDown == true)
					{
						LastKeyPressed = Key.Rewind;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Key LastKeyPressed
		{
			get;
			private set;
		} = Key.None;
		#endregion

		#region Input Actions
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctx"></param>
		public void OnPlay(InputAction.CallbackContext ctx)
		{
			IsPlayKeyDown = ctx.performed;
			UpdateTimelines();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctx"></param>
		public void OnRewind(InputAction.CallbackContext ctx)
		{
			IsRewindKeyDown = ctx.performed;
			UpdateTimelines();
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		void UpdateTimelines()
		{
			// Default to paused
			ControllableTimeline.GlobalState = ControllableTimeline.PlayState.Paused;

			// Check if script is enabled, and any key is down
			if(enabled && (IsPlayKeyDown || IsRewindKeyDown))
			{
				// Grab the last key, and update state accordingly
				ControllableTimeline.GlobalState = ControllableTimeline.PlayState.Playing;
				if(LastKeyPressed == Key.Rewind)
				{
					ControllableTimeline.GlobalState = ControllableTimeline.PlayState.Rewinding;
				}
			}
		}
	}
}
