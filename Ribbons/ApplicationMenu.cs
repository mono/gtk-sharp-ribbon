using System;
using System.Collections.Generic;
using Gtk;
using Cairo;

namespace Ribbons
{
	public class ApplicationMenu
	{
		protected Theme theme = new Theme ();
		
		protected List<ApplicationMenuItem> items;
		private Widget defaultMenu;
		private int itemHeight;
		
		private Button optionsBtn, exitBtn;
		private Widget activeMenu;
		
		public Button OptionsButton
		{
			get { return optionsBtn; }
			set
			{
				if(optionsBtn == value) return;
				optionsBtn = value;
			}
		}
		
		public Button ExitButton
		{
			get { return exitBtn; }
			set
			{
				if(exitBtn == value) return;
				exitBtn = value;
			}
		}
		
		public int ItemHeigth
		{
			get { return itemHeight; }
			set
			{
				if(itemHeight == value) return;
				itemHeight = value;
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
			}
		}
		
		public ApplicationMenu ()
		{
			items = new List<ApplicationMenuItem> ();
			itemHeight = 32;
		}
		
		public void Prepend (ApplicationMenuItem i)
		{
			Insert (i, 0);
		}
		
		public void Append (ApplicationMenuItem i)
		{
			Insert (i, -1);
		}
		
		public void Insert (ApplicationMenuItem i, int ItemIndex)
		{
			i.Visible = true;
			
			if(ItemIndex == -1)
				items.Add (i);
			else
				items.Insert (ItemIndex, i);
		}
		
		public void Remove (int ItemIndex)
		{
			if(ItemIndex == -1)
				items.RemoveAt (items.Count - 1);
			else
				items.RemoveAt (ItemIndex);
		}
		
		public void ShowAt (int x, int y)
		{
			Window win = new Window (this);
			win.GdkWindow.Move (x, y);
			win.Show ();
		}
		
		private class Window : SyntheticWindow
		{
			private static int borderWidth = 2;
			private static int space = 2;
			
			private ApplicationMenu parent;
			
			private int menuItemsColWidth;
			private int buttonsHeight;
			private int exitBtnWidth, optionsBtnWidth;
			private int visibleMenuItems;
			private bool activeMenuVisible;
			private bool optionsBtnVisible, exitBtnVisible;
			
			public Window (ApplicationMenu Parent) : base (Gtk.WindowType.Popup)
			{
				parent = Parent;
				
				foreach(ApplicationMenuItem mi in parent.items)
				{
					mi.Parent = this;
				}
			}
			
			protected override void ForAll (bool include_internals, Callback callback)
			{
				List<ApplicationMenuItem> items = parent.items;
				
				for(int i = 0, i_ub = visibleMenuItems ; i < i_ub ; ++i)
				{
					Widget w = items[i];
					if(w.Visible) callback (w);
				}
				
				if(parent.optionsBtn != null && optionsBtnVisible)
				{
					callback (parent.optionsBtn);
				}
				
				if(parent.exitBtn != null && exitBtnVisible)
				{
					callback (parent.exitBtn);
				}
				
				if(parent.activeMenu != null && activeMenuVisible)
				{
					callback (parent.activeMenu);
				}
			}
			
			protected override void OnSizeRequested (ref Requisition requisition)
			{
				base.OnSizeRequested (ref requisition);
				
				List<ApplicationMenuItem> items = parent.items;
				int itemHeight = parent.itemHeight;
				
				menuItemsColWidth = 0;
				int menuItemsColHeight = 0;
				
				foreach(ApplicationMenuItem mi in items)
				{
					if(mi.Visible)
					{
						mi.HeightRequest = itemHeight;
						Gtk.Requisition req = mi.SizeRequest ();
						if(req.Width > menuItemsColWidth) menuItemsColWidth = req.Width;
						menuItemsColHeight += itemHeight;
					}
				}
				
				Console.WriteLine ("> " + menuItemsColHeight +" " + menuItemsColWidth);
				
				requisition.Height = menuItemsColHeight;
				requisition.Width = menuItemsColWidth;
				
				if(parent.activeMenu != null)
				{
					Gtk.Requisition req = parent.activeMenu.SizeRequest ();
					requisition.Width += req.Width;
					if(req.Height > requisition.Height) requisition.Height = req.Height;
				}
				
				int buttonsWidth = 0;
				buttonsHeight = 0;
				
				if(parent.optionsBtn != null)
				{
					Gtk.Requisition req = parent.optionsBtn.SizeRequest ();
					buttonsWidth = req.Width;
					buttonsHeight = req.Height;
					optionsBtnWidth = req.Width;
				}
				
				if(parent.exitBtn != null)
				{
					Gtk.Requisition req = parent.exitBtn.SizeRequest ();
					buttonsWidth = req.Width;
					if(parent.optionsBtn != null) buttonsWidth += space;
					if(req.Height > buttonsHeight) buttonsHeight = req.Height;
					exitBtnWidth = req.Width;
				}
				
				buttonsHeight += space;
				if(buttonsWidth > requisition.Width) requisition.Width = buttonsWidth;
				
				requisition.Width += borderWidth << 1;
				requisition.Height += borderWidth << 1;
				
				Console.WriteLine (requisition.Height + " " + requisition.Width);
				
				this.menuItemsColWidth = menuItemsColWidth;
			}
			
			protected override void OnSizeAllocated (Gdk.Rectangle allocation)
			{
				base.OnSizeAllocated (allocation);
				
				List<ApplicationMenuItem> items = parent.items;
				int itemHeight = parent.itemHeight;
				
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
						
						if(parent.exitBtn != null)
						{
							alloc.X -= exitBtnWidth;
							alloc.Width = exitBtnWidth;
							if(alloc.X >= allocation.X + borderWidth)
							{
								parent.exitBtn.SizeAllocate (alloc);
							}
						}
						
						if(parent.optionsBtn != null)
						{
							if(parent.exitBtn != null) alloc.X -= space;
							alloc.X -= optionsBtnWidth;
							alloc.Width = optionsBtnWidth;
							if(alloc.X >= allocation.X + borderWidth)
							{
								parent.exitBtn.SizeAllocate (alloc);
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
						
						foreach(ApplicationMenuItem mi in items)
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
					
					if(parent.activeMenu != null)
					{
						alloc.X = allocation.X + borderWidth + menuItemsColWidth + space;
						alloc.Width = allocation.Right - alloc.X - borderWidth;
						alloc.Y = allocation.Y + borderWidth;
						alloc.Height = allocation.Bottom - alloc.Y - borderWidth;
						
						if(alloc.Width > 0 && alloc.Width > 0)
						{
							parent.activeMenu.SizeAllocate (alloc);
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
				parent.theme.DrawApplicationMenu (cr, rect, parent, this);
			}
		}
	}
}
