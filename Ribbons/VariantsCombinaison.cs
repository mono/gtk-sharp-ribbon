using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	// Note: this widget shall display a button at the left and the right
	// to scroll groups when the allocated with is not enough.
	
	/// <summary>
	/// A set of widgets displayed horizontaly.
	/// </summary>
	/// <remarks>
	/// On the contrary of a classical container, the VariantsCombinaison is only
	/// the parent of its children if it has a parent itself.
	/// </remarks>
	public class VariantsCombinaison : Container
	{
		private List<Widget> widgets;
		private Button left, right;
		private Gtk.Requisition[] requisitions;
		private int spacing;
		
		public VariantsCombinaison ()
		{
			Init ();
		}
		
		protected VariantsCombinaison (IntPtr raw) : base (raw)
		{
			Init ();
		}
		
		private void Init ()
		{
			this.widgets = new List<Widget> ();
			
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.spacing = 2;
		}
		
		/*public void Measure (int Height)
		{
			width = 0;
			foreach(Widget variant in variants)
			{
				variant.Measure (Height);
				width += variant.Width;
			}
		}*/
		
		/// <summary>Gets or sets the spacing between children.</summary>
		public int Spacing
		{
			set
			{
				spacing = value;
				QueueDraw ();
			}
			get { return spacing; }
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
			if(Parent != null) w.Parent = this;
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
			if(Parent != null) widgets[WidgetIndex].Parent = null;
			
			if(WidgetIndex == -1)
				widgets.RemoveAt (widgets.Count - 1);
			else
				widgets.RemoveAt (WidgetIndex);
			
			ShowAll ();
		}
		
		protected override void OnParentSet (Widget previous_parent)
		{
			base.OnParentSet (previous_parent);
			
			if(Parent == null)
			{Console.WriteLine ("unparnet");
				foreach(Widget w in widgets) w.Unparent ();
			}
			else
			{Console.WriteLine ("ok");
				foreach(Widget w in widgets) w.Parent = this;
			}
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			//if(Parent == null) return;
			
			foreach(Widget w in widgets)
			{
				if(w.Visible) callback (w);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			if(requisitions == null || requisitions.Length != widgets.Count)
			{
				requisitions = new Gtk.Requisition[widgets.Count];
			}
			
			int totalWidth = 0, rowHeight = 0;
			if(HeightRequest == -1)
			{
				foreach(Widget w in widgets)
				{
					if(w.Visible)
					{
						rowHeight = Math.Max (rowHeight, w.SizeRequest ().Height);
					}
				}
			}
			else rowHeight = HeightRequest - 2*(int)BorderWidth;
			
			int i = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					w.HeightRequest = rowHeight;
					requisitions[i] = w.SizeRequest ();
					totalWidth += requisitions[i].Width + spacing;
				}
				++i;
			}
			totalWidth -= spacing;
			
			requisition.Height = rowHeight;
			if(WidthRequest != -1)
				requisition.Width = WidthRequest;
			else
				requisition.Width = totalWidth;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			int right = allocation.X + allocation.Width - (int)BorderWidth;
			int left = allocation.X + (int)BorderWidth;
			int bottom = allocation.Y + allocation.Height - (int)BorderWidth;
			int x = left, y = allocation.Y + (int)BorderWidth;
			
			int i = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					Gdk.Rectangle r;
					r.X = x;
					r.Y = y;
					r.Width = Math.Min (right, r.X + requisitions[i].Width) - r.X;
					//r.Height = Math.Min (bottom, r.Y + requisitions[i].Height) - r.Y;
					r.Height = bottom - y;
					w.SizeAllocate (r);
					
					x += r.Width + spacing;
				}
				++i;
			}
		}
	}
}
