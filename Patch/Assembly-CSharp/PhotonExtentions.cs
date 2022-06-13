using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

public static class PhotonExtentions
{
	public static void SetCustomProperties(this Player player, object key, object value)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add(key, value);
		player.SetCustomProperties(hashtable, null, null);
	}

	public static void SetCustomProperties(this Player player, params object[] args)
	{
		Hashtable hashtable = new Hashtable();
		for (int i = 0; i < args.Length; i += 2)
		{
			hashtable.Add(args[i], args[i + 1]);
		}
		player.SetCustomProperties(hashtable, null, null);
	}
}
