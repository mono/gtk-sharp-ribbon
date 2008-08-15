using System;
using Gtk;

namespace Ribbons
{
	public class DropdownRibbonGroup : ToggleButton
	{
		private RibbonGroup group;
		private SyntheticWindow win;
		
		public RibbonGroup Group
		{
			get { return group; }
			set { group = value; }
		}
		
		public DropdownRibbonGroup ()
		{
			DrawBackground = true;
			ImagePosition = PositionType.Top;
			isSmall = false;
			DisplayArrow = true;
		}
		
		protected override void BindedWidget_ButtonReleaseEvent (object sender, ButtonReleaseEventArgs evnt)
		{
			base.BindedWidget_ButtonReleaseEvent (sender, evnt);
			
			if(Value)
			{
				int x, y;
				ParentWindow.GetOrigin (out x, out y);
				x += Allocation.X;
				y += Allocation.Bottom;
				
				ShowAt (x, y);
			}
			else
			{
				KillMenu (true);
			}
		}
		
		private void ShowAt (int x, int y)
		{
			if(win != null) return;
			
			win = new SyntheticWindow (WindowType.Popup);
			win.Child = group;
			
			win.Hidden += delegate { KillMenu (true); };
			
			win.ShowAll ();
			win.GdkWindow.Move (x, y);
			
			win.ButtonReleaseEvent += delegate { KillMenu (true); };
			win.AddEvents ((int)(Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			Grab.Add (win);
			Gdk.GrabStatus grabbed = Gdk.Pointer.Grab (win.GdkWindow, true, Gdk.EventMask.ButtonPressMask, null, null, 0);
			if(grabbed != Gdk.GrabStatus.Success)
			{
				KillMenu (false);
				return;
			}
			
			grabbed = Gdk.Keyboard.Grab (win.GdkWindow, true, 0);
			if(grabbed != Gdk.GrabStatus.Success)
			{
				KillMenu (false);
				return;
			}
		}
		
		private void KillMenu (bool Ungrab)
		{
			if(win == null) return;
			
			Grab.Remove (win);
			if(Ungrab)
			{
				Gdk.Pointer.Ungrab (0);
				Gdk.Keyboard.Ungrab (0);
			}
			win.Hide ();
			group.Unparent ();
			win = null;
			
			Value = false;
		}
	}
}
