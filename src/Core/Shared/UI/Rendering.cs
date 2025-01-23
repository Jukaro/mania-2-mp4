using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rythmify.UI;

public static class Rendering {
	public static void DrawScaled(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 scale) =>
		spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
}
