using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class QuickAccessToolbar : Container
	{
		private List<Widget> widgets;
		private int[] widths;
		
		public QuickAccessToolbar()
		{
			widgets = new List<Widget> ();
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
			widgets[WidgetIndex].Parent = null;
			
			if(WidgetIndex == -1)
				widgets.RemoveAt (widgets.Count - 1);
			else
				widgets.RemoveAt (WidgetIndex);
			
			ShowAll ();
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
				b.HeightRequest = requisition.Height;
				Gtk.Requisition req = b.SizeRequest ();
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
	}
}
