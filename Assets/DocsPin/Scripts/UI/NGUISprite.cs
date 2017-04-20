using UnityEngine;
using System.Collections;

namespace DocsPin.UI
{
	/* If you use NGUI, delete this comment.
	[RequireComponent(typeof(UISprite))]
	*/
	public class NGUISprite : DocsPin.UI.DocsUIBase
	{
		protected override void setValue(object value)
		{
			/* If you use NGUI, delete this comment.
			UISprite sprite = this.gameObject.GetComponent<UISprite>();
			if(sprite == null || value == null)
				return;
			sprite.spriteName = value.ToString();
			*/
		}
	}
}
