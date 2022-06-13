using System;
using UnityEngine;

namespace PG_Atributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class HideInInspectorIf : PropertyAttribute
	{
		public HideInInspectorIf(string conditionalStr)
		{
			this.ConditionalStr = conditionalStr;
		}

		public string ConditionalStr;
	}
}
