using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	/// <summary>ToolBox containing several widgets displayed in rows.</summary>
	public class ToolBox : Container
	{
		private List<Widget> widgets;
		private Gtk.Requisition[] requisitions;
		private int spacing;
		
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
		
		/// <summary>Default constructor.</summary>
		public ToolBox ()
		{
			this.widgets = new List<Widget> ();
			
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			spacing = 2;
		}
		
		/// <summary>Adds a widget before all existing widgetw.</summary>
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
			
			if(requisitions == null || requisitions.Length != widgets.Count)
			{
				requisitions = new Gtk.Requisition[widgets.Count];
			}
			
			int totalWidth = 0, rowHeight = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					rowHeight = Math.Max (rowHeight, w.SizeRequest ().Height);
				}
			}
			
			int i = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					w.HeightRequest = rowHeight;
					requisitions[i] = w.SizeRequest ();
					totalWidth += requisitions[i].Width;
				}
				++i;
			}
			
			if(WidthRequest != -1 && HeightRequest != -1)
			{
				requisition.Width = WidthRequest;
				requisition.Height = HeightRequest;
			}
			else if(WidthRequest != -1)
			{
				int totalHeight = rowHeight, curWidth = 0;
				int availWidth = WidthRequest - 2*(int)BorderWidth;
				
				i = 0;
				foreach(Widget w in widgets)
				{
					if(w.Visible)
					{
						Gtk.Requisition r = requisitions[i];
						
						if(curWidth == 0 || curWidth + r.Width <= availWidth)
						{	// Continue current line
							curWidth += r.Width;
							if(curWidth != 0) curWidth += spacing;
						}
						else
						{	// Start new line
							totalHeight += rowHeight + spacing;
							curWidth = 0;
						}
					}
					++i;
				}
				
				requisition.Width = WidthRequest;
				requisition.Height = totalHeight + 2*(int)BorderWidth;
			}
			else
			{
				int rowsLeft = (int)Math.Floor ((double)(HeightRequest + spacing) / (double)(rowHeight + spacing));
				if(rowsLeft == 0) rowsLeft = 1;
				int widthLeft = totalWidth;
				int curWidth = 0, maxWidth = 0;
				int minWidth = widthLeft / rowsLeft;
				
				i = 0;
				int currentWidgetCounter = 0;
				foreach(Widget w in widgets)
				{
					if(w.Visible)
					{
						Gtk.Requisition r = requisitions[i];
						
						widthLeft -= r.Width;
						curWidth += r.Width;
						++currentWidgetCounter;
						
						if(curWidth >= minWidth)
						{	// Start new line
							curWidth += (currentWidgetCounter - 1) * spacing;
							maxWidth = Math.Max (maxWidth, curWidth);
							curWidth = 0;
							--rowsLeft;
							if(rowsLeft == 0) break;
							minWidth = widthLeft / rowsLeft;
							currentWidgetCounter = 0;
						}
					}
					++i;
				}
				
				requisition.Width = maxWidth + 2*(int)BorderWidth;
				
				if(HeightRequest == -1)
					requisition.Height = rowHeight;
				else
					requisition.Height = HeightRequest;
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			int right = allocation.X + allocation.Width - (int)BorderWidth;
			int left = allocation.X + (int)BorderWidth;
			int bottom = allocation.Y + allocation.Height - (int)BorderWidth;
			int x = left, rowY = allocation.Y + (int)BorderWidth;
			int maxHeight = 0;
			
			int i = 0;
			foreach(Widget w in widgets)
			{
				if(w.Visible)
				{
					Gdk.Rectangle r;
					r.Width = requisitions[i].Width;
					r.Height = requisitions[i].Height;
					
					if(x > left && x + r.Width > right)
					{
						rowY += maxHeight + spacing;
						maxHeight = 0;
						x = left;
					}
					
					r.X = x;
					r.Y = rowY;
					r.Width = Math.Min (right, r.X + r.Width) - r.X;
					r.Height = Math.Min (bottom, r.Y + r.Height) - r.Y;
					w.SizeAllocate (r);
					
					x += r.Width + spacing;
					maxHeight = Math.Max (maxHeight, r.Height);
				}
				++i;
			}
		}
	}
}
