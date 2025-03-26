using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.UI;

public class UIElementContainer : Button {
	public List<UIElement> UIElementsList;

	public int nbRenderedUIElements = 0;
	public int UsableWidth = 0;
	public int UsableHeight = 0;

	public UIElementContainer(GraphicsDevice graphics, int width, int height, Vector2 pos, string name, Color color) : base(graphics, width, height, pos, name, color) {
		Init();
	}

	public UIElementContainer(GraphicsDevice graphics, Vector2 pos, string name, Visuals visuals)  : base(graphics, pos, name, visuals) {
		Init();
	}

	private void Init() {
		UIElementsList = new();
		UsableWidth = Width;
		UsableHeight = Height;
	}

/* -------------------------------- Accessors ------------------------------- */

	public UIElement this[int index] {
		get => UIElementsList[index];
		set {
			value.SetAbsolutePos(new (AbsolutePos.X + value.RelativePos.X, AbsolutePos.Y + value.RelativePos.Y));
			UIElementsList[index] = value;
		}
	}

	public UIElement this[string key] {
		get => UIElementsList.FirstOrDefault(o => o.Name == key);
		set {
			UIElement UIElement = UIElementsList.FirstOrDefault(o => o.Name == key);
			if (UIElement != null) {
				UIElement = value;
				UIElement.SetAbsolutePos(new (AbsolutePos.X + UIElement.RelativePos.X, AbsolutePos.Y + UIElement.RelativePos.Y));
			}
			else
				Logger.LogDebug($"Didn't find the UIElement \"{key}\"");
		}
	}

/* ---------------------------- Getters / Setters --------------------------- */

	public override void SetAbsolutePos(Vector2 pos) {
		base.SetAbsolutePos(pos);

		foreach (var UIElement in UIElementsList) {
			UIElement.SetAbsolutePos(new (AbsolutePos.X + UIElement.RelativePos.X, AbsolutePos.Y + UIElement.RelativePos.Y));
		}
	}

/* --------------------------------- Methods -------------------------------- */

	public virtual void Add(UIElement UIElement) {
		UIElement.SetAbsolutePos(new (AbsolutePos.X + UIElement.RelativePos.X, AbsolutePos.Y + UIElement.RelativePos.Y));
		UIElementsList.Add(UIElement);
	}

	public virtual void RemoveAll() {
		UIElementsList.Clear();
	}

	public int GetIndexOfUIElement(UIElement UIElement) {
		return UIElementsList.IndexOf(UIElement);
	}

	public virtual UIElement LastElement() {
		return UIElementsList.Last();
	}

/* --------------------------------- Update --------------------------------- */

	public override void Update() {
		foreach (var UIElement in UIElementsList) {
			if (UIElement.AbsolutePos.Y <= AbsolutePos.Y + Visuals.Texture.Height && UIElement.AbsolutePos.Y >= AbsolutePos.Y) { // mdr non faut ameliorer
				UIElement.Update();
			}
		}
		base.Update();
	}

/* --------------------------------- Render --------------------------------- */

	public override void Render(SpriteBatch spriteBatch) {
		base.Render(spriteBatch);
		RenderUIElements(spriteBatch);
	}

	public override void RenderPartial(SpriteBatch spriteBatch, float limitY, int mode) {
		base.RenderPartial(spriteBatch, limitY, mode);
		RenderUIElements(spriteBatch);
	}

	protected virtual void RenderUIElements(SpriteBatch spriteBatch) {
		if (Hide)
			return;
		foreach (var UIElement in UIElementsList) {
			if (UIElement.AbsolutePos.Y + UIElement.Height < 0 || UIElement.AbsolutePos.Y > 1000)
				continue;
			RenderUIElement(UIElement, spriteBatch);
		}
	}

	private void RenderUIElement(UIElement UIElement, SpriteBatch spriteBatch) {
		if (UIElement.AbsolutePos.Y >= AbsolutePos.Y && UIElement.AbsolutePos.Y + UIElement.Height <= AbsolutePos.Y + Height) { // if (UIElement.AbsolutePos.Y + UIElement.Height) puis else if (UIElement.AbsolutePos.Y): PartialRender
			UIElement.Render(spriteBatch);
		}
		else if (UIElement.AbsolutePos.Y <= AbsolutePos.Y + Height && UIElement.AbsolutePos.Y + UIElement.Height >= AbsolutePos.Y + Height) {
			UIElement.RenderPartial(spriteBatch, AbsolutePos.Y + Height, 1);
		}
		else if (UIElement.AbsolutePos.Y <= AbsolutePos.Y && UIElement.AbsolutePos.Y + UIElement.Height > AbsolutePos.Y) {
			UIElement.RenderPartial(spriteBatch, AbsolutePos.Y, 0);
		}
	}
}
