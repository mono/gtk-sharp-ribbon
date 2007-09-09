using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>Toggle button to be used in Ribbons.</summary>
	public class ToggleButton : BaseButton
	{
		private bool value;
		
		protected const double lineWidth = 1.0;
		
		public event EventHandler ValueChanged;
		
		public bool Value
		{
			set
			{
				if(this.value != value)
				{
					this.value = value;
					OnValueChanged ();
				}
			}
			get { return value; }
		}
		
		/// <summary>Default constructor.</summary>
		public ToggleButton ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.Padding = 2;
			this.ImagePosition = PositionType.Top;
			this.isSmall = false;
			this.enable = true;
			this.value = false;
		}
		
		/// <summary>Constructor given a label to display.</summary>
		/// <param name="Label">Label to display.</param>
		public ToggleButton (string Label) : this ()
		{
			this.Label = Label;
		}
		
		/// <summary>Constructor given an image to display.</summary>
		/// <param name="Image">Image to display</param>
		public ToggleButton (Image Image) : this ()
		{
			this.Image = Image;
		}
		
		/// <summary>Constructor given a label and an image to display.</summary>
		/// <param name="Image">Image to display.</param>
		/// <param name="Label">Label to display.</param>
		public ToggleButton (Image Image, string Label) : this ()
		{
			this.Image = Image;
			this.Label = Label;
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static ToggleButton FromStockIcon (string Name, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			ToggleButton btn = new ToggleButton (img);
			if(!Large) btn.ImagePosition = PositionType.Left;
			return btn;
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Label">Label to display.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static ToggleButton FromStockIcon (string Name, string Label, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			ToggleButton btn = new ToggleButton (img, Label);
			if(!Large) btn.ImagePosition = PositionType.Left;
			return btn;
		}
		
		protected override void BindedWidget_ButtonPressEvent (object sender, ButtonPressEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
		}
		
		protected override void BindedWidget_ButtonReleaseEvent (object sender, ButtonReleaseEventArgs evnt)
		{
			ProcessEvent (evnt.Event);
			Value = !Value;
			QueueDraw ();
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			Requisition childRequisition = new Requisition ();
			if(Child != null && Child.Visible)
			{
				childRequisition = Child.SizeRequest ();
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
			
			allocation.X += (int)(lineWidth * 2 + padding);
			allocation.Y += (int)(lineWidth * 2 + padding);
			allocation.Height -= (int)(lineWidth * 4 + padding * 2);
			allocation.Width -= (int)(lineWidth * 4 + padding * 2);
			
			if(allocation.Height < 0) allocation.Height = 0;
			if(allocation.Width < 0) allocation.Width = 0;
			
			if(Child != null && Child.Visible)
			{
				Child.SizeAllocate (allocation);
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (this.GdkWindow);
			
			cr.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip ();
			Draw (cr);
			
			return base.OnExposeEvent (evnt);
		}
		
		protected void Draw (Context cr)
		{
			Rectangle rect = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			double roundSize = isSmall ? 2.0 : 3.0;
			Theme.ButtonState s = this.state;
			if(this.value) s = Theme.ButtonState.Pressed;
			theme.DrawButton (cr, rect, s, roundSize, lineWidth, 0, 0, false, this);
		}
		
		protected virtual void OnValueChanged ()
		{
			if(ValueChanged != null) ValueChanged (this, EventArgs.Empty);
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonPressEvent (evnt);
			state = Theme.ButtonState.Pressed;
			if(!enable) state = Theme.ButtonState.Default;
			this.QueueDraw ();
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
