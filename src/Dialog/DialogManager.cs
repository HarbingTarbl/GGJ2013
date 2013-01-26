using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2013.Dialog
{
	public class DialogManager
	{
		public DialogManager()
		{
			MessageQueue = new List<Message>();
			
		}

		public void Update(GameTime gameTime)
		{
			for (var i = 0; i < MessageQueue.Count; i++ )
			{
				var msg = MessageQueue[i];
				if (!msg.IsShown)
					msg.TimeUntil -= gameTime.ElapsedGameTime;
				else
				{
					msg.Duration -= gameTime.ElapsedGameTime;
					msg.FadeAmount = MathHelper.Clamp((FadeTime - (float)msg.Duration.TotalSeconds) / FadeTime, 0f, 1f);
					if ((int)msg.Duration.TotalMilliseconds <= 0)
					{
						MessageQueue.RemoveAt(i);
						i--;
					}
				}
			}

			
		}

		public void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			var drawLocation = new Vector2(MessageBounds.X + SidePadding, MessageBounds.Y + LinePadding);
			foreach (var msg in MessageQueue)
			{
				if (msg.IsShown)
				{
					for (var i = 0; i < msg.Lines.Length; i++)
					{
						batch.DrawString(Font, msg.Lines[i], drawLocation,
						                 Color.Lerp(msg.Color, new Color(msg.Color.R, msg.Color.G, msg.Color.B, 0), msg.FadeAmount));
					}
					drawLocation += new Vector2(0, msg.Height);
				}

			}
			batch.End();
		}


		public void PostMessage(string message, TimeSpan timeUntil, TimeSpan duration, Color? color = null)
		{
			var strMeasure = Font.MeasureString(message);
			var msg = new Message
			{
				Duration = duration,
				Width = strMeasure.X,
				TimeUntil = timeUntil,
				Color = color.HasValue ? color.Value : Color.White
			};

			msg.Lines = SplitString(message, msg.Width);
			msg.Height = msg.Lines.Length*(LinePadding + strMeasure.Y);

		
			MessageQueue.Add(msg);


			MessageQueue.Sort();
		}

		public void PostQueuedMessage(string message, TimeSpan? duration = null, Color? color = null)
		{
			if (MessageQueue.Count == 0)
			{
				PostMessage(message, TimeSpan.Zero, duration.HasValue ? duration.Value : EstimateDuration(message), color);
				return;
			}

			var latestTime = MessageQueue.Last();
			if (!duration.HasValue)
			{
				duration = EstimateDuration(message);
			}
			PostMessage(message, latestTime.TimeUntil + latestTime.Duration - new TimeSpan(0, 0, (int)FadeTime), duration.Value, color);
		}

		public TimeSpan EstimateDuration(string message)
		{
			return new TimeSpan(message.Length/AverageWordLength*TimePerWord.Ticks);
		}

		public string[] SplitString(string message, float width)
		{
			return new string[1] { message };
		}


		public SpriteFont Font
		{
			get { return _font; }
			set
			{
				var str = "abcdefghijklmnopqrstuvwyxz1234567890";
				CharacterSize = value.MeasureString(str);
				CharacterSize.X /= str.Length;
				_font = value;
			}
		}

		public float FadeTime = 1; //In seconds
		public float LinePadding = 5;
		public float SidePadding = 5;

		

		private SpriteFont _font;
		
		public Rectangle MessageBounds;
		public List<Message> MessageQueue;
		public TimeSpan TimePerWord = new TimeSpan(TimeSpan.TicksPerSecond * WordsPerSecond);

		public const long WordsPerSecond = 4; //US Average = 5ish
		public const long AverageWordLength = 5;

		public Vector2 CharacterSize;

		public class Message
			: IComparable<Message>
		{
			public string[] Lines;

			public TimeSpan TimeUntil;
			public TimeSpan Duration;

			public float FadeAmount;

			public float Width;
			public float Height;

			public Color Color;

			public bool IsShown
			{
				get { return TimeUntil.TotalMilliseconds <= 0f; }
			}

			public bool IsFading
			{
				get { return FadeAmount > 0f; }
			}


			public int CompareTo(Message other)
			{
				return TimeUntil.CompareTo(other.TimeUntil);
			}


		}
	}

	
}
