using System;
using Gtk;

namespace Ribbons
{
	public class KeyTip
	{
		private Widget target;
		private string accelerator;
		private bool enabled;
		private Window win;
		private Theme theme = new Theme ();
		
		public Widget Target
		{
			get { return target; }
			set { target = value; }
		}
		
		public string Accelerator
		{
			get { return accelerator; }
			set { accelerator = value; }
		}
		
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}
		
		public KeyTip ()
		{
			
		}
		
		public KeyTip (Widget Target, string Accelerator)
		{
			target = Target;
			accelerator = Accelerator;
			enabled = true;
		}
		
		public void ShowAt (int x, int y, double horizontal_align, double vertical_align)
		{
			if(win == null)
			{
				win = new Window (WindowType.Popup);
			}
			
			Pango.Layout layout = win.CreatePangoLayout (accelerator);
			int width, height;
			layout.GetPixelSize (out width, out height);
			width += 2;
			height += 2;
			
			x -= (int)(horizontal_align * width);
			y -= (int)(vertical_align * height);
			
			win.Show ();
			win.GdkWindow.Move (x, y);
			win.GdkWindow.Resize (width, height);
			
			win.ExposeEvent += delegate(object Sender, ExposeEventArgs Args)
			{
				Gdk.EventExpose evnt = Args.Event;
				
				Cairo.Context cr = Gdk.CairoHelper.Create (win.GdkWindow);
				
				cr.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
				cr.Clip ();
				theme.DrawKeyTip (cr, new Cairo.Point (win.Allocation.X, win.Allocation.Y), 0, 0, layout);
				
				((IDisposable)cr.Target).Dispose ();
				((IDisposable)cr).Dispose ();
			};
		}
		
		public void Hide ()
		{
			if(this.win == null) return;
			
			win.Hide ();
		}
	}
}
