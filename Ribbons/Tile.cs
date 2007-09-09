using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>The Tile widget.</summary>
	public abstract class Tile : Widget
	{
		protected Theme theme = new Theme ();
		private bool selected;
		private uint borderWidth;
		
		/// <summary>Gets or sets the width of the border.</summary>
		public uint BorderWidth
		{
			set
			{
				borderWidth = value;
				QueueDraw ();
			}
			get { return borderWidth; }
		}
		
		/// <summary>Gets or sets the state of the Tile.</summary>
		public bool Selected
		{
			set
			{
				selected = value;
				QueueDraw ();
			}
			get { return selected; }
		}
		
		/// <summary>Fired when the Tile has been clicked.</summary>
		public event EventHandler Clicked;
		
		/// <summary>Theme used to draw the widget.</summary>
		public Theme Theme
		{
			set
			{
				theme = value;
				QueueDraw ();
			}
			get { return theme; }
		}
		
		/// <summary>Default constructor.</summary>
		public Tile ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.selected = false;
			this.BorderWidth = 4;
		}
		
		/// <summary>Creates a carbon copy of the current Tile.</summary>
		public abstract Tile Copy ();
		
		/// <summary>Fires the Click event.</summary>
		public void Click ()
		{
			if(Clicked != null) Clicked (this, EventArgs.Empty);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (this.GdkWindow);
			
			Cairo.Rectangle area = new Cairo.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			Cairo.Rectangle allocation = new Cairo.Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			Cairo.Rectangle contentArea = new Cairo.Rectangle (allocation.X + BorderWidth, allocation.Y + BorderWidth, allocation.Width - 2 * BorderWidth, allocation.Height - 2 * BorderWidth);
			
			cr.Rectangle (area);
			cr.Clip ();
			theme.DrawTile (cr, allocation, contentArea, this);
			
			DrawContent (cr, contentArea);
			
			return base.OnExposeEvent (evnt);
		}
		
		/// <summary>
		/// Draws the content of the tile.
		/// </summary>
		/// <param name="Context">Cairo context to be used to draw the content.</param>
		/// <param name="Area">Area that can be painted.</param>
		public abstract void DrawContent (Cairo.Context Context, Cairo.Rectangle Area);
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonPressEvent (evnt);
			
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonReleaseEvent (evnt);
			Click ();
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnEnterNotifyEvent (evnt);
			
			this.QueueDraw ();
			return ret;
		}
		
		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnLeaveNotifyEvent (evnt);
			
			this.QueueDraw ();
			return ret;
		}
	}
}
