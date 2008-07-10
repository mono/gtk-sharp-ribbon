using System;
using System.Collections.Generic;
using Gtk;
using Cairo;

namespace Ribbons
{
	public class ApplicationMenu : Container
	{
		private static int borderWidth = 2;
		private static int space = 2;
		
		protected Theme theme = new Theme ();
		
		private List<ApplicationMenuItem> items;
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
		
		private Window win;
		
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
		
		/// <summary>Returns the number of children.</summary>
		public int NChildren
		{
			get { return items.Count; }
		}
		
		/// <summary>Default constructor.</summary>
		public ApplicationMenu ()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			//this.children = new List<Widget> ();
			
			//Append (new Button ("OK"));
			
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
			i.Parent = this;
			i.Visible = true;
			
			if(ItemIndex == -1)
				items.Add (i);
			else
				items.Insert (ItemIndex, i);
		}
		
		public void Remove (int ItemIndex)
		{
			if(ItemIndex == -1) ItemIndex = items.Count - 1;
			
			items[ItemIndex].Unparent ();
			items.RemoveAt (ItemIndex);
		}
		
		public void ActivateMenu (Widget w)
		{
			if(w == null)
				activeMenu = defaultMenu;
			else
				activeMenu = w;
			
			QueueResize ();
		}
		
		public void ShowAt (int x, int y)
		{
			//if(win != null) KillMenu (true);
			
			//foreach(ApplicationMenuItem item in items) item.Parent = this;
			
			if(win == null)
			{
				win = new SyntheticWindow (WindowType.Popup);
				win.Child = this;
				
				win.Hidden += delegate { KillMenu (true); };
				
				win.ShowAll ();
				win.GdkWindow.Move (x, y);
				
				win.ButtonPressEvent += delegate { KillMenu (true); };
				win.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			}
			else
			{
				win.ShowAll ();
				win.GdkWindow.Move (x, y);
			}
			
			Grab.Add (win);
			Gdk.GrabStatus grabbed = Gdk.Pointer.Grab (win.GdkWindow, true, Gdk.EventMask.ButtonPressMask, null, null, 0);
			if(grabbed != Gdk.GrabStatus.Success)
			{
				KillMenu (false);
				return;
			}
			
			grabbed = Gdk.Keyboard.Grab (win.GdkWindow, true, 0);
			if(grabbed != Gdk.GrabStatus.Success)
			{
				KillMenu (false);
				return;
			}
		}
		
		private void KillMenu (bool Ungrab)
		{
			if(this.win == null) return;
			
			//Window win = this.win;
			//this.win = null;
			
			//win.Child = null;
			
			Grab.Remove (win);
			if(Ungrab)
			{
				Gdk.Pointer.Ungrab (0);
				Gdk.Keyboard.Ungrab (0);
			}
			win.Hide ();
			//win.Destroy ();
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(Widget w in items)
			{
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
				Gtk.Requisition req = optionsBtn.SizeRequest ();
				buttonsWidth = req.Width;
				buttonsHeight = req.Height;
				optionsBtnWidth = req.Width;
			}
			
			if(exitBtn != null)
			{
				Gtk.Requisition req = exitBtn.SizeRequest ();
				buttonsWidth = req.Width;
				if(optionsBtn != null) buttonsWidth += space;
				if(req.Height > buttonsHeight) buttonsHeight = req.Height;
				exitBtnWidth = req.Width;
			}
			
			if(buttonsHeight > 0) buttonsHeight += space;
			if(buttonsWidth > requisition.Width) requisition.Width = buttonsWidth;
			
			requisition.Width += borderWidth << 1;
			requisition.Height += borderWidth << 1;
			
			Console.WriteLine (requisition.Height + " " + requisition.Width);
			
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
	}
}
