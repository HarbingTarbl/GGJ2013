﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammy;
using Jammy.Collision;
using Jammy.Helpers;
using Jammy.Sprites;
using Jammy.StateManager;
using Memory.Collision;
using Memory.Items;
using Memory.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Memory
{
	public class TentState
		: MemoryState
	{
		public TentState()
			: base ("Tent")
		{
			Background = G.C.Load<Texture2D> ("TentArea/background");
			#region NavMesh
			var p1 = new Polygon (
				new Vector2 (106, 469),
				new Vector2 (152, 442),
				new Vector2 (420, 433),
				new Vector2 (777, 429),
				new Vector2 (1000, 443),
				new Vector2 (997, 480),
				new Vector2 (896, 594),
				new Vector2 (627, 646),
				new Vector2 (345, 644),
				new Vector2 (159, 605));

			var p1n = new PolyNode (p1);

			Nav = new List<PolyNode> { p1n };
			#endregion

			Blanket = CreateItem ("Blanket", "A warm blanket", "TentArea/blanket", 200, 500,
				new Vector2 (0, 41),
				new Vector2 (256, -6),
				new Vector2 (419, 91),
				new Vector2 (296, 153));

			Flash = CreateItem ("Flashlight", "A dead flashlight, it's missing batteries", "TentArea/flashlight","UI/Icons/flashlight_off", 587, 436,
				new Vector2 (568 - 587, 493 - 436),
				new Vector2 (575 - 587, 427 - 436),
				new Vector2 (630 - 587, 433 - 436),
				new Vector2 (634 - 587, 484 - 436));

			Sweater = CreateItem("Sweater", "A Sweater", "TentArea/sweater", "UI/Icons/sweater", 390, 550,
				new Vector2 (426 - 500, 563 - 550),
				new Vector2 (513 - 500, 551 - 550),
				new Vector2 (653 - 500, 559 - 550),
				new Vector2 (639 - 500, 594 - 550),
				new Vector2 (566 - 500, 615 - 550),
				new Vector2 (496 - 500, 603 - 550));

			Matches = CreateItem ("Matches", "A set of matches", "TentArea/matches", "UI/Icons/matches", 850, 500,
				new Vector2 (830 - 850, 505 - 500),
				new Vector2 (892 - 850, 491 - 500),
				new Vector2 (905 - 850, 516 - 500),
				new Vector2 (844 - 850, 532 - 500));

			Blanket.OnClick += t =>
			{
				Blanket.IsActive = false;
				Blanket.Texture = G.C.Load<Texture2D> ("TentArea/tossedblanket");
				Sweater.IsActive = true;
				G.C.Load<SoundEffect> ("sfx/Blanket").Play();
			};

			Sweater.OnClick += s => G.C.Load<SoundEffect> ("sfx/Sweater").Play();
			Matches.OnClick += s => SoundWrapper.PlayDialog ("Matches");
			Flash.OnClick += s => SoundWrapper.PlayDialog ("Flashlight");

			Matches.IsActive = false;
			Matches.IsVisible = false;
			Blanket.IsActive = true;
			Blanket.CanPickup = false;
			Sweater.CanPickup = true;
			Flash.IsActive = true;
			Flash.CanPickup = true;

			Bag = new Hotspot (
				"Duffle Bag",
				new Polygon (
					new Vector2 (757, 443),
					new Vector2 (883, 417),
					new Vector2 (902, 489),
					new Vector2 (776, 507),
					new Vector2 (745, 483)
					), (t, i) =>
					   {
						   if (BagOpened)
							   return;

						   Player.AnimationManager.SetAnimation ("Kick");
						   Matches.IsVisible = true;
						   Matches.IsActive = true;
						   Matches.CanPickup = true;
						   BagOpened = true;
					   }) { WalkLocation = new Vector2 (740, 482) };

			Exit = new Hotspot(
				"Tent Exit",
				new Polygon(
					new Vector2(620 + 90, 250),
					new Vector2(607 + 90, 373),
					new Vector2(575 + 90, 459),
					new Vector2(650 + 90, 468),
					new Vector2(675 + 90, 374),
					new Vector2(657 + 90, 299)), (t,i) =>
					{
						if (CanLeaveLevel)
						{
							G.C.Load<SoundEffect> ("sfx/Zipper").Play ();
							G.FadeOut.Finished = () => {
								G.FadeOut.Reset();
								G.LastScreen = "Tent";
								G.StateManager.Set ("Camp");
								G.FadeIn.TriggerStart();
							};
							G.FadeOut.TriggerStart();
						}
						else
						{
							SoundWrapper.PlayDialog ("Cold and Dark");
							G.DialogManager.PostMessage ("I should put on some clothes... and grab my flashlight.", TimeSpan.Zero, new TimeSpan (0, 0, 5));
						}
					}) { WalkLocation = new Vector2(706, 486)};

			ItemsToLeave.Add("Sweater");
			ItemsToLeave.Add("Flashlight");

			lantern = CreateSprite ("TentArea/lantern", 510, 89);
			light1 = CreateSprite ("TentArea/light1");
			light2 = CreateSprite ("TentArea/light2");
			glow = CreateSprite ("TentArea/lanternGlow", 370, 140);
			border = CreateSprite ("TentArea/border");

			LanternSpot = new Hotspot("Unlit Lantern",
				new Circlegon(545, 315, 64), (t,i) =>
			{
				if (light2.IsVisible)
					return;

				G.C.Load<SoundEffect> ("sfx/Lantern").Play();
				Player.AnimationManager.SetAnimation ("Wake Up");
				LanternSpot.Name = "Lit Lantern";
				light1.IsVisible = false;
				light2.IsVisible = true;
				//glow.IsVisible = true;
			}) { WalkLocation = new Vector2 (596, 546) };

			light1.IsVisible = true;
			light2.IsVisible = false;
			glow.IsVisible = false;

			Items.Add (Flash);
			Items.Add (Sweater);
			Items.Add (Matches);
			Items.Add (Blanket);

			Lights.Add (lantern);
			Lights.Add (glow);
			Lights.Add (border);
			Lights.Add (light1);
			Lights.Add (light2);

			Hotspots.Add (Exit);
			Hotspots.Add (LanternSpot);
			Hotspots.Add (Bag);
		}

		private Sprite lantern;
		private Sprite light1;
		private Sprite light2;
		private Sprite glow;
		private Sprite border;

		public GameItem Sweater;
		public GameItem Blanket;
		public GameItem Flash;
		public GameItem Matches;

		public Hotspot LanternSpot;
		public Hotspot Exit;
		public Hotspot Bag;
		private bool BagOpened;

		protected override void OnLevelStart (string lastScreen)
		{
			MediaPlayer.Stop();
			switch (lastScreen)
			{
				case "None":
					SoundWrapper.PlayDialog ("Intro");
					Player.Location = new Vector2 (440, 495);
					break;
				case "Camp":
					G.Player.Location = new Vector2(696, 511);
					G.C.Load<SoundEffect> ("sfx/Zipper").Play();
					MediaPlayer.Volume = 0.25f;
					MediaPlayer.IsRepeating = true;
					break;
			}

			base.OnLevelStart (lastScreen);
		}
	}
}
