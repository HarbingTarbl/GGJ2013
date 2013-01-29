using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Memory
{
	public static class SoundWrapper
	{
		public static void PlayDialog (string assetName)
		{
			if (currentDialog != null)
			{
				currentDialog.Stop();
				currentDialog.Dispose();
			}

			currentDialog = G.C.Load<SoundEffect> ("sfx/dialog/" + assetName).CreateInstance();
			currentDialog.Play();
		}

		private static SoundEffectInstance currentDialog;
	}
}
