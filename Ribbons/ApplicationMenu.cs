using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class ApplicationMenu : SyntheticWindow
	{
		private static int borderWidth = 2;
		private static int space = 2;
		
		private List<MenuItem> items;
		private Widget defaultMenu;
		private int itemHeight;
		
		private Button optionsBtn, exitBtn;
		private Widget activeMenu;
		
		private int menuItemsColWidth;
		private int buttonsHeight;
		private int exitBtnWidth, optionsBtnWidth;
		private int visibleMenuItems;
		
		[GLib.Signal("options_clicked")]
		public event EventHandler OptionClicked;
		
		[GLib.Signal("exit_clicked")]
		public event EventHandler ExitClicked;
		
		public int ItemHeigth
		{
			get { return itemHeight; }
			set
			{
				throw new NotImplementedException();
			}
		}
		
		public Widget DefaultMenu
		{
			get { return defaultMenu; }
			set
			{
				throw new NotImplementedException();
			}
		}
		
		public ApplicationMenu (Gtk.WindowType type) : base (type)
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
					Gtk.Requisition req = mi.SizeRequest ();
					if(req.Width > menuItemsColWidth) menuItemsColWidth = req.Width;
					menuItemsColHeight += req.Height;
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
		
		
		public class MenuItem : Widget
		{
			private Image icon;
			private string label;
			private Widget menu;
			
			[GLib.Signal("action")]
			public event EventHandler Action;
			
			public Image Icon
			{
				get { return icon; }
				set
				{
					throw new NotImplementedException();
				}
			}
			
			public string Label
			{
				get { return label; }
				set
				{
					throw new NotImplementedException();
				}
			}
			
			public Widget Menu
			{
				get { return menu; }
				set
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
