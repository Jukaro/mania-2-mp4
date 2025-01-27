using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.UI;

public static class VisualsStore {
	public static List<Visuals> visuals;

	public static void InitVisuals(GraphicsDevice graphics, List<GradientList> gradients) {
		visuals = new();

		// 0
		Visuals basicBlackButton = new(graphics, 100, 50, Color.Black) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		visuals.Add(basicBlackButton);

		// 1
		Visuals basicRGBButton = new(graphics, 300, 50, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicRGBButton.SetGradientAsColor(gradients[0], 10);
		visuals.Add(basicRGBButton);

		// 2
		Visuals basicBlackRedButton = new(graphics, 100, 50, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicBlackRedButton.SetGradientAsColor(gradients[1], 100);
		visuals.Add(basicBlackRedButton);

		// 3
		Visuals basicThinBlackRedButton = new(graphics, 100, 10, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		basicThinBlackRedButton.SetGradientAsColor(gradients[1], 100);
		visuals.Add(basicThinBlackRedButton);

		// 4
		Visuals beatmapDropdown = new(graphics, 600, 800, Color.White) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		beatmapDropdown.SetGradientAsColor(gradients[1], 100);
		visuals.Add(beatmapDropdown);

		// 5
		Visuals beatmapDisplay = new(graphics, 600, 100, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		visuals.Add(beatmapDisplay);

		// 6
		Visuals scoreDropdown = new(graphics, 600, 800, Color.Transparent) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		// scoreDropdown.SetGradientAsColor(gradients[1], 100);
		visuals.Add(scoreDropdown);

		// 7
		Visuals scoreDisplay = new(graphics, 600, 100, Color.White) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		visuals.Add(scoreDisplay);

		// 8
		Visuals sessionDropdown = new(graphics, 300, 800, Color.Transparent) {
			BlinkOnMouseClick = false,
			BlinkOnMouseOver = false
		};
		// scoreDropdown.SetGradientAsColor(gradients[1], 100);
		visuals.Add(sessionDropdown);

		// 9
		Visuals sessionDisplay = new(graphics, 300, 20, Color.Black) {
			BlinkOnMouseClick = true,
			BlinkOnMouseOver = true
		};
		visuals.Add(sessionDisplay);
	}
}
