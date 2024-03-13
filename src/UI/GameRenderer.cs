using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core.Game;

namespace Rythmify.UI
{
	public static class RenderableGameNote
	{
		public static float ToScreenSpaceY(double y) => (float)(y * 1440.0 / 480.0);

		public static void RenderGameNote(GameNote gameNote, SkinRenderer skinRenderer, SpriteBatch spriteBatch) {
			var drawPos = new Vector2(
				gameNote.Lane * skinRenderer.NoteTexture.Width + 30 * gameNote.Lane,
				ToScreenSpaceY(gameNote.Y) - skinRenderer.NoteTexture.Height
			);
			spriteBatch.Draw(skinRenderer.NoteTexture, drawPos, Color.White);
		}

		public static void RenderHoldNote(HoldNote note, SkinRenderer skinRenderer, SpriteBatch spriteBatch) {
			var screenSpaceHoldNoteSize = note.Height * 1440.0 / 480.0;
			var screenSpaceY = note.Y * 1440.0 / 480.0;
			var holdNoteTopDrawPos = new Vector2(
				note.Lane * skinRenderer.HoldNoteBodyTexture.Width + 30 * note.Lane,
				(float)(screenSpaceY - screenSpaceHoldNoteSize)
			);

			spriteBatch.Draw(skinRenderer.HoldNoteBodyTexture, holdNoteTopDrawPos, null, Color.White, 0, Vector2.Zero, new Vector2(1, (float)screenSpaceHoldNoteSize / skinRenderer.HoldNoteBodyTexture.Height), SpriteEffects.None, 0);
		}

		public static void RenderHitLine(GraphicsDeviceManager graphics, SkinRenderer skinRenderer, SpriteBatch spriteBatch) {
			var screenSpaceHitPosition = (int)(skinRenderer.Skin.HitPosition * graphics.PreferredBackBufferHeight / 480.0f);
			spriteBatch.Draw(skinRenderer.HitLineTexture, new Vector2(0, screenSpaceHitPosition), null, Color.White, 0, Vector2.Zero, new Vector2(graphics.PreferredBackBufferWidth, 1), SpriteEffects.None, 0);
		}
	}
}
