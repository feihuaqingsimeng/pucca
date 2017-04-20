using UnityEngine;
using System.Collections;

namespace DocsPin.UI
{
	/* If you use NGUI, delete this comment.
	[RequireComponent(typeof(UILabel))]
	*/
	public class NGUIBool : DocsPin.UI.DocsUIBase
	{
		protected override void setValue(object value)
		{
			/* If you use NGUI, delete this comment.
			UILabel label = this.gameObject.GetComponent<UILabel>();

			bool show = false;
			if(label != null || value != null)
			{
				if((value is bool) == true || (value is esLibs.Detector.Type.mBool) == true)
					show = (bool)value;
			}
			NGUITools.SetActive(label.gameObject, show);
			*/
		}
	}
}