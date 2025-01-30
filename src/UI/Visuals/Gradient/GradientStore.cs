
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Rythmify.UI;

public static class GradientStore {
	public static List<GradientList> gradients;

	public static void InitGradients() {
		gradients = new();

		GradientList RGBGradient = new();
		RGBGradient.Add(new(new(255, 0, 0), new(255, 255, 0)));
		RGBGradient.Add(new(new(255, 255, 0), new(0, 255, 0)));
		RGBGradient.Add(new(new(0, 255, 0), new(0, 255, 255)));
		RGBGradient.Add(new(new(0, 255, 255), new(0, 0, 255)));
		RGBGradient.Add(new(new(0, 0, 255), new(255, 0, 255)));
		RGBGradient.Add(new(new(255, 0, 255), new(255, 0, 0)));
		gradients.Add(RGBGradient);

		GradientList BlackRedGradient = new();
		BlackRedGradient.Add(new(Color.Black, Color.Red));
		BlackRedGradient.Add(new(Color.Red, Color.Black));
		gradients.Add(BlackRedGradient);
	}
}


