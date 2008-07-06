using System;
using System.Collections.Generic;
using Gtk;
using Cairo;

namespace Ribbons
{
	public class ApplicationMenu : SyntheticWindow
	{
		private static int borderWidth = 2;
		private static int space = 2;
		
		protected Theme theme = new Theme ();
		
		private List<MenuItem> items;
		private Widget defaultMenu;
		private int itemHeight;
		
		private Button optionsBtn, exitBtn;
		private Widget activeMenu;
		
		private int menuItemsColWidth;
		private int buttonsHeight;
		private int exitBtnWidth, optionsBtnWidth;
		private int visibleMenuItems;
		private bool activeMenuVisible;
		private bool optionsBtnVisible, exitBtnVisible;
		
		public Button OptionsButton
		{
			get { return optionsBtn; }
			set
			{
				if(optionsBtn == value) return;
				optionsBtn = value;
				if(value == null) optionsBtnVisible = false;
				ShowAll ();
			}
		}
		
		public Button ExitButton
		{
			get { return exitBtn; }
			set
			{
				if(exitBtn == value) return;
				exitBtn = value;
				if(value == null) exitBtnVisible = false;
				ShowAll ();
			}
		}
		
		public int ItemHeigth
		{
			get { return itemHeight; }
			set
			{
				if(itemHeight == value) return;
				itemHeight = value;
				ShowAll ();
			}
		}
		
		public Widget DefaultMenu
		{
			get { return defaultMenu; }
			set
			{
				Widget prevDefaultMenu = defaultMenu;
				if(defaultMenu == value) return;
				defaultMenu = value;
				if(activeMenu == prevDefaultMenu) ShowAll ();
			}
		}
		
		public ApplicationMenu () : base (Gtk.WindowType.Popup)
		{
			items = new List<MenuItem> ();
		}
		
		public void Prepend (MenuItem i)
		{
			Insert (i, 0);
		}
		
		public void Append (MenuItem i)
		{
			Insert (i, -1);
		}
		
		public void Insert (MenuItem i, int ItemIndex)
		{
			i.Parent = this;
			i.Visible = true;
			
			if(ItemIndex == -1)
				items.Add (i);
			else
				items.Insert (ItemIndex, i);
			
			ShowAll ();
		}
		
		public void Remove (int ItemIndex)
		{
			items[ItemIndex].Parent = null;
			
			if(ItemIndex == -1)
				items.RemoveAt (items.Count - 1);
			else
				items.RemoveAt (ItemIndex);
			
			ShowAll ();
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			for(int i = 0, i_ub = visibleMenuItems ; i < i_ub ; ++i)
			{
				Widget w = items[i];
				if(w.Visible) callback (w);
			}
			
			if(optionsBtn != null && optionsBtnVisible)
			{
				callback (optionsBtn);
			}
			
			if(exitBtn != null && exitBtnVisible)
			{
				callback (exitBtn);
			}
			
			if(activeMenu != null && activeMenuVisible)
			{
				callback (activeMenu);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			int menuItemsColWidth = 0;
			int menuItemsColHeight = 0;
			
			foreach(MenuItem mi in items)
			{
				if(mi.Visible)
				{
					mi.HeightRequest = itemHeight;
					Gtk.Requisition req = mi.SizeRequest ();
					if(req.Width > menuItemsColWidth) menuItemsColWidth = req.Width;
					menuItemsColHeight += itemHeight;
				}
			}
			
			requisition.Height = menuItemsColHeight;
			requisition.Width = menuItemsColWidth;
			
			if(activeMenu != null)
			{
				Gtk.Requisition req = activeMenu.SizeRequest ();
				requisition.Width += req.Width;
				if(req.Height > requisition.Height) requisition.Height = req.Height;
			}
			
			int buttonsWidth = 0;
			buttonsHeight = 0;
			
			if(optionsBtn != null)
			{
				Gtk.Requisition req = activeMenu.SizeRequest ();
				buttonsWidth = req.Width;
				buttonsHeight = req.Height;
				optionsBtnWidth = req.Width;
			}
			
			if(exitBtn != null)
			{
				Gtk.Requisition req = activeMenu.SizeRequest ();
				buttonsWidth = req.Width;
				if(optionsBtn != null) buttonsWidth += space;
				if(req.Height > buttonsHeight) buttonsHeight = req.Height;
				exitBtnWidth = req.Width;
			}
			
			buttonsHeight += space;
			if(buttonsWidth > requisition.Width) requisition.Width = buttonsWidth;
			
			requisition.Width += borderWidth << 1;
			requisition.Height += borderWidth << 1;
			
			this.menuItemsColWidth = menuItemsColWidth;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			visibleMenuItems = 0;
			
			allocation.Height -= borderWidth;
			
			if(buttonsHeight + borderWidth <= allocation.Height)
			{
				Gdk.Rectangle alloc;
				
				if(buttonsHeight > 0)
				{
					alloc.X = allocation.Right - borderWidth;
					alloc.Y = allocation.Bottom - borderWidth - buttonsHeight;
					alloc.Height = buttonsHeight;
					
					if(exitBtn != null)
					{
						alloc.X -= exitBtnWidth;
						alloc.Width = exitBtnWidth;
						if(alloc.X >= allocation.X + borderWidth)
						{
							exitBtn.SizeAllocate (alloc);
						}
					}
					
					if(optionsBtn != null)
					{
						if(exitBtn != null) alloc.X -= space;
						alloc.X -= optionsBtnWidth;
						alloc.Width = optionsBtnWidth;
						if(alloc.X >= allocation.X + borderWidth)
						{
							exitBtn.SizeAllocate (alloc);
						}
					}
					
					allocation.Height -= buttonsHeight + space;
				}
				
				alloc.X = allocation.X + borderWidth;
				alloc.Y = allocation.Y + borderWidth;
				alloc.Height = itemHeight;
				if(allocation.Right - alloc.X - borderWidth < menuItemsColWidth)
				{
					menuItemsColWidth = allocation.Right - alloc.X - borderWidth;
				}

				if(menuItemsColWidth > 0)
				{
					alloc.Width = menuItemsColWidth;
					
					foreach(MenuItem mi in items)
					{
						if(mi.Visible)
						{
							if(alloc.Bottom <= allocation.Bottom)
							{
								mi.SizeAllocate (alloc);
								alloc.Y += itemHeight;
								++visibleMenuItems;
							}
						}
					}
				}
				
				if(activeMenu != null)
				{
					alloc.X = allocation.X + borderWidth + menuItemsColWidth + space;
					alloc.Width = allocation.Right - alloc.X - borderWidth;
					alloc.Y = allocation.Y + borderWidth;
					alloc.Height = allocation.Bottom - alloc.Y - borderWidth;
					
					if(alloc.Width > 0 && alloc.Width > 0)
					{
						activeMenu.SizeAllocate (alloc);
					}
				}
			}
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
			theme.DrawApplicationMenu (cr, rect, this);
		}
		
		
		public class MenuItem : Bin
		{
			private Widget img;
			private Label lbl;
			private Widget menu;
			
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
			
			/// <summary>Constructor given a label to display.</summary>
			/// <param name="Label">Label to display.</param>
			public MenuItem (string Label) : this ()
			{
				this.Label = Label;
			}
			
			/// <summary>Constructor given an image to display.</summary>
			/// <param name="Image">Image to display</param>
			public MenuItem (Image Image) : this ()
			{
				this.Image = Image;
			}
			
			/// <summary>Constructor given a label and an image to display.</summary>
			/// <param name="Image">Image to display.</param>
			/// <param name="Label">Label to display.</param>
			public MenuItem (Image Image, string Label) : this ()
			{
				this.Image = Image;
				this.Label = Label;
			}
			
			/// <summary>Constructs a Button from a stock.</summary>
			/// <param name="Name">Name of the stock.</param>
			/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
			public static MenuItem FromStockIcon (string Name, bool Large)
			{
				Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
				return btn = new MenuItem (img);
			}
			
			/// <summary>Constructs a Button from a stock.</summary>
			/// <param name="Name">Name of the stock.</param>
			/// <param name="Label">Label to display.</param>
			/// <param name="Large"><b>true</b> if the image should be large, <b>false</b> otherwise.</param>
			public static MenuItem FromStockIcon (string Name, string Label, bool Large)
			{
				Image img = new Image (Name, Large ? IconSize.LargeToolbar : IconSize.SmallToolbar);
				return new MenuItem (img, Label);
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
			
			protected override void OnSizeRequested (ref Requisition requisition)
			{
				base.OnSizeRequested (ref requisition);
			}
			
			protected override void OnSizeAllocated (Gdk.Rectangle allocation)
			{
				base.OnSizeAllocated (allocation);
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
				theme.DrawApplicationMenuItem (cr, rect, this);
			}
		}
	}
}
