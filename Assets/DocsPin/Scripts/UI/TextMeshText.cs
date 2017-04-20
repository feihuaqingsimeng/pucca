using UnityEngine;
using System.Collections;

// TextMeshPro Text.
namespace DocsPin.UI
{
	/* If you use TextMeshPro, delete this comment.
	[RequireComponent(typeof(TMPro.TextMeshPro))]
	*/
	public class TextMeshText : DocsPin.UI.DocsUIBase
	{
		protected override void setValue(object value)
		{
			/* If you use TextMeshPro, delete this comment.
			TMPro.TextMeshPro textMesh = this.gameObject.GetComponent<TMPro.TextMeshPro>();
			if(textMesh == null)
				return;

			if(value == null)
				textMesh.text = "";
			else
				textMesh.text = value.ToString();
			*/
		}
	}
}
