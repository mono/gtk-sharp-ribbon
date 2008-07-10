using System;
using Gtk;
using Cairo;

namespace Ribbons
{
	public class ApplicationMenuItem : Bin
	{
		private const double lineWidth = 1.0;
		private const double arrowPadding = 2.0;
		private const double arrowSize = 10.0;
		private const double roundSize = 3.0;
		
		private Theme.ButtonState state = Theme.ButtonState.Default;
		private int padding = 2;
		private Gdk.Rectangle arrowAllocation;
		private double effectiveArrowSize;
		
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
		
		private void ActivateMenu ()
		{
			ApplicationMenu win = Parent as ApplicationMenu;
			if(win != null)
			{
				win.ActivateMenu (menu);
			}
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
			
			Requisition childRequisition = new Requisition ();
			if(Child != null)
			{
				childRequisition = Child.SizeRequest ();
			}
			
			if(Menu != null)
			{
				int arrowSpace = (int)(arrowSize + 2 * (lineWidth + arrowPadding));
				childRequisition.Width += arrowSpace;
			}
			
			if(HeightRequest == -1)
			{
				requisition.Height = childRequisition.Height + (int)(lineWidth * 4 + padding * 2);
			}
			if(WidthRequest == -1)
			{
				requisition.Width = childRequisition.Width + (int)(lineWidth * 4 + padding * 2);
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			effectiveArrowSize = arrowSize;
			
			if(Menu != null)
			{
				if(Action != null)
					arrowAllocation.Width = (int)(arrowSize + 2 * arrowPadding);
				else
					arrowAllocation.Width = (int)(allocation.Width - 4 * lineWidth);
				
				arrowAllocation.Height = (int)(allocation.Height - 4 * lineWidth);
				
				arrowAllocation.X = (int)(allocation.Right - arrowAllocation.Width - 2 * lineWidth);
				arrowAllocation.Y = (int)(allocation.Bottom - arrowAllocation.Height - 2 * lineWidth);
			}
			else
			{
				effectiveArrowSize = 0;
			}
			
			allocation.X += (int)(lineWidth * 2 + padding);
			allocation.Y += (int)(lineWidth * 2 + padding);
			allocation.Height -= (int)(lineWidth * 4 + padding * 2);
			allocation.Width -= (int)(lineWidth * 4 + padding * 2);
			
			if(Menu != null)
			{
				int arrowSpace = (int)(effectiveArrowSize + 2 * (lineWidth + arrowPadding));
				allocation.Width -= arrowSpace;
			}
			
			if(allocation.Height < 0) allocation.Height = 0;
			if(allocation.Width < 0) allocation.Width = 0;
			
			if(Child != null)
			{
				Child.SizeAllocate (allocation);
			}
		}
		
		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evnt)
		{
			bool ret = base.OnEnterNotifyEvent (evnt);
			state = Theme.ButtonState.Hover;
			//if(!enable) state = Theme.ButtonState.Default;
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
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonPressEvent (evnt);
			state = Theme.ButtonState.Pressed;
			//if(!enable) state = Theme.ButtonState.Default;
			this.QueueDraw ();
			
			if(Menu != null && arrowAllocation.Contains ((int)evnt.X, (int)evnt.Y))
			{
				ActivateMenu ();
			}
			
			return ret;
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
			Rectangle rect = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			bool drawSeparator = Action != null && Menu != null;
			theme.DrawApplicationMenuItem (cr, rect, state, roundSize, lineWidth, effectiveArrowSize, arrowPadding, drawSeparator, this);
		}
	}
}
