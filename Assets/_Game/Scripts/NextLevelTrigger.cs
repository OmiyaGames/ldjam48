using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames.Global;

namespace LD48
{
	[RequireComponent(typeof(Door))]
	public class NextLevelTrigger : MonoBehaviour
	{
		/// <summary>
		///  Start is called before the first frame update
		/// </summary>
		void Start()
		{
			// Grab the attached door
			var door = GetComponent<Door>();

			// Listen to door event
			door.OnOpenChanged += Door_OnOpenChanged;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		private void Door_OnOpenChanged(Door arg1, bool arg2)
		{
			// Check if door opened
			if(arg2 == true)
			{
				// Stop listening to the event
				arg1.OnOpenChanged -= Door_OnOpenChanged;

				// Transition to the next scene
				var manager = Singleton.Get<OmiyaGames.Scenes.SceneTransitionManager>();
				manager.LoadNextLevel();
			}
		}
	}
}
