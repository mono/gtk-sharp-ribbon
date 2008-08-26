using System;
using System.Collections.Generic;
using Gtk;
using Cairo;

namespace Ribbons
{
	/// <summary>
	/// Displays several widgets (typical shortcuts to common functionalities) next to the application button.
	/// </summary>
	public class QuickAccessToolbar : Container
	{
		private List<Widget> widgets;
		private List<KeyTip> keyTips;
		private int[] widths;
		private Theme theme = Theme.DefaultTheme;
		
		public QuickAccessToolbar()
		{
			// This is a No Window widget => it does not have its own Gdk Window => it can be transparent
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.widgets = new List<Widget> ();
			this.keyTips = new List<KeyTip> ();
		}
		
		/// <summary>Adds a widget before all existing widgets.</summary>
		/// <param name="w">The widget to add.</param>
		public void Prepend (Widget w)
		{
			Insert (w, 0);
		}
		
		/// <summary>Adds a widget after all existing widgets.</summary>
		/// <param name="w">The widget to add.</param>
		public void Append (Widget w)
		{
			Insert (w, -1);
		}
		
		/// <summary>Inserts a widget at the specified location.</summary>
		/// <param name="w">The widget to add.</param>
		/// <param name="WidgetIndex">The index (starting at 0) at which the widget must be inserted, or -1 to insert the widget after all existing widgets.</param>
		public void Insert (Widget w, int WidgetIndex)
		{
			w.Parent = this;
			w.Visible = true;
			
			if(WidgetIndex == -1)
				widgets.Add (w);
			else
				widgets.Insert (WidgetIndex, w);
			
			ShowAll ();
		}
		
		/// <summary>Removes the widget at the specified index.</summary>
		/// <param name="WidgetIndex">Index of the widget to remove.</param>
		public void Remove (int WidgetIndex)
		{
			if(WidgetIndex == -1) WidgetIndex = widgets.Count - 1;
			
			widgets[WidgetIndex].Parent = null;
			widgets.RemoveAt (WidgetIndex);
			
			ShowAll ();
		}
		
		public void AddKeyTip (KeyTip kt)
		{
			keyTips.Add (kt);
		}
		
		public void RemoveKeyTip (KeyTip kt)
		{
			keyTips.Remove (kt);
		}
		
		public void ClearKeyTips ()
		{
			keyTips.Clear ();
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(Widget w in widgets)
			{
				if(w.Visible) callback (w);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(widths == null || widths.Length != widgets.Count)
			{
				widths = new int[widgets.Count];
			}
			
			requisition.Height = 16;
			requisition.Width = 0;
			
			int i = 0;
			foreach(BaseButton b in widgets)
			{
				//b.HeightRequest = requisition.Height;
				Gtk.Requisition req = b.SizeRequest ();
				if(req.Height > requisition.Height) requisition.Height = req.Height;
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
			foreach(BaseButton b in widgets)
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
		
		public void ShowKeyTips ()
		{
			int x, y;
			GdkWindow.GetOrigin (out x, out y);
			int lineY = y + Allocation.Y + (int)(0.66666 * Allocation.Height);
			
			foreach(KeyTip kt in keyTips)
			{
				Gdk.Rectangle alloc = kt.Target.Allocation;
				kt.ShowAt (x + alloc.X + (alloc.Width >> 1), lineY, 0.5, 0.0);
			}
		}
		
		public void HideKeyTips ()
		{
			foreach(KeyTip kt in keyTips)
			{
				kt.Hide ();
			}
		}
	}
}
