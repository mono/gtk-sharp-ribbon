using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>Set of ribbon buttons packed together.</summary>
	public class ToolPack : Container
	{
		private List<BaseButton> buttons;
		private int[] widths;
		
		/// <summary>Default constructor.</summary>
		public ToolPack ()
		{
			this.buttons = new List<BaseButton> ();
			
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		/// <summary>Adds a button before all existing buttons.</summary>
		/// <param name="Widget">The button to add.</param>
		public void PrependButton (BaseButton Widget)
		{
			InsertButton (Widget, 0);
		}
		
		/// <summary>Adds a button after all existing buttons.</summary>
		/// <param name="Widget">The button to add.</param>
		public void AppendButton (BaseButton Widget)
		{
			InsertButton (Widget, -1);
		}
		
		/// <summary>Inserts a button at the specified location.</summary>
		/// <param name="Widget">The button to add.</param>
		/// <param name="ButtonIndex">The index (starting at 0) at which the button must be inserted, or -1 to insert the button after all existing buttons.</param>
		public void InsertButton (BaseButton Widget, int ButtonIndex)
		{
			Widget.Parent = this;
			Widget.Visible = true;
			
			Widget.DrawBackground = true;
			
			if(ButtonIndex == -1 || ButtonIndex == buttons.Count)
			{
				if(buttons.Count == 0)
				{
					Widget.GroupStyle = GroupStyle.Alone;
				}
				else
				{
					Widget.GroupStyle = GroupStyle.Right;
					
					if(buttons.Count == 1)
					{
						buttons[buttons.Count - 1].GroupStyle = GroupStyle.Left;
					}
					else if(buttons.Count > 1)
					{
						buttons[buttons.Count - 1].GroupStyle = GroupStyle.Center;
					}
				}
				buttons.Add (Widget);
			}
			else
			{
				if(ButtonIndex == 0)
				{
					buttons[buttons.Count - 1].GroupStyle = GroupStyle.Left;
					if(buttons.Count == 1)
					{
						buttons[0].GroupStyle = GroupStyle.Right;
					}
					else
					{
						buttons[0].GroupStyle = GroupStyle.Center;
					}
				}
				buttons.Insert (ButtonIndex, Widget);
			}
			
			ShowAll ();
		}
		
		/// <summary>Removes the button at the specified index.</summary>
		/// <param name="ButtonIndex">Index of the button to remove.</param>
		public void RemoveButton (int ButtonIndex)
		{
			buttons[ButtonIndex].Parent = null;
			
			if(ButtonIndex == 0)
			{
				if(buttons.Count > 1)
				{
					if(buttons.Count > 2)
					{
						buttons[0].GroupStyle = GroupStyle.Left;
					}
					else
					{
						buttons[0].GroupStyle = GroupStyle.Alone;
					}
				}
			}
			else if(ButtonIndex == buttons.Count - 1)
			{
				if(buttons.Count > 1)
				{
					if(buttons.Count > 2)
					{
						buttons[0].GroupStyle = GroupStyle.Right;
					}
					else
					{
						buttons[0].GroupStyle = GroupStyle.Alone;
					}
				}
			}
			buttons.RemoveAt (ButtonIndex);
			
			ShowAll ();
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(BaseButton b in buttons)
			{
				if(b.Visible) callback (b);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(widths == null || widths.Length != buttons.Count)
			{
				widths = new int[buttons.Count];
			}
			
			requisition.Height = requisition.Width = 0;
			int i = 0;
			foreach(BaseButton b in buttons)
			{
				Gtk.Requisition req = b.SizeRequest ();
				if(requisition.Height < req.Height) requisition.Height = req.Height;
				requisition.Width += req.Width;
				widths[i++] = req.Width;
			}
			if(HeightRequest != -1) requisition.Height = HeightRequest;
			if(WidthRequest != -1) requisition.Width = WidthRequest;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			int i = 0, x = allocation.X;
			foreach(BaseButton b in buttons)
			{
				Gdk.Rectangle r;
				r.X = x;
				r.Y = allocation.Y;
				r.Width = widths[i];
				r.Height = allocation.Height;
				b.SizeAllocate (r);
				x += r.Width;
				++i;
			}
		}
	}
}
