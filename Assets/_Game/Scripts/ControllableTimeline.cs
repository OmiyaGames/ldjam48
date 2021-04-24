using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(PlayableDirector))]
	public class ControllableTimeline : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		public enum PlayState : byte
		{
			/// <summary>
			/// 
			/// </summary>
			Paused,
			/// <summary>
			/// 
			/// </summary>
			Playing,
			/// <summary>
			/// 
			/// </summary>
			Rewinding
		}

		[SerializeField]
		float warmUpDuration = 0.25f;

		static PlayState globalState = PlayState.Paused;
		PlayableDirector timelineCache;
		float warmUpProgress = 0;

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public static PlayState GlobalState
		{
			get
			{
				if(ActiveControllers.Count == 0)
				{
					return PlayState.Paused;
				}
				else
				{
					return globalState;
				}
			}
			set => globalState = value;
		}
		/// <summary>
		/// 
		/// </summary>
		static HashSet<ControllableTimeline> ActiveControllers
		{
			get;
		} = new HashSet<ControllableTimeline>();
		/// <summary>
		/// 
		/// </summary>
		public PlayableDirector Director
		{
			get => OmiyaGames.Helpers.GetComponentCached(this, ref timelineCache);
		}
		/// <summary>
		/// 
		/// </summary>
		public PlayState? OverrideState
		{
			get;
			set;
		} = null;
		/// <summary>
		/// 
		/// </summary>
		public PlayState State
		{
			get
			{
				if(OverrideState.HasValue)
				{
					return OverrideState.Value;
				}
				else
				{
					return GlobalState;
				}
			}
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		void Start()
		{
			ActiveControllers.Add(this);
		}

		/// <summary>
		/// 
		/// </summary>
		void OnDestroy()
		{
			ActiveControllers.Remove(this);
		}

		/// <summary>
		/// 
		/// </summary>
		void FixedUpdate()
		{
			double newTime = Director.time;
			switch(State)
			{
				case PlayState.Playing:
					newTime += getTimeIncrement();
					if(newTime > Director.playableAsset.duration)
					{
						newTime = Director.playableAsset.duration;
					}
					SetTimeline(newTime);
					break;
				case PlayState.Rewinding:
					newTime -= getTimeIncrement();
					if(newTime < 0)
					{
						newTime = 0;
					}
					SetTimeline(newTime);
					break;
				case PlayState.Paused:
					warmUpProgress = 0;
					break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		double getTimeIncrement()
		{
			// Default returnTime to delta time
			double returnTime = Time.deltaTime;

			// Track how much time has passed
			warmUpProgress += Time.deltaTime;
			if(warmUpProgress < warmUpDuration)
			{
				// If still warming up, lerp the progress we're making
				returnTime *= (warmUpProgress / warmUpDuration);
			}
			return returnTime;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newTime"></param>
		void SetTimeline(double newTime)
		{
			Director.time = newTime;
			Director.Evaluate();
		}
	}
}
