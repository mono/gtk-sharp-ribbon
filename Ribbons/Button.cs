using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>Button to be used in Ribbons.</summary>
	public class Button : BaseButton
	{
		private Menu dropDownMenu;
		
		private double arrowSize;
		private Gdk.Rectangle arrowAllocation;
		
		protected const double lineWidth = 1.0;
		protected const double arrowPadding = 2.0;
		protected const double smallArrowSize = 5.0;
		protected const double bigArrowSize = 8.0;
		
		/// <summary>Fired when the button is clicked.</summary>
		public event EventHandler Clicked;
		
		/// <summary>Drop down menu displayed when the arrow is pressed.</summary>
		public Menu DropDownMenu
		{
			set
			{
				dropDownMenu = value;
				QueueDraw ();
			}
			get
			{
				return dropDownMenu;
			}
		}
		
		/// <summary>Default constructor.</summary>
		public Button ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.Padding = 2;
			this.ImagePosition = PositionType.Top;
			this.isSmall = false;
			this.enable = true;
		}
		
		/// <summary>Constructor given a label to display.</summary>
		/// <param name="Label">Label to display.</param>
		public Button (string Label) : this ()
		{
			this.Label = Label;
		}
		
		/// <summary>Constructor given an image to display.</summary>
		/// <param name="Image">Image to display</param>
		public Button (Image Image) : this ()
		{
			this.Image = Image;
		}
		
		/// <summary>Constructor given a label and an image to display.</summary>
		/// <param name="Image">Image to display.</param>
		/// <param name="Label">Label to display.</param>
		public Button (Image Image, string Label) : this ()
		{
			this.Image = Image;
			this.Label = Label;
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static Button FromStockIcon (string Name, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			Button btn = new Button (img);
			if(!Large) btn.ImagePosition = PositionType.Left;
			return btn;
		}
		
		/// <summary>Constructs a Button from a stock.</summary>
		/// <param name="Name">Name of the stock.</param>
		/// <param name="Label">Label to display.</param>
		/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
		public static Button FromStockIcon (string Name, string Label, bool Large)
		{
			Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
			Button btn = new Button (img, Label);
			if(!Large) btn.ImagePosition = PositionType.Left;
			return btn;
		}
		
		/// <summary>Fires the Click event.</summary>
		public void Click ()
		{
			if(enable && Clicked != null) Clicked (this, EventArgs.Empty);
		}
		
		/// <summary>Displays the drop down menu if any.</summary>
		public void Popup ()
		{
			if(enable && dropDownMenu != null)
			{
				dropDownMenu.Popup ();
				dropDownMenu.ShowAll ();
			}
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
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			Requisition childRequisition = new Requisition ();
			if(Child != null && Child.Visible)
			{
				childRequisition = Child.SizeRequest ();
			}
			
			if(dropDownMenu != null)
			{
				int arrowSpace = (int)((isSmall ? smallArrowSize : bigArrowSize) + 2 * (lineWidth + arrowPadding));
				
				if(imgPos == PositionType.Top || imgPos == PositionType.Bottom)
				{
					childRequisition.Height += arrowSpace;
				}
				else
				{
					childRequisition.Width += arrowSpace;
				}
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
			
			if(dropDownMenu != null)
			{
				arrowSize = isSmall ? smallArrowSize : bigArrowSize;
				
				if(imgPos == PositionType.Top || imgPos == PositionType.Bottom)
				{
					if(Clicked != null)
						arrowAllocation.Height = (int)(arrowSize + 2 * arrowPadding);
					else
						arrowAllocation.Height = (int)(allocation.Height - 4 * lineWidth);
					
					arrowAllocation.Width = (int)(allocation.Width - 4 * lineWidth);
				}
				else
				{
					if(Clicked != null)
						arrowAllocation.Width = (int)(arrowSize + 2 * arrowPadding);
					else
						arrowAllocation.Width = (int)(allocation.Width - 4 * lineWidth);
					
					arrowAllocation.Height = (int)(allocation.Height - 4 * lineWidth);
				}
				
				arrowAllocation.X = (int)(allocation.Right - arrowAllocation.Width - 2 * lineWidth);
				arrowAllocation.Y = (int)(allocation.Bottom - arrowAllocation.Height - 2 * lineWidth);
			}
			else
			{
				arrowSize = 0;
			}
			
			allocation.X += (int)(lineWidth * 2 + padding);
			allocation.Y += (int)(lineWidth * 2 + padding);
			allocation.Height -= (int)(lineWidth * 4 + padding * 2);
			allocation.Width -= (int)(lineWidth * 4 + padding * 2);
			
			if(dropDownMenu != null)
			{
				int arrowSpace = (int)((isSmall ? smallArrowSize : bigArrowSize) + 2 * (lineWidth + arrowPadding));
				
				if(imgPos == PositionType.Top || imgPos == PositionType.Bottom)
				{
					allocation.Height -= arrowSpace;
				}
				else
				{
					allocation.Width -= arrowSpace;
				}
			}
			
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
			bool drawSeparator = (Clicked != null) && (dropDownMenu != null);
			theme.DrawButton (cr, rect, state, roundSize, lineWidth, arrowSize, arrowPadding, drawSeparator, this);
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			bool ret = base.OnButtonPressEvent (evnt);
			state = Theme.ButtonState.Pressed;
			if(!enable) state = Theme.ButtonState.Default;
			this.QueueDraw ();
			
			if(dropDownMenu != null && arrowAllocation.Contains ((int)evnt.X, (int)evnt.Y))
			{
				Popup ();
			}
			
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
