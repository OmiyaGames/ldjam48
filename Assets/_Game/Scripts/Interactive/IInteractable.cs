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
		public class HoverInfo
		{
			/// <summary>
			/// 
			/// </summary>
			public string DisplayString
			{
				get;
				set;
			}
		}

		/// <summary>
		/// Activates on hover.
		/// </summary>
		/// <returns>True if something happens.</returns>
		public abstract bool OnHover(out HoverInfo info);
		/// <summary>
		/// Activates on mouse up.
		/// </summary>
		/// <returns>True if something happens.</returns>
		public abstract bool OnClick();
	}
}
