using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	public class ApplicationButton : BaseButton
	{
		protected const double lineWidth = 1.0;
		
		private ApplicationMenu appMenu;
		
		/// <summary>Fired when the button is clicked.</summary>
		[GLib.Signal("clicked")]
		public event EventHandler Clicked;
		
		public ApplicationMenu Menu
		{
			get { return appMenu; }
		}
		
		public ApplicationButton()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			appMenu = new ApplicationMenu (this);
			
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
			theme.DrawApplicationButton (cr, Allocation, state, lineWidth, this);
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
