using System;
using UnityEngine;

namespace PG_Atributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowInInspectorIf : PropertyAttribute
	{
		public ShowInInspectorIf(string conditionalStr)
		{
			this.ConditionalStr = conditionalStr;
		}

		public string ConditionalStr;
	}
}
