using UnityEngine;
using System.Collections;

// Unity GUI Text.
namespace DocsPin.UI
{
	[RequireComponent(typeof(UnityEngine.UI.Text))]
	public class uGUIText : DocsPin.UI.DocsUIBase
	{
		protected override void setValue(object value)
		{
			UnityEngine.UI.Text text = this.gameObject.GetComponent<UnityEngine.UI.Text>();
			if(text == null)
				return;
			if(value == null)
				text.text = "";
			else
				text.text = value.ToString();
		}
	}
}
