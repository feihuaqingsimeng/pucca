using UnityEngine;
using System.Collections;

namespace DocsPin.UI
{
	/* If you use NGUI, delete this comment.
	[RequireComponent(typeof(UILabel))]
	*/
	public class NGUIText : DocsPin.UI.DocsUIBase
	{
		protected override void setValue(object value)
		{
			/* If you use NGUI, delete this comment.
			UILabel label = this.gameObject.GetComponent<UILabel>();
			if(label == null)
				return;
			if(value == null)
				label.text = "";
			else
				label.text = value.ToString();
			*/
		}
	}
}
