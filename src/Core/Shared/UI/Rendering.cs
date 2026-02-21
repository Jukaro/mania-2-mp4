using System.Numerics;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Rythmify.UI;

public static class Rendering {
	// public static void DrawScaled(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 scale) =>
	// 	spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

	public static void DrawScaled(DrawingContext drawingContext, Bitmap texture, Vector2 position, Vector2 scale) =>
		drawingContext.DrawImage(texture, new Avalonia.Rect(position.X, position.Y, texture.Size.Width * scale.X, texture.Size.Height * scale.Y));
}
