using Cairo;
using System;

namespace Ribbons
{
	/// <summary>Ribbon theme.</summary>
	/// <remarks>Used to draw ribbon widgets.</remarks>
	public class Theme
	{
		public enum ButtonState
		{
			Default, Hover, Pressed
		}
		
		protected ColorScheme colorScheme = new ColorScheme ();
		
		/// <summary>Draws a group.</summary>
		internal void DrawGroup (Context cr, Rectangle r, double roundSize, double lineWidth, double space, Pango.Layout l, Gtk.Widget expandButton, RibbonGroup w)
		{
			double lineWidth05 = lineWidth/2, lineWidth15 = 3*lineWidth05;
			LinearGradient linGrad;
			
			double x0 = r.X + roundSize, x1 = r.X + r.Width - roundSize;
			double y0 = r.Y + roundSize, y1 = r.Y + r.Height - roundSize;
			cr.Arc (x1, y1, roundSize - lineWidth05, 0, Math.PI/2);
			cr.Arc (x0 + lineWidth, y1, roundSize - lineWidth05, Math.PI/2, Math.PI);
			cr.Arc (x0, y0, roundSize - lineWidth15, Math.PI, 3*Math.PI/2);
			cr.Arc (x1, y0 + lineWidth, roundSize - lineWidth05, 3*Math.PI/2, 0);
			cr.LineTo (x1 + roundSize - lineWidth05, y1);
			cr.LineWidth = lineWidth;
			cr.Color = colorScheme.Bright;
			cr.Stroke ();
			
			if(l != null)
			{
				int lblWidth, lblHeight;
				Pango.CairoHelper.UpdateLayout (cr, l);
				l.GetPixelSize(out lblWidth, out lblHeight);
				
				double bandHeight = lblHeight + 2*space;
				cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
				cr.Arc (x0, y1 - lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
				double bandY = y1 + roundSize - 2*lineWidth - bandHeight;
				cr.LineTo (x0 - roundSize + lineWidth05, bandY);
				cr.LineTo (x1 + roundSize - lineWidth15, bandY);
				linGrad = new LinearGradient (0, bandY, 0, bandY + bandHeight);
				linGrad.AddColorStop (0.0, colorScheme.Dark);
				linGrad.AddColorStop (1.0, colorScheme.PrettyDark);
				cr.Pattern = linGrad;
				cr.Fill ();
				
				double frameSize = 2*lineWidth + space;
				double availableHorizontalSpace = r.Width - 2 * frameSize;
				if(expandButton.Visible) availableHorizontalSpace -= expandButton.WidthRequest + space;
				
				cr.Save ();
				cr.Rectangle (r.X + frameSize, bandY, availableHorizontalSpace, bandHeight);
				cr.Clip ();
				
				cr.Color = new Color(1, 1, 1);
				Pango.CairoHelper.UpdateLayout (cr, l);
				cr.MoveTo (r.X + frameSize + Math.Max(0, (availableHorizontalSpace - lblWidth) / 2), bandY + space);
				Pango.CairoHelper.ShowLayout (cr, l);
				
				cr.Restore();
			}
			
			cr.MoveTo (x1 + roundSize - lineWidth15, y1);
			cr.Arc (x1, y1, roundSize - lineWidth15, 0, Math.PI/2);
			cr.Arc (x0, y1 - lineWidth, roundSize - lineWidth05, Math.PI/2, Math.PI);
			cr.Arc (x0, y0, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
			cr.Arc (x1 - lineWidth, y0, roundSize - lineWidth05, 3*Math.PI/2, 0);
			cr.LineTo (x1 + roundSize - lineWidth15, y1);
			cr.LineWidth = lineWidth;
			linGrad = new LinearGradient (0, r.Y, 0, r.Y + r.Height - lineWidth);
			linGrad.AddColorStop (0.0, ColorScheme.GetColorRelative (colorScheme.PrettyDark, 0.1));
			linGrad.AddColorStop (1.0, colorScheme.PrettyDark);
			cr.Pattern = linGrad;
			cr.Stroke ();
		}
		
		public Gdk.Color GetForecolorForRibbonTabs (bool Selected)
		{
			if(Selected)
				return new Gdk.Color (0, 0, 0);
			else
				return new Gdk.Color (255, 255, 255);
		}
		
		/// <summary>Draws a ribbon.</summary>
		public void DrawRibbon (Context cr, Gdk.Rectangle bodyAllocation, double roundSize, double lineWidth, Ribbon widget)
		{
			double lineWidth05 = lineWidth / 2;
			double lineWidth15 = 3 * lineWidth05;
			double x0, x1, y0, y1;
			LinearGradient linGrad;
			
			Ribbon.RibbonPage p = widget.CurrentPage;
			if(p != null)
			{
				//Color c = ColorScheme.GetColor(colorScheme.Bright, 0.92);
				Color c = colorScheme.Normal;
				
				if(bodyAllocation.Height > 0)
				{
					/*** PAGE ***/
					
					x0 = bodyAllocation.X; x1 = bodyAllocation.X + bodyAllocation.Width;
					y0 = bodyAllocation.Y; y1 = bodyAllocation.Y + bodyAllocation.Height;
					
					cr.Arc (x0 + roundSize, y1 - roundSize, roundSize - lineWidth05, Math.PI/2, Math.PI);
					cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
					cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
					cr.Arc (x1 - roundSize, y1 - roundSize, roundSize - lineWidth05, 0, Math.PI/2);
					cr.LineTo (x0 + roundSize, y1 - lineWidth05);
					
					/*** BACKGOUND ***/
					cr.Color = c;
					cr.FillPreserve ();

					/*** DARK BORDER ***/
					cr.LineWidth = lineWidth;
					cr.Color = ColorScheme.GetColorAbsolute (colorScheme.Normal, 0.75);
					cr.Stroke ();
					
					/*** GLASS EFFECT ***/
					double ymid = Math.Round(y0 + (y1 - y0) * 0.25);
					
					cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth, Math.PI, 3*Math.PI/2);
					cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth, 3*Math.PI/2, 0);
					cr.LineTo (x1 - lineWidth, ymid);
					cr.LineTo (x0 + lineWidth, ymid);
					cr.LineTo (x0 + lineWidth, y0 + roundSize);
					linGrad = new LinearGradient (0, y0, 0, ymid);
					linGrad.AddColorStop (0.0, new Color (0, 0, 0, 0.0));
					linGrad.AddColorStop (1.0, new Color (0, 0, 0, 0.075));
					cr.Pattern = linGrad;
					cr.Fill ();
					
					cr.Arc (x0 + roundSize, y1 - roundSize, roundSize - lineWidth, Math.PI/2, Math.PI);
					cr.LineTo (x0 + lineWidth, ymid);
					cr.LineTo (x1 - lineWidth, ymid);
					cr.Arc (x1 - roundSize, y1 - roundSize, roundSize - lineWidth, 0, Math.PI/2);
					cr.LineTo (x0 + roundSize, y1 - lineWidth);
					linGrad = new LinearGradient (0, ymid, 0, y1);
					linGrad.AddColorStop (0.0, new Color (0, 0, 0, 0.1));
					linGrad.AddColorStop (0.5, new Color (0, 0, 0, 0.0));
					cr.Pattern = linGrad;
					cr.Fill ();
				}
				
				/*** TAB ***/
				
				Gdk.Rectangle r = p.LabelAllocation;
				
				x0 = r.X; x1 = r.X + r.Width;
				y0 = r.Y; y1 = r.Y + r.Height + lineWidth;
				
				/*** TAB :: BACKGROUND ***/
				
				cr.MoveTo (x0 + lineWidth05, y1);
				cr.LineTo (x0 + lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth05, y1);
				
				linGrad = new LinearGradient (0, y0, 0, y1);
				linGrad.AddColorStop (0.0, colorScheme.PrettyBright);
				linGrad.AddColorStop (1.0, c);
				cr.Pattern = linGrad;
				cr.Fill ();
				
				/*** TAB :: DARK BORDER ***/
				
				cr.MoveTo (x0 + lineWidth05, y1);
				cr.LineTo (x0 + lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth05, y1);
				
				cr.LineWidth = lineWidth;
				cr.Color = ColorScheme.GetColorRelative (colorScheme.Bright, -0.1);
				cr.Stroke ();
				
				y1 -= 1.0;
				
				/*** TAB :: HIGHLIGHT ***/
				
				cr.MoveTo (x0 + lineWidth15, y1);
				cr.LineTo (x0 + lineWidth15, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize - lineWidth15, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize - lineWidth15, 3*Math.PI/2, 0);
				cr.LineTo (x1 - lineWidth15, y1);
				
				cr.LineWidth = lineWidth;
				linGrad = new LinearGradient (0, y0+lineWidth, 0, y1);
				linGrad.AddColorStop (0.0, colorScheme.PrettyBright);
				linGrad.AddColorStop (1.0, ColorScheme.SetAlphaChannel (colorScheme.Bright, 0));
				cr.Pattern = linGrad;
				cr.Stroke ();
				
				/*** TAB :: SHADOW ***/
				
				cr.MoveTo (x0 - lineWidth05, y1);
				cr.LineTo (x0 - lineWidth05, y0 + roundSize);
				cr.Arc (x0 + roundSize, y0 + roundSize, roundSize + lineWidth05, Math.PI, 3*Math.PI/2);
				cr.Arc (x1 - roundSize, y0 + roundSize, roundSize + lineWidth05, 3*Math.PI/2, 0);
				cr.LineTo (x1 + lineWidth05, y1);
				
				cr.LineWidth = lineWidth;
				cr.Color = new Color (0, 0, 0, 0.2);
				cr.Stroke ();
			}
		}
		
		/// <summary>Draws a button.</summary>
		public void DrawButton (Context cr, Rectangle bodyAllocation, ButtonState state, double roundSize, double lineWidth, double arrowSize, double arrowPadding, bool drawSeparator, BaseButton widget)
		{
			double lineWidth05 = lineWidth / 2;
			double lineWidth15 = lineWidth05 * 3;
			
			bool upLeft = true, upRight = true, downRight = true, downLeft = true;
			switch(widget.GroupStyle)
			{
				case GroupStyle.Left:
					upRight = downRight = false;
					break;
				case GroupStyle.Center:
					upLeft = downLeft = upRight = downRight = false;
					break;
				case GroupStyle.Right:
					upLeft = downLeft = false;
					break;
			}
			
			cr.LineWidth = lineWidth;
			
			if(state == ButtonState.Pressed || state == ButtonState.Hover)
			{
				LinearGradient bodyPattern, innerBorderPattern;
				Color borderColor;
				
				if(state == ButtonState.Pressed)
				{
					bodyPattern = new LinearGradient (bodyAllocation.X, bodyAllocation.Y, bodyAllocation.X, bodyAllocation.Y + bodyAllocation.Height);
					bodyPattern.AddColorStopRgb (0.0, new Color (0.996, 0.847, 0.667));
					bodyPattern.AddColorStopRgb (0.37, new Color (0.984, 0.710, 0.396));
					bodyPattern.AddColorStopRgb (0.43, new Color (0.980, 0.616, 0.204));
					bodyPattern.AddColorStopRgb (1.0, new Color (0.992, 0.933, 0.667));
					
					innerBorderPattern = new LinearGradient (bodyAllocation.X, bodyAllocation.Y, bodyAllocation.X + bodyAllocation.Width, bodyAllocation.Y + bodyAllocation.Height);
					innerBorderPattern.AddColorStop (0.0, new Color (0.876, 0.718, 0.533, 1));
					innerBorderPattern.AddColorStop (1.0, new Color (0.876, 0.718, 0.533, 0));
					
					borderColor = new Color (0.671, 0.631, 0.549);
				}
				else
				{
					bodyPattern = new LinearGradient (bodyAllocation.X, bodyAllocation.Y, bodyAllocation.X, bodyAllocation.Y + bodyAllocation.Height);
					bodyPattern.AddColorStopRgb (0.0, new Color (1, 0.996, 0.890));
					bodyPattern.AddColorStopRgb (0.37, new Color (1, 0.906, 0.592));
					bodyPattern.AddColorStopRgb (0.43, new Color (1, 0.843, 0.314));
					bodyPattern.AddColorStopRgb (1.0, new Color (1, 0.906, 0.588));
					
					innerBorderPattern = new LinearGradient (bodyAllocation.X, bodyAllocation.Y, bodyAllocation.X + bodyAllocation.Width, bodyAllocation.Y + bodyAllocation.Height);
					innerBorderPattern.AddColorStop (0.0, new Color (1, 1, 0.969, 1));
					innerBorderPattern.AddColorStop (1.0, new Color (1, 1, 0.969, 0));
					
					borderColor = new Color (0.824, 0.753, 0.553);
				}
				
				double x0 = bodyAllocation.X + lineWidth05, y0 = bodyAllocation.Y + lineWidth05;
				double x1 = bodyAllocation.X + bodyAllocation.Width - lineWidth05, y1 = bodyAllocation.Y + bodyAllocation.Height - lineWidth05;
				
				if(upLeft) cr.MoveTo (x0 + roundSize, y0);
				else cr.MoveTo (x0, y0);
				if(upRight) cr.Arc (x1 - roundSize, y0 + roundSize, roundSize, 1.5*Math.PI, 0);
				else cr.LineTo (x1, y0);
				if(downRight) cr.Arc (x1 - roundSize, y1 - roundSize, roundSize, 0, 0.5*Math.PI);
				else cr.LineTo (x1, y1);
				if(downLeft) cr.Arc (x0 + roundSize, y1 - roundSize, roundSize, 0.5*Math.PI, Math.PI);
				else cr.LineTo (x0, y1);
				if(upLeft) cr.Arc (x0 + roundSize, y0 + roundSize, roundSize, Math.PI, 1.5*Math.PI);
				else cr.LineTo (x0, y0);
				
				cr.Pattern = bodyPattern;
				cr.Fill ();
				
				x0 = bodyAllocation.X + lineWidth15; y0 = bodyAllocation.Y + lineWidth15;
				x1 = bodyAllocation.X + bodyAllocation.Width - lineWidth15; y1 = bodyAllocation.Y + bodyAllocation.Height - lineWidth15;
				
				double roundSizeMinusLineWidth = roundSize - lineWidth;
				
				if(widget.GroupStyle != GroupStyle.Left) x0 -= lineWidth;
				
				if(upLeft) cr.MoveTo (x0 + roundSizeMinusLineWidth, y0);
				else cr.MoveTo (x0, y0);
				if(upRight) cr.Arc (x1 - roundSizeMinusLineWidth, y0 + roundSizeMinusLineWidth, roundSizeMinusLineWidth, 1.5*Math.PI, 0);
				else cr.LineTo (x1, y0);
				if(downRight) cr.Arc (x1 - roundSizeMinusLineWidth, y1 - roundSizeMinusLineWidth, roundSizeMinusLineWidth, 0, 0.5*Math.PI);
				else cr.LineTo (x1, y1);
				if(downLeft) cr.Arc (x0 + roundSizeMinusLineWidth, y1 - roundSizeMinusLineWidth, roundSizeMinusLineWidth, 0.5*Math.PI, Math.PI);
				else cr.LineTo (x0, y1);
				if(upLeft) cr.Arc (x0 + roundSizeMinusLineWidth, y0 + roundSizeMinusLineWidth, roundSizeMinusLineWidth, Math.PI, 1.5*Math.PI);
				else cr.LineTo (x0, y0);
				
				if(widget.GroupStyle != GroupStyle.Left) x0 += lineWidth;
				
				cr.Pattern = innerBorderPattern;
				cr.Stroke ();
				
				x0 = bodyAllocation.X + lineWidth05; y0 = bodyAllocation.Y + lineWidth05;
				x1 = bodyAllocation.X + bodyAllocation.Width - lineWidth05; y1 = bodyAllocation.Y + bodyAllocation.Height - lineWidth05;
				
				if(upLeft) cr.MoveTo (x0 + roundSize, y0);
				else cr.MoveTo (x0, y0);
				if(upRight) cr.Arc (x1 - roundSize, y0 + roundSize, roundSize, 1.5*Math.PI, 0);
				else cr.LineTo (x1, y0);
				if(downRight) cr.Arc (x1 - roundSize, y1 - roundSize, roundSize, 0, 0.5*Math.PI);
				else cr.LineTo (x1, y1);
				if(downLeft) cr.Arc (x0 + roundSize, y1 - roundSize, roundSize, 0.5*Math.PI, Math.PI);
				else cr.LineTo (x0, y1);
				if(widget.GroupStyle == GroupStyle.Left)
				{
					if(upLeft) cr.Arc (x0 + roundSize, y0 + roundSize, roundSize, Math.PI, 1.5*Math.PI);
					else cr.LineTo (x0, y0);
				}
				
				cr.Color = borderColor;
				cr.Stroke ();
			}
			else if(widget.DrawBackground)
			{
				LinearGradient bodyPattern = new LinearGradient (bodyAllocation.X, bodyAllocation.Y, bodyAllocation.X, bodyAllocation.Y + bodyAllocation.Height);
				bodyPattern.AddColorStop (0.0, new Color (1, 1, 1, 0.7));
				bodyPattern.AddColorStop (0.37, new Color (1, 1, 1, 0.2));
				bodyPattern.AddColorStop (0.43, new Color (1, 1, 1, 0.2));
				bodyPattern.AddColorStop (1.0, new Color (1, 1, 1, 0.7));
				
				double x0 = bodyAllocation.X + lineWidth05, y0 = bodyAllocation.Y + lineWidth05;
				double x1 = bodyAllocation.X + bodyAllocation.Width - lineWidth05, y1 = bodyAllocation.Y + bodyAllocation.Height - lineWidth05;
				
				if(upLeft) cr.MoveTo (x0 + roundSize, y0);
				else cr.MoveTo (x0, y0);
				if(upRight) cr.Arc (x1 - roundSize, y0 + roundSize, roundSize, 1.5*Math.PI, 0);
				else cr.LineTo (x1, y0);
				if(downRight) cr.Arc (x1 - roundSize, y1 - roundSize, roundSize, 0, 0.5*Math.PI);
				else cr.LineTo (x1, y1);
				if(downLeft) cr.Arc (x0 + roundSize, y1 - roundSize, roundSize, 0.5*Math.PI, Math.PI);
				else cr.LineTo (x0, y1);
				if(upLeft) cr.Arc (x0 + roundSize, y0 + roundSize, roundSize, Math.PI, 1.5*Math.PI);
				else cr.LineTo (x0, y0);
				
				cr.Pattern = bodyPattern;
				cr.Fill ();
				
				if(widget.GroupStyle != GroupStyle.Left)
				{
					if(downLeft) cr.Arc (x0 + roundSize, y1 - roundSize, roundSize, 0.5*Math.PI, Math.PI);
					else cr.MoveTo (x0, y1);
					if(upLeft) cr.Arc (x0 + roundSize, y0 + roundSize, roundSize, Math.PI, 1.5*Math.PI);
					else cr.LineTo (x0, y0);
					
					cr.Color = new Color (1, 1, 1, 0.8);
					cr.Stroke ();
				}
				
				if(upLeft) cr.Arc (x0 + roundSize, y0 + roundSize, roundSize, Math.PI, 1.5*Math.PI);
				else cr.MoveTo (x0, y0);
				if(upRight) cr.Arc (x1 - roundSize, y0 + roundSize, roundSize, 1.5*Math.PI, 0);
				else cr.LineTo (x1, y0);
				if(downRight) cr.Arc (x1 - roundSize, y1 - roundSize, roundSize, 0, 0.5*Math.PI);
				else cr.LineTo (x1, y1);
				if(downLeft) cr.Arc (x0 + roundSize, y1 - roundSize, roundSize, 0.5*Math.PI, Math.PI);
				else cr.LineTo (x0, y1);
				if(widget.GroupStyle == GroupStyle.Left)
				{
					if(upLeft) cr.LineTo (x0, y0 + roundSize);
					else cr.LineTo (x0, y0);
				}
				
				cr.Color = new Color (0, 0, 0, 0.2);
				cr.Stroke ();
			}
			
			if(arrowSize > 0)
			{
				double x, y;
				
				switch(widget.ImagePosition)
				{
					case Gtk.PositionType.Bottom:
					case Gtk.PositionType.Top:
						x = bodyAllocation.X + (bodyAllocation.Width - arrowSize) / 2.0;
						y = bodyAllocation.Y + bodyAllocation.Height - 2 * lineWidth - arrowSize - 2 * arrowPadding;
						
						if(drawSeparator)
						{
							double left = bodyAllocation.X + 2 * lineWidth, right = bodyAllocation.X + bodyAllocation.Width - 2 * lineWidth;
							
							cr.MoveTo (left, y - lineWidth / 2);
							cr.LineTo (right, y - lineWidth / 2);
							cr.Color = new Color (0, 0, 0, 0.1);
							cr.Stroke ();
							
							cr.MoveTo (left, y + lineWidth / 2);
							cr.LineTo (right, y + lineWidth / 2);
							cr.Color = new Color (1, 1, 1, 0.6);
							cr.Stroke ();
						}
						
						y += arrowPadding;
						break;
					default:
						x = bodyAllocation.X + bodyAllocation.Width - 2 * lineWidth - arrowSize - 2 * arrowPadding;
						y = bodyAllocation.Y + (bodyAllocation.Height - arrowSize) / 2.0;
						
						if(drawSeparator)
						{
							double top = bodyAllocation.Y + 2 * lineWidth, bottom = bodyAllocation.Y + bodyAllocation.Height - 2 * lineWidth;
							cr.MoveTo (x - lineWidth / 2, top);
							cr.LineTo (x - lineWidth / 2, bottom);
							cr.Color = new Color (0, 0, 0, 0.1);
							cr.Stroke ();
							
							cr.MoveTo (x + lineWidth / 2, top);
							cr.LineTo (x + lineWidth / 2, bottom);
							cr.Color = new Color (1, 1, 1, 0.6);
							cr.Stroke ();
						}
						
						x += arrowPadding;
						break;
				}
				
				y += arrowSize / 4.0 + lineWidth / 2.0;
				cr.MoveTo (x, y);
				cr.LineTo (x + arrowSize, y);
				cr.LineTo (x + arrowSize / 2.0, y + arrowSize / 2.0);
				cr.LineTo (x, y);
				cr.Color = new Color (0, 0, 0);
				cr.Fill ();
			}
		}
		
		/// <summary>Draws a gallery.</summary>
		public void DrawGallery (Context cr, Rectangle bodyAllocation, Rectangle tilesAllocation, Gallery widget)
		{
			cr.Color = new Color (0, 0, 0, 0.3);
			cr.LineWidth = 1;
			cr.Rectangle (tilesAllocation.X - 0.5, tilesAllocation.Y - 0.5, tilesAllocation.Width + 1.0, tilesAllocation.Height + 1.0);
			cr.Stroke ();
		}
		
		/// <summary>Draws a tile.</summary>
		public void DrawTile (Context cr, Rectangle bodyAllocation, Rectangle contentAllocation, Tile widget)
		{
			if(widget.Selected)
			{
				LinearGradient grad = new LinearGradient (bodyAllocation.X, bodyAllocation.Y, bodyAllocation.X, bodyAllocation.Y + bodyAllocation.Height);
				grad.AddColorStop (0.00, new Color (0.9922, 0.7373, 0.4353));
				grad.AddColorStop (0.27, new Color (0.9961, 0.8039, 0.5569));
				grad.AddColorStop (0.33, new Color (0.9961, 0.7255, 0.4078));
				grad.AddColorStop (1.00, new Color (0.9843, 0.8980, 0.6313));
				cr.Pattern = grad;
				cr.Rectangle (bodyAllocation);
				cr.Fill ();
			}
			
			cr.Color = new Color (1, 1, 1);
			cr.Rectangle (contentAllocation);
			cr.Fill ();
		}
	}
}
