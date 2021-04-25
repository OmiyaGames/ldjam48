using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class IInteractable : MonoBehaviour
	{
		/// <summary>
		/// 
		/// </summary>
		public enum HoverIcon
		{
			Interact,
			Error,
			Info
		}

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class HoverInfo
		{
			public HoverIcon displayIcon;
			public string displayInstructions;
		}

		/// <summary>
		/// Activates on hover.
		/// </summary>
		/// <returns>True if something happens.</returns>
		public abstract bool OnHover(Inventory source, out HoverInfo info);
		/// <summary>
		/// Activates on mouse up.
		/// </summary>
		/// <returns>True if something happens.</returns>
		public abstract bool OnClick(Inventory source);
	}
}
