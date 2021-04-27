using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public class Remote : IInteractable
	{
		[SerializeField]
		GlobalTimelineController adjustController;
		[SerializeField]
		HoverInfo collectInfo = new HoverInfo()
		{
			displayIcon = HoverIcon.Interact,
			displayInstructions = "Collect remote"
		};
		[SerializeField]
		GameObject[] activateOnClick;

		/// <inheritdoc/>
		public override bool OnClick(Inventory source)
		{
			// Activate ALL the powers
			adjustController.IsPlayKeyEnabled = true;
			adjustController.IsRewindKeyEnabled = true;

			// Destroy this collectable
			Destroy(gameObject);

			// Activate
			foreach(var activate in activateOnClick)
			{
				activate.SetActive(true);
			}
			return true;
		}

		/// <inheritdoc/>
		public override bool OnHover(Inventory source, out HoverInfo info)
		{
			info = collectInfo;
			return true;
		}
	}
}