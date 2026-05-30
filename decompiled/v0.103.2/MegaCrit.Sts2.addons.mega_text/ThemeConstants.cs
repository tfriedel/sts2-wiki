using Godot;

namespace MegaCrit.Sts2.addons.mega_text;

public static class ThemeConstants
{
	public static class Label
	{
		public static readonly StringName FontSize = StringName.op_Implicit("font_size");

		public static readonly StringName Font = StringName.op_Implicit("font");

		public static readonly StringName LineSpacing = StringName.op_Implicit("line_spacing");

		public static readonly StringName OutlineSize = StringName.op_Implicit("outline_size");

		public static readonly StringName FontColor = StringName.op_Implicit("font_color");

		public static readonly StringName FontOutlineColor = StringName.op_Implicit("font_outline_color");

		public static readonly StringName FontShadowColor = StringName.op_Implicit("font_shadow_color");
	}

	public static class RichTextLabel
	{
		public static readonly StringName NormalFont = StringName.op_Implicit("normal_font");

		public static readonly StringName BoldFont = StringName.op_Implicit("bold_font");

		public static readonly StringName ItalicsFont = StringName.op_Implicit("italics_font");

		public static readonly StringName LineSpacing = StringName.op_Implicit("line_separation");

		public static readonly StringName NormalFontSize = StringName.op_Implicit("normal_font_size");

		public static readonly StringName BoldFontSize = StringName.op_Implicit("bold_font_size");

		public static readonly StringName BoldItalicsFontSize = StringName.op_Implicit("bold_italics_font_size");

		public static readonly StringName ItalicsFontSize = StringName.op_Implicit("italics_font_size");

		public static readonly StringName MonoFontSize = StringName.op_Implicit("mono_font_size");

		public static readonly StringName[] AllFontSizes = (StringName[])(object)new StringName[5] { NormalFontSize, BoldFontSize, BoldItalicsFontSize, ItalicsFontSize, MonoFontSize };

		public static readonly StringName DefaultColor = StringName.op_Implicit("default_color");

		public static readonly StringName FontOutlineColor = StringName.op_Implicit("font_outline_color");

		public static readonly StringName FontShadowColor = StringName.op_Implicit("font_shadow_color");
	}

	public static class Control
	{
		public static readonly StringName Focus = StringName.op_Implicit("focus");
	}

	public static class MarginContainer
	{
		public static readonly StringName MarginLeft = StringName.op_Implicit("margin_left");

		public static readonly StringName MarginRight = StringName.op_Implicit("margin_right");

		public static readonly StringName MarginTop = StringName.op_Implicit("margin_top");

		public static readonly StringName MarginBottom = StringName.op_Implicit("margin_bottom");
	}

	public static class BoxContainer
	{
		public static readonly StringName Separation = StringName.op_Implicit("separation");
	}

	public static class FlowContainer
	{
		public static readonly StringName HSeparation = StringName.op_Implicit("h_separation");

		public static readonly StringName VSeparation = StringName.op_Implicit("v_separation");
	}

	public static class TextEdit
	{
		public static readonly StringName Font = StringName.op_Implicit("font");
	}

	public static class LineEdit
	{
		public static readonly StringName Font = StringName.op_Implicit("font");
	}
}
