using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>Ribbon widget.</summary>
	public class Ribbon : Container
	{
		private static double borderWidth = 2.0;
		private static double space = 2.0;
		private static double pagePadding = 3.0;
		private static double tabPadding = 4.0;
		private static double minimalTabsHorizontalPosition = 8.0;
		private static double lineWidth = 1.0;
		private static double roundSize = 4.0;
		
		protected ColorScheme colorScheme = new ColorScheme ();
		protected Theme theme = new Theme ();
		
		protected List<RibbonPage> pages;
		protected int curPageIndex;
		protected Widget shortcuts;
		private Gdk.Rectangle bodyAllocation, pageAllocation;
		
		private Gtk.Requisition shortcutsRequisition;
		private Gtk.Requisition pageRequisition;
		private double headerHeight;
		
		public event PageSelectedHandler PageSelected;
		public event PageAddedHandler PageAdded;
		public event PageMovedHandler PageMoved;
		public event PageRemovedHandler PageRemoved;
		
		/// <summary>Indix of the currently selected page.</summary>
		/// <remakrs>Returns -1 if no page is selected.</remarks>
		public int CurrentPageIndex
		{
			set
			{
				if(curPageIndex != -1)
				{
					CurrentPage.Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (false));
					CurrentPage.Page.Unparent ();
				}
				curPageIndex = value;
				if(curPageIndex != -1)
				{
					CurrentPage.Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (true));
					CurrentPage.Page.Parent = this;
				}
				
				ShowAll ();
				QueueDraw ();
			}
			get
			{
				return curPageIndex;
			}
		}
		
		/// <summary>Currently selected page.</summary>
		public RibbonPage CurrentPage
		{
			get
			{
				int idx = curPageIndex;
				return idx == -1 ? null : pages[idx];
			}
		}
		
		/// <summary>Number of pages.</summary>
		public int NPages
		{
			get { return pages.Count; }
		}
		
		/// <summary>Shortcuts widget.</summary>
		/// <remarks>The shortcuts widget is displayed next to the tabs.</remarks>
		public Widget Shortcuts
		{
			set
			{
				if(shortcuts != null) shortcuts.Unparent ();
				shortcuts = value;
				if(shortcuts != null)
				{
					shortcuts.Visible = true;
					shortcuts.Parent = this;
				}
				QueueDraw ();
			}
			get { return shortcuts; }
		}
		
		/// <summary>Theme used to draw the widget.</summary>
		public Theme Theme
		{
			set
			{
				theme = value;
				QueueDraw ();
			}
			get { return theme; }
		}
		
		/// <summary>Default constructor.</summary>
		public Ribbon()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.pages = new List<RibbonPage> ();
			this.curPageIndex = -1;
		}
		
		/// <summary>Adds a new page after all existing pages.</summary>
		/// <param name="Child">The widget to use as the content of the page.</param>
		/// <param name="Label">The widget to use as the tab.</param>
		public void AppendPage (Widget Child, Widget Label)
		{
			InsertPage (Child, Label, -1);
		}
		
		/// <summary>Adds a new page before all existing pages.</summary>
		/// <param name="Child">The widget to use as the content of the page.</param>
		/// <param name="Label">The widget to use as the tab.</param>
		public void PrependPage (Widget Child, Widget Label)
		{
			InsertPage (Child, Label, 0);
		}
		
		/// <summary>Adds a new page at the specified position.</summary>
		/// <param name="Child">The widget to use as the content of the page.</param>
		/// <param name="Label">The widget to use as the tab.</param>
		/// <param name="Position">The index (starting at 0) at which the page must be inserted, or -1 to insert the page after all existing pages.</param>
		public void InsertPage (Widget Child, Widget Label, int Position)
		{
			RibbonPage p = new RibbonPage (this, Child, Label);
			
			if(Position == -1)
			{
				pages.Add (p);
			}
			else
			{
				pages.Insert (Position, p);
				
				if(curPageIndex != -1)
				{
					if(Position <= curPageIndex)
						++curPageIndex;
				}
			}
			
			if(pages.Count == 1)
			{
				CurrentPageIndex = 0;
			}
			else
			{
				Label.ModifyFg (StateType.Normal, theme.GetForecolorForRibbonTabs (false));
			}
			
			Label.ButtonPressEvent += delegate (object sender, ButtonPressEventArgs evnt)
			{
				this.SelectRibbonPage (p);
			};
			
			Label.EnterNotifyEvent += delegate (object sender, EnterNotifyEventArgs evnt)
			{
				
			};
			
			Label.LeaveNotifyEvent += delegate (object sender, LeaveNotifyEventArgs evnt)
			{
				
			};
			
			OnPageAdded (new PageEventArgs (p));
			for(int idx = Position + 1 ; idx < pages.Count ; ++idx)
			{
				OnPageSelected (new PageEventArgs (pages[idx]));
			}
		}
		
		/// <summary>Removes the specified page.</summary>
		/// <param name="PageNumber">Index of the page to remove.</param>
		public void RemovePage (int PageNumber)
		{
			if(curPageIndex != -1)
			{
				if(PageNumber < curPageIndex)
				{
					--curPageIndex;
				}
				else if(PageNumber == curPageIndex)
				{
					curPageIndex = -1;
				}
			}
			
			RibbonPage p = pages[PageNumber];
			if(curPageIndex == -1)
				pages.RemoveAt (pages.Count - 1);
			else
				pages.RemoveAt (PageNumber);
			
			OnPageRemoved (new PageEventArgs (p));
		}
		
		/// <summary>Returns the index of the specified page given its content widget.</summary>
		/// <param name="Child">The content of the page whose index must be returned.</param>
		/// <returns>The index.</returns>
		public int PageNum (Widget Child)
		{
			// Since it is unlikely that the widget will containe more than
			// a dozen pages, it is just fine to do a linear search.
			for(int i = 0, i_up = pages.Count ; i < i_up ; ++i)
				if(pages[i].Page == Child)
					return i;
			return -1;
		}
		
		/// <summary>Returns the index of the specified page.</summary>
		/// <param name="Page">The page whose index must be returned.</param>
		/// <returns>The RibbonPage.</returns>
		public int RibbonPageNum (RibbonPage Page)
		{
			// Since it is unlikely that the widget will containe more than
			// a dozen pages, it is just fine to do a linear search.
			for(int i = 0, i_up = pages.Count ; i < i_up ; ++i)
				if(pages[i] == Page)
					return i;
			return -1;
		}
		
		/// <summary>Sets the label widget of the specified page.</summary>
		/// <param name="Page">The content of the page whose label must be modified.</param>
		/// <param name="Label">The new label widget.</param>
		public void SetPageLabel (Widget Child, Widget Label)
		{
			pages[PageNum (Child)].Label = Label;
		}
		
		/// <summary>Gets the label widget of the specified page.</summary>
		/// <param name="Child">The content of the page whose label must be returned.</param>
		/// <returns>The label widget.</returns>
		public Widget GetPageLabel (Widget Child)
		{
			return pages[PageNum (Child)].Label;
		}
		
		/// <summary>Returns the content widget of the n-th page.</summary>
		/// <param name="Position">Index of the page whose content has to be returned.</param>
		/// <returns>The n-th page.</returns>
		public Widget GetNthPage (int Position)
		{
			return pages[Position].Page;
		}
		
		/// <summary>Returns the n-th page.</summary>
		/// <param name="Position">Index of the page to return.</param>
		public RibbonPage GetNthRibbonPage (int Position)
		{
			return pages[Position];
		}
		
		/// <summary>Selects the specified page.</summary>
		/// <param name="Page">The page to select.</param>
		public void SelectRibbonPage (RibbonPage page)
		{
			int idx = RibbonPageNum (page);
			if(idx != -1) CurrentPageIndex = idx;
			OnPageSelected (new PageEventArgs (page));
		}
		
		/// <summary>Selects the previous page.</summary>
		public void PrevPage ()
		{
			int i = CurrentPageIndex;
			if(i > 0) CurrentPageIndex = i - 1;
		}
		
		/// <summary>Selects the next page.</summary>
		public void NextPage ()
		{
			int i = CurrentPageIndex;
			if(i < NPages - 1) CurrentPageIndex = i + 1;
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			if(Shortcuts != null && Shortcuts.Visible)
			{
				callback (Shortcuts);
			}
			
			foreach(RibbonPage p in pages) callback (p.Label);
			
			if(CurrentPage != null)
			{
				callback (CurrentPage.Page);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			RibbonPage page = CurrentPage;
			
			double tabsWidth = 0, tabsHeight = 0;
			foreach(RibbonPage p in pages)
			{
				Gtk.Requisition req = p.Label.SizeRequest ();
				tabsWidth += req.Width;
				tabsHeight = Math.Max (tabsHeight, req.Height);
				p.LabelRequisition = req;
			}
			tabsWidth += pages.Count * 2 * tabPadding;
			tabsHeight += 2 * tabPadding;
			
			double headerWidth = tabsWidth;
			
			if(shortcuts != null && shortcuts.Visible)
			{
				shortcutsRequisition = shortcuts.SizeRequest ();
				double x = shortcutsRequisition.Width + space;
				headerWidth += Math.Max (x, minimalTabsHorizontalPosition);
			}
			else
			{
				shortcutsRequisition = new Gtk.Requisition ();
				headerWidth += minimalTabsHorizontalPosition;
			}
			
			headerHeight = Math.Max (tabsHeight, shortcutsRequisition.Height);
			
			double pageWidth = 0, pageHeight = 0;
			if(page != null)
			{
				pageRequisition = page.Page.SizeRequest ();
				pageWidth = pageRequisition.Width + 2 * pagePadding;
				pageHeight = pageRequisition.Height + 2 * pagePadding;
			}
			else
			{
				pageRequisition = new Gtk.Requisition ();
			}
			
			double width = Math.Max (headerWidth, pageWidth);
			width = borderWidth + width + borderWidth;
			double height = borderWidth + headerHeight + pageHeight + borderWidth;
			
			requisition.Width = (int)Math.Ceiling (width - double.Epsilon);
			requisition.Height = (int)Math.Ceiling (height - double.Epsilon);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			RibbonPage page = CurrentPage;
			
			if(allocation.Height < headerHeight + borderWidth) return;
			
			double headerBottom = allocation.X + borderWidth + headerHeight;
			double currentX = space;
			
			if(shortcuts != null && shortcuts.Visible)
			{
				Gdk.Rectangle alloc;
				alloc.X = (int)currentX;
				alloc.Y = (int)(headerBottom - shortcutsRequisition.Height);
				alloc.Width = shortcutsRequisition.Width;
				alloc.Height = shortcutsRequisition.Height;
				shortcuts.SizeAllocate (alloc);
				currentX += shortcutsRequisition.Width;
			}
			currentX += space;
			currentX = Math.Max (currentX, minimalTabsHorizontalPosition);
			
			foreach(RibbonPage p in pages)
			{
				Gdk.Rectangle alloc;
				alloc.X = (int)(currentX + tabPadding);
				alloc.Y = (int)(headerBottom - tabPadding - p.LabelRequisition.Height);
				alloc.Width = p.LabelRequisition.Width;
				alloc.Height = p.LabelRequisition.Height;
				p.Label.SizeAllocate (alloc);
				
				alloc.X = (int)currentX;
				alloc.Y = (int)(headerBottom - tabPadding - p.LabelRequisition.Height - tabPadding);
				alloc.Width = (int)(tabPadding + p.LabelRequisition.Width + tabPadding);
				alloc.Height = (int)(tabPadding + p.LabelRequisition.Height + tabPadding);
				p.SetLabelAllocation (alloc);
				
				currentX += p.LabelRequisition.Width + 2 * tabPadding;
			}
			
			bodyAllocation.X = allocation.X + (int)borderWidth;
			bodyAllocation.Y = (int)headerBottom;
			bodyAllocation.Width = allocation.Width - bodyAllocation.X  - (int)borderWidth;
			bodyAllocation.Height = allocation.Height - bodyAllocation.Y - (int)borderWidth;
			
			if(page != null)
			{
				pageAllocation = bodyAllocation;
				int pad = (int)pagePadding;
				pageAllocation.Inflate (-pad, -pad);
				page.Page.SizeAllocate (pageAllocation);
			}
			else
			{
				pageAllocation = Gdk.Rectangle.Zero;
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
			theme.DrawRibbon (cr, bodyAllocation, roundSize, lineWidth, this);
		}
		
		protected virtual void OnPageSelected (PageEventArgs args)
		{
			if(PageSelected != null) PageSelected (this, args);
		}
		
		protected virtual void OnPageAdded (PageEventArgs args)
		{
			if(PageAdded != null) PageAdded (this, args);
		}
		
		protected virtual void OnPageMoved (PageEventArgs args)
		{
			if(PageMoved != null) PageMoved (this, args);
		}
		
		protected virtual void OnPageRemoved (PageEventArgs args)
		{
			if(PageRemoved != null) PageRemoved (this, args);
		}
		
		/// <summary>Ribbon page.</summary>
		public class RibbonPage
		{
			private Ribbon parent;
			private Widget label, page;
			private Requisition labelReq;
			private Gdk.Rectangle labelAlloc;
			
			/// <summary>Label widget of the page.</summary>
			public Widget Label
			{
				set
				{
					if(label != null) label.Unparent ();
					label = value;
					if(label != null) label.Parent = parent;
				}
				get { return label; }
			}
			
			/// <summary>Widget used as the content of the page.</summary>
			public Widget Page
			{
				set { page = value; }
				get { return page; }
			}
			
			internal Requisition LabelRequisition
			{
				set { labelReq = value; }
				get { return labelReq; }
			}
			
			public Gdk.Rectangle LabelAllocation
			{
				get { return labelAlloc; }
			}
			
			public RibbonPage (Ribbon Parent, Widget Page, Widget Label)
			{
				parent = Parent;
				this.Label = Label;
				this.Page = Page;
			}
			
			public void SetLabelAllocation (Gdk.Rectangle r)
			{
				labelAlloc = r;
			}
		}
	}
}
