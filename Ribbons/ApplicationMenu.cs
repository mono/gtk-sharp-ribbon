using System;
using System.Collections.Generic;
using System.Threading;
using Gtk;
using Cairo;

namespace Ribbons
{
	/// <summary>
	/// The main menu of an application, displaying application-level commands, and documents list. 
	/// </summary>
	public class ApplicationMenu : Container
	{
		private static readonly TimeSpan openTimeoutSec = new TimeSpan (0, 0, 0, 0, 300);
		
		private const int verticalWindowOffset = topPadding - space;
		private const double lineWidth = 1.0;
		private const int topPadding = 24;
		private const int borderWidth = 6;
		private const int space = 2;
		
		protected Theme theme = new Theme ();
		
		private ApplicationButton appBtn;
		private List<ApplicationMenuItem> items;
		private Widget defaultMenu;
		private int itemHeight;
		private Gdk.Size menuSize;
		
		private Button optionsBtn, exitBtn;
		private Widget activeMenu;
		
		private Gdk.Rectangle itemsAlloc;
		private int menuItemsColWidth;
		private int buttonsHeight;
		private int exitBtnWidth, optionsBtnWidth;
		private int visibleMenuItems;
		private bool activeMenuVisible;
		private bool optionsBtnVisible, exitBtnVisible;
		
		private Window win;
		
		public ApplicationButton ApplicationButton
		{
			get { return appBtn; }
		}
		
		public Button OptionsButton
		{
			get { return optionsBtn; }
			set
			{
				if(optionsBtn == value) return;
				if(optionsBtn != null) optionsBtn.Unparent ();
				optionsBtn = value;
				if(value != null)
				{
					value.DrawBackground = true;
					value.OpaqueBackground = true;
					value.Parent = this;
					value.Visible = true;
				}
			}
		}
		
		public Button ExitButton
		{
			get { return exitBtn; }
			set
			{
				if(exitBtn == value) return;
				if(exitBtn != null) exitBtn.Unparent ();
				exitBtn = value;
				if(value != null)
				{
					value.DrawBackground = true;
					value.OpaqueBackground = true;
					value.Parent = this;
					value.Visible = true;
				}
			}
		}
		
		public int ItemHeigth
		{
			get { return itemHeight; }
			set
			{
				if(itemHeight == value) return;
				itemHeight = value;
				QueueResize ();
			}
		}
		
		public Gdk.Size MenuSize
		{
			get { return menuSize; }
			set
			{
				if(menuSize == value) return;
				menuSize = value;
				QueueResize ();
			}
		}
		
		public Widget DefaultMenu
		{
			get { return defaultMenu; }
			set
			{
				if(defaultMenu == value) return;
				bool updateActive = defaultMenu == activeMenu;
				if(updateActive && defaultMenu != null) defaultMenu.Unparent ();
				defaultMenu = value;
				if(updateActive) SetActiveMenu (value);
			}
		}
		
		/// <summary>Returns the number of children.</summary>
		public int NChildren
		{
			get { return items.Count; }
		}
		
		/// <summary>Default constructor.</summary>
		internal ApplicationMenu (ApplicationButton Button)
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			//this.children = new List<Widget> ();
			
			//Append (new Button ("OK"));
			
			appBtn = Button;
			items = new List<ApplicationMenuItem> ();
			itemHeight = 32;
			menuSize = new Gdk.Size (240, 320);
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
			if(w == null) w = defaultMenu;
			SetActiveMenu (w);
		}
		
		private void SetActiveMenu (Widget w)
		{
			if(activeMenu != null) activeMenu.Unparent ();
			activeMenu = w;
			w.Parent = this;
			w.Visible = true;
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
				win.GdkWindow.Move (x, y - verticalWindowOffset);
				
				win.ButtonPressEvent += delegate { KillMenu (true); };
				win.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			}
			else
			{
				win.ShowAll ();
				win.GdkWindow.Move (x, y - verticalWindowOffset);
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
			this.Hide ();
			//win.Destroy ();
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(Widget w in items)
			{
				if(w.Visible) callback (w);
			}
			
			if(optionsBtn != null && optionsBtn.Visible)
			{
				callback (optionsBtn);
			}
			
			if(exitBtn != null && exitBtn.Visible)
			{
				callback (exitBtn);
			}
			
			if(activeMenu != null && activeMenu.Visible)
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
			
			requisition.Height = menuItemsColHeight;
			requisition.Width = menuItemsColWidth;
			
			if(activeMenu != null)
			{
				/*Gtk.Requisition req = activeMenu.SizeRequest ();
				requisition.Width += req.Width;
				if(req.Height > requisition.Height) requisition.Height = req.Height;*/
				
				requisition.Width += menuSize.Width;
				if(menuSize.Height > requisition.Height) requisition.Height = menuSize.Height;
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
				buttonsWidth += req.Width;
				if(optionsBtn != null) buttonsWidth += space;
				if(req.Height > buttonsHeight) buttonsHeight = req.Height;
				exitBtnWidth = req.Width;
			}
			
			if(buttonsWidth > requisition.Width) requisition.Width = buttonsWidth;
			
			if(buttonsHeight > 0) requisition.Height += buttonsHeight + space;
			requisition.Width += borderWidth << 1;
			requisition.Height += borderWidth + topPadding;
			
			this.menuItemsColWidth = menuItemsColWidth;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			visibleMenuItems = 0;
			exitBtnVisible = optionsBtnVisible = false;
			
			allocation.Height -= borderWidth;
			
			if(buttonsHeight + topPadding <= allocation.Height)
			{
				Gdk.Rectangle alloc;
				
				if(buttonsHeight > 0)
				{
					alloc.X = allocation.Right - borderWidth;
					alloc.Y = allocation.Bottom - buttonsHeight;
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
							optionsBtn.SizeAllocate (alloc);
						}
					}
					
					allocation.Height -= buttonsHeight + space;
				}
				
				alloc.X = allocation.X + borderWidth;
				alloc.Y = allocation.Y + topPadding;
				itemsAlloc.X = alloc.X;
				itemsAlloc.Y = alloc.Y;
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
				
				itemsAlloc.Width = menuItemsColWidth + space;
				itemsAlloc.Height = allocation.Bottom - itemsAlloc.Y - borderWidth;
				
				if(activeMenu != null)
				{
					alloc.X = allocation.X + borderWidth + menuItemsColWidth + space;
					alloc.Width = allocation.Right - alloc.X - borderWidth;
					alloc.Y = allocation.Y + topPadding;
					alloc.Height = allocation.Bottom - alloc.Y - borderWidth;
					
					if(alloc.Width > 0 && alloc.Width > 0)
					{
						activeMenu.SizeAllocate (alloc);
					}
				}
			}
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			SetActiveMenu (defaultMenu);
			return base.OnButtonPressEvent (evnt);
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
			theme.DrawApplicationMenu (cr, rect, itemsAlloc, lineWidth, this);
		}
	}
}
