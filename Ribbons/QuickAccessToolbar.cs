using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class QuickAccessToolbar : Container
	{
		private List<Widget> widgets;		
		
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
			
			requisition.Width = 16;
			requisition.Height = 16;
		}
	}
}
