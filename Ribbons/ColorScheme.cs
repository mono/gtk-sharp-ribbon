using Cairo;
using System;

namespace Ribbons
{
	/// <summary>Color scheme.</summary>
	public class ColorScheme
	{
		private Color prettyDark, dark, lightDark, normal, lightBright, bright, prettyBright;

		public Color PrettyDark
		{
			set { prettyDark = value; }
			get { return prettyDark; }
		}

		public Color Dark
		{
			set { dark = value; }
			get { return dark; }
		}

		public Color LightDark
		{
			set { lightDark = value; }
			get { return lightDark; }
		}

		public Color Normal
		{
			set { normal = value; }
			get { return normal; }
		}

		public Color LightBright
		{
			set { lightBright = value; }
			get { return lightBright; }
		}

		public Color Bright
		{
			set { bright = value; }
			get { return bright; }
		}

		public Color PrettyBright
		{
			set { prettyBright = value; }
			get { return prettyBright; }
		}
		
		//public ColorScheme() : this (new Color(0.957, 0.957, 0.957))
		public ColorScheme() : this (new Color(0.867, 0.867, 0.867))
		//public ColorScheme() : this (new Color(0.925, 0.914, 0.847))
		//public ColorScheme() : this (new Color(0.937, 0.922, 0.898))
		{
			
		}
		
		public ColorScheme (Color Normal)
		{
			prettyDark = GetColorRelative (Normal, -0.4);
			dark = GetColorRelative (Normal, -0.1);
			lightDark = GetColorRelative (Normal, -0.05);
			normal = Normal;
			lightBright = GetColorRelative (Normal, 0.05);
			bright = GetColorRelative (Normal, 0.1);
			prettyBright = GetColorRelative (Normal, 0.15);
		}
		
		internal static Color SetAlphaChannel(Color C, double Alpha)
		{
			return new Color (C.R, C.G, C.B, Alpha);
		}
		
		internal static Color GetColorAbsolute(Color C, double luminance)
		{
			/*double h, s, v;
			RGB2HSV(C, out h, out s, out v);
			v = v + (1.7 * (luminance - 1));
			if(v < 0) v = 0; else if(v > 1) v = 1;
			return HSV2RGB(h, s, v);*/
			
			double h, s, l;
			RGB2HSL(C, out h, out s, out l);
			/*double a = (h % 60) / 60;
			if(a > 0.5) a -= 0.5;*/
			//l = l + (1.7 * (luminance - 1));
			l = luminance;
			if(l < 0) l = 0; else if(l > 1) l = 1;
			return HSL2RGB(h, s, l);
		}

		internal static Color GetColorRelative(Color C, double luminance)
		{
			double h, s, l;
			RGB2HSL(C, out h, out s, out l);
			/*double a = (h % 60) / 60;
			if(a > 0.5) a -= 0.5;*/
			l = l + luminance;
			if(l < 0) l = 0; else if(l > 1) l = 1;
			return HSL2RGB(h, s, l);
		}

		private static void RGB2HSL(Color C, out double H, out double S, out double L)
		{
			double r = C.R, g = C.G, b = C.B;
			double max = Math.Max(r, Math.Max(g, b));
			double min = Math.Min(r, Math.Min(g, b));
			L = 0.5 * (max + min);
			if(max == min)
			{
				H = double.NaN;
			}
			else
			{
				if(max == r)
				{
					if(g >= b)
						H = (g-b)/(max-min) * 60;
					else
						H = (g-b)/(max-min) * 60 + 360;
				}
				else if(max == g)
					H = (b-r)/(max-min) * 60 + 120;
				else
					H = (r-g)/(max-min) * 60 + 240;
			}
			if(0.0000001 <= L && L <= 0.5)
				S = (max-min) / (2*L);
			else if(L > 0.5)
				S = (max-min) / (2-2*L);
			else
				S = 0;
		}

		private static Color HSL2RGB(double H, double S, double L)
		{
			double r = L, g = L, b = L;
			double Q;
			if(L < 0.5)
				Q = L * (1 + S);
			else
				Q = L + S - L * S;
			double P = 2 * L - Q;
			if(Q > 0)
			{
				double m = L + L - Q;
				H /= 60;
				int i = (int)H % 6;
				double vsf = Q * (Q - m) / Q * (H - i); 
				switch(i)
				{
					case 0:
						r = Q;
						g = m + vsf;
						b = m;
						break;
					case 1:
						r = m - vsf;
						g = Q;
						b = m;
						break;
					case 2:
						r = m;
						g = Q;
						b = m + vsf;
						break;
					case 3:
						r = m;
						g = m - vsf;
						b = Q;
						break;
					case 4:
						r = m + vsf;
						g = m;
						b = Q;
						break;
					case 5:
						r = Q;
						g = m;
						b = m - vsf;
						break;
				}
			}
			return new Color (r, g, b);
		}

		private static void RGB2HSV(Color C, out double H, out double S, out double V)
		{	// http://www.daniweb.com/techtalkforums/thread38302.html
			double r = C.R, g = C.G, b = C.B;
			double max = Math.Max (r, Math.Max (g, b));
			double min = Math.Min (r, Math.Min (g, b));
			double delta = max - min;
			V = max;
			if(Math.Abs (delta) < 0.0000001)
			{
				H = S = 0;
			}
			else
			{
				S = delta / max;
				if(r == max)
					H = 60.0 * (g - b) / delta;
				else if(g == max)
					H = 60.0 * (2 + (b - r) / delta);
				else
					H = 60.0 * (4 + (r - g) / delta);
				if(H < 0) H += 360; else if(H > 360) H -= 360;
			}
		}

		private static Color HSV2RGB(double H, double S, double V)
		{	// http://en.wikipedia.org/wiki/HSV_color_space
			int H_i = (int)(H / 60) % 6;
			double f = H / 60 - H_i;
			if(H_i == 0) return new Color (V, V * (1 - (1 - f) * S), V * (1 - S));
			if(H_i == 1) return new Color (V * (1 - f * S), V, V * (1 - S));
			if(H_i == 2) return new Color (V * (1 - S), V, V * (1 - (1 - f) * S));
			if(H_i == 3) return new Color (V * (1 - S), V * (1 - f * S), V);
			if(H_i == 4) return new Color (V * (1 - (1 - f) * S), V * (1 - S), V);
			return new Color (V, V * (1 - S), V * (1 - f * S));
		}
	}
}
