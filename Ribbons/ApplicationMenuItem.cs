using System;
using Gtk;
using Cairo;

namespace Ribbons
{
	public class ApplicationMenuItem : Bin
	{
		private Widget img;
		private Label lbl;
		private Widget menu;
		
		protected Theme theme = new Theme ();
		
		[GLib.Signal("action")]
		public event EventHandler Action;
		
		/// <summary>Image to display.</summary>
		public Widget Image
		{
			set
			{
				if(img == value) return;
				if(img != null) UnbindWidget (img);
				img = value;
				if(img != null) BindWidget (img);
				UpdateImageLabel ();
			}
			get { return img; }
		}
	
		/// <summary>Label to display.</summary>
		public string Label
		{
			set
			{
				if(lbl != null) UnbindWidget (lbl);
				lbl = new Gtk.Label (value);
				if(lbl != null) BindWidget (lbl);
				UpdateImageLabel ();
			}
			get
			{
				return lbl == null ? null : lbl.Text;
			}
		}
		
		public Widget Menu
		{
			get { return menu; }
			set
			{
				menu = value;
				QueueResize ();
			}
		}
		
		/// <summary>Default constructor.</summary>
		public ApplicationMenuItem ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		/// <summary>Constructor given a label to display.</summary>
		/// <param name="Label">Label to display.</param>
		public ApplicationMenuItem (string Label) : this ()
		{
			this.Label = Label;
		}
		
		/// <summary>Constructor given an image to display.</summary>
		/// <param name="Image">Image to display</param>
		public ApplicationMenuItem (Image Image) : this ()
		{
			this.Image = Image;
		}
		
		/// <summary>Constructor given a label and an image to display.</summary>
		/// <param name="Image">Image to display.</param>
		/// <param name="Label">Label to display.</param>
		public ApplicationMenuItem (Image Image, string Label) : this ()
		{
			this.Image = Image;
			this.Label = Label;
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static ApplicationMenuItem FromStockIcon (string Name, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			return new ApplicationMenuItem (img);
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Label">Label to display.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static ApplicationMenuItem FromStockIcon (string Name, string Label, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			return new ApplicationMenuItem (img, Label);
		}
		
		/// <summary>Fires the Action event.</summary>
		public void Click ()
		{
			if(Action != null) Action (this, EventArgs.Empty);
		}
		
		/// <summary>Updates the child widget containing the label and/or image.</summary>
		protected void UpdateImageLabel ()
		{
			if(Child != null)
			{
				Container con = Child as Container;
				if(con != null)
				{
					con.Remove (img);
					con.Remove (lbl);
				}
				Remove (Child);
			}
			
			if(lbl != null && img != null)
			{
				HBox box = new HBox (false, 0);
				box.Add (img);
				box.Add (lbl);
				Child = box;
			}
			else if(lbl != null)
			{
				Child = lbl;
			}
			else if(img != null)
			{
				Child = img;
			}
		}
		
		/// <summary>Binds a widget to listen to all button events.</summary>
		protected void BindWidget (Widget w)
		{
			w.ButtonPressEvent += BindedWidget_ButtonPressEvent;
			w.ButtonReleaseEvent += BindedWidget_ButtonReleaseEvent;
		}
		
		/// <summary>Unbinds a widget to no longer listen to button events.</summary>
		protected void UnbindWidget (Widget w)
		{
			w.ButtonPressEvent -= BindedWidget_ButtonPressEvent;
			w.ButtonReleaseEvent -= BindedWidget_ButtonReleaseEvent;
		}
		
		protected void BindedWidget_ButtonPressEvent (object sender, ButtonPressEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
		}
		
		protected void BindedWidget_ButtonReleaseEvent (object sender, ButtonReleaseEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
			Click ();
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(Child != null)
			{
				requisition = Child.SizeRequest ();
			}
		}
		
		/*protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
		}*/
		
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
			Rectangle rect = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			theme.DrawApplicationMenuItem (cr, rect, this);
		}
	}
}
