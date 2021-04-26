using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayOnHoverDoor : MonoBehaviour
	{
		[SerializeField]
		Door onHover;
		[SerializeField]
		PlayableDirector timeline;

		/// <summary>
		/// Start is called before the first frame update
		/// </summary>
		void Start()
		{
			onHover.OnHovered += OnHover_OnHovered;
		}

		/// <summary>
		/// Play animation
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		private void OnHover_OnHovered(Door arg1, IInteractable.HoverInfo arg2)
		{
			if(arg2.displayIcon == IInteractable.HoverIcon.Interact)
			{
				timeline.Play();
				onHover.OnHovered -= OnHover_OnHovered;
			}
		}
	}
}
