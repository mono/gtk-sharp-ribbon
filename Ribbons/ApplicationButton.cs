using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>
	/// A button used to display the application menu. This button has a theme different than all other buttons. 
	/// </summary>
	public class ApplicationButton : BaseButton
	{
		protected const double lineWidth = 1.0;
		
		private ApplicationMenu appMenu;
		private bool menuOpened;
		private KeyTip keyTip;
		
		/// <summary>Fired when the button is clicked.</summary>
		[GLib.Signal("clicked")]
		public event EventHandler Clicked;
		
		public ApplicationMenu Menu
		{
			get { return appMenu; }
		}
		
		public KeyTip KeyTip
		{
			get { return keyTip; }
			set { keyTip = value; }
		}
		
		public ApplicationButton()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			menuOpened = false;
			
			appMenu = new ApplicationMenu (this);
			appMenu.Hidden += appMenu_Hidden;
			
			HeightRequest = 36;
			WidthRequest = 36;
			
			this.enable = true;
		}
		
		/// <summary>Fires the Click event.</summary>
		public void Click ()
		{
			if(enable && Clicked != null) Clicked (this, EventArgs.Empty);
			
			int x, y;
			ParentWindow.GetOrigin (out x, out y);
			x += Allocation.X;
			y += Allocation.Bottom;
			
			appMenu.ShowAt (x, y);
			menuOpened = true;
			QueueDraw ();
		}
		
		private void appMenu_Hidden (object sender, EventArgs args)
		{
			menuOpened = false;
			QueueDraw ();
		}
		
		protected override void BindedWidget_ButtonPressEvent (object sender, ButtonPressEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
		}
		
		protected override void BindedWidget_ButtonReleaseEvent (object sender, ButtonReleaseEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
			Click ();
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (this.GdkWindow);
			
			cr.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip ();
			Draw (cr);
			
			((IDisposable)cr.Target).Dispose ();
			((IDisposable)cr).Dispose ();
			
			return base.OnExposeEvent (evnt);
		}
		
		protected void Draw (Context cr)
		{
			Theme.ButtonState state = this.state;
			if(menuOpened) state = Theme.ButtonState.Pressed;
			theme.DrawApplicationButton (cr, Allocation, state, lineWidth, this);
		}
		
		public void ShowKeyTips ()
		{
			if(keyTip != null)
			{
				Gdk.Rectangle alloc = Allocation;
				int x, y;
				GdkWindow.GetOrigin (out x, out y);
				keyTip.ShowAt (x + alloc.X + (alloc.Width >> 1), y + alloc.Y + (alloc.Height >> 1), 0.5, 0.5);
			}
		}
		
		public void HideKeyTips ()
		{
			if(keyTip != null)
			{
				keyTip.Hide ();
			}
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonPressEvent (evnt);
			state = Theme.ButtonState.Pressed;
			if(!enable) state = Theme.ButtonState.Default;
			this.QueueDraw ();
			Click ();
			return ret;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonReleaseEvent (evnt);
			state = Theme.ButtonState.Hover;
			if(!enable) state = Theme.ButtonState.Default;
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnEnterNotifyEvent (evnt);
			state = Theme.ButtonState.Hover;
			if(!enable) state = Theme.ButtonState.Default;
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnLeaveNotifyEvent (evnt);
			state = Theme.ButtonState.Default;
			this.QueueDraw ();
			return ret;
		}
	}
}
