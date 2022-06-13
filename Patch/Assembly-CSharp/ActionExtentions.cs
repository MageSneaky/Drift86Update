using System;

public static class ActionExtentions
{
	public static void SafeInvoke(this Action action)
	{
		if (action != null)
		{
			action();
		}
	}

	public static void SafeInvoke<P>(this Action<P> action, P p)
	{
		if (action != null)
		{
			action(p);
		}
	}

	public static void SafeInvoke<P1, P2>(this Action<P1, P2> action, P1 p1, P2 p2)
	{
		if (action != null)
		{
			action(p1, p2);
		}
	}
}
