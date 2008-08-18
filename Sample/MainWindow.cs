using System;
using Cairo;
using Gtk;
using Ribbons;

namespace Sample
{
	public class MainWindow : SyntheticWindow
	{
		protected bool composeAvailable = false;
		
		protected Ribbon ribbon;
		protected RibbonGroup group0, group1, group2;
		
		protected Label pageLabel1;
		
		public MainWindow() : base (WindowType.Toplevel)
		{
			AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		
			VBox master = new VBox ();
			master.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			Title = "Ribbons Sample";
			AppPaintable = true;
			
			VariantsCombinaisonSwitcher page0 = new VariantsCombinaisonSwitcher ();
			
			RibbonGroup group0 = CreateGroup0 ();
			RibbonGroup group1 = CreateGroup1 ();
			RibbonGroup group2 = CreateGroup2 ();
			DropdownRibbonGroup dgroup0 = CreateDropdownGroup0 ();
			dgroup0.Group = group0;
			DropdownRibbonGroup dgroup1 = CreateDropdownGroup1 ();
			dgroup1.Group = group1;
			DropdownRibbonGroup dgroup2 = CreateDropdownGroup2 ();
			dgroup2.Group = group2;
			
			VariantsCombinaison combi0 = new VariantsCombinaison ();
			combi0.Append (group0);
			combi0.Append (group1);
			combi0.Append (group2);
			page0.AddCombinaison (combi0);
			
			VariantsCombinaison combi1 = new VariantsCombinaison ();
			combi1.Append (group0);
			combi1.Append (group1);
			combi1.Append (dgroup2);
			page0.AddCombinaison (combi1);
			
			VariantsCombinaison combi2 = new VariantsCombinaison ();
			combi2.Append (dgroup0);
			combi2.Append (dgroup1);
			combi2.Append (dgroup2);
			page0.AddCombinaison (combi2);
			
			HBox page1 = new HBox (false, 2);
			RibbonGroup group10 = new RibbonGroup ();
			group10.Label = "Welcome on the second page";
			page1.PackStart (group10, false, false, 0);
			
			HBox page2 = new HBox (false, 2);
			
			Label pageLabel0 = new Label ("Page 1");
			pageLabel1 = new Label ("Page 2");
			Label pageLabel2 = new Label ("Page 3");
			
			Ribbons.Button shortcuts = new Ribbons.Button ("Menu");
			shortcuts.Child.ModifyFg (Gtk.StateType.Normal, new Gdk.Color(255, 255, 255));
			
			Menu mainMenu = new Menu ();
			MenuItem mainMenu_quit = new MenuItem ("Quit");
			mainMenu_quit.Activated += delegate (object Sender, EventArgs e)
			{
				Application.Quit ();
			};
			mainMenu.Append (mainMenu_quit);
			
			shortcuts.Clicked += delegate (object Sender, EventArgs e)
			{
				mainMenu.Popup ();
				mainMenu.ShowAll ();
			};
			
			QuickAccessToolbar qat = new QuickAccessToolbar ();
			Ribbons.Button qatNew, qatSave;
			qat.Append (qatNew = Ribbons.Button.FromStockIcon (Gtk.Stock.New, false));
			qat.Append (qatSave = Ribbons.Button.FromStockIcon (Gtk.Stock.Save, false));
			
			ribbon = new Ribbon ();
			ribbon.ApplicationButton = new ApplicationButton ();
			ribbon.QuickAccessToolbar = qat;
			//ribbon.Shortcuts = shortcuts;
			ribbon.AppendPage (page0, pageLabel0);
			ribbon.AppendPage (page1, pageLabel1);
			ribbon.AppendPage (page2, pageLabel2);
			pageLabel1.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			pageLabel1.ButtonPressEvent += delegate (object sender, ButtonPressEventArgs e)
			{
				Console.WriteLine("label1 press");
			};
			pageLabel1.EnterNotifyEvent += delegate (object sender, EnterNotifyEventArgs e)
			{
				Console.WriteLine("label1 enter");
			};
			pageLabel1.LeaveNotifyEvent += delegate (object sender, LeaveNotifyEventArgs e)
			{
				Console.WriteLine("label1 leave");
			};
			
			ApplicationMenu appMenu = ribbon.ApplicationButton.Menu;
			TextView mnu = new TextView ();
			appMenu.DefaultMenu = mnu;
			mnu.Buffer.InsertAtCursor ("Default");
			ApplicationMenuItem mi = new ApplicationMenuItem ("Test 1");
			mnu = new TextView ();
			mi.Menu = mnu;
			mnu.Buffer.InsertAtCursor ("Test 1");
			appMenu.Append (mi);
			mi = new ApplicationMenuItem ("Test 2");
			appMenu.Append (mi);
			mi = new ApplicationMenuItem ("Test 3");
			appMenu.Append (mi);
			
			appMenu.OptionsButton = new Ribbons.Button ("Options");
			appMenu.ExitButton = new Ribbons.Button ("Exit");
			
			TextView txt = new TextView ();
			
			master.PackStart (ribbon, false, false, 0);
			master.PackStart (txt, true, true, 0);
			
			Add (master);

			ribbon.ApplicationButton.KeyTip = new KeyTip (appMenu, "A");
			qat.AddKeyTip (new KeyTip (qatNew, "B"));
			qat.AddKeyTip (new KeyTip (qatSave, "C"));
			ribbon.AddTabKeyTip (new KeyTip (pageLabel0, "D"));
			ribbon.AddTabKeyTip (new KeyTip (pageLabel1, "E"));
			ribbon.AddTabKeyTip (new KeyTip (pageLabel2, "F"));
			
			ScreenChanged += Window_OnScreenChanged;
			Window_OnScreenChanged (this, null);
			ExposeEvent += Window_OnExpose;
			DeleteEvent += Window_OnDelete;
			
			this.Resize (200, 200);
			this.ShowAll ();
		}

		private RibbonGroup CreateGroup0 ()
		{
			Ribbons.Button button0 = new Ribbons.Button ("Hello World");
			
			RibbonGroup group0 = new RibbonGroup ();
			group0.Label = "Summer of Code";
			group0.Child = button0;
			group0.Expand += onClick;
			
			return group0;
		}
		
		private DropdownRibbonGroup CreateDropdownGroup0 ()
		{
			DropdownRibbonGroup ret = new DropdownRibbonGroup ();
			ret.Label = "Summer of Code";
			return ret;
		}
		
		private RibbonGroup CreateGroup1 ()
		{
			Ribbons.Button button1 = new Ribbons.Button ("Menu Test");
			button1.Clicked += onClick;
			Menu button1_menu = new Menu ();
			MenuItem option1 = new MenuItem ("Option 1");
			button1_menu.Append (option1);
			button1.DropDownMenu = button1_menu;
			
			Menu openMenu = new Menu ();
			MenuItem abc_txt = new MenuItem ("abc.txt");
			openMenu.Append (abc_txt);
			MenuItem foo_txt = new MenuItem ("foo.txt");
			openMenu.Append (foo_txt);
			
			Ribbons.Button open = Ribbons.Button.FromStockIcon (Gtk.Stock.Open, "Open", false);
			open.DropDownMenu = openMenu;
			open.Clicked += onClick;
			
			Ribbons.ToolPack fileToolPack = new Ribbons.ToolPack ();
			fileToolPack.AppendButton (Ribbons.Button.FromStockIcon (Gtk.Stock.New, "New", false));
			fileToolPack.AppendButton (open);
			fileToolPack.AppendButton (Ribbons.Button.FromStockIcon (Gtk.Stock.Save, "Save", false));
			
			Ribbons.ToolPack printerToolPack = new Ribbons.ToolPack ();
			printerToolPack.AppendButton (Ribbons.Button.FromStockIcon (Gtk.Stock.Print, "Print", false));
			
			Ribbons.ToolPack fontToolPack = new Ribbons.ToolPack ();
			fontToolPack.AppendButton (Ribbons.ToggleButton.FromStockIcon (Gtk.Stock.Bold, false));
			fontToolPack.AppendButton (Ribbons.ToggleButton.FromStockIcon (Gtk.Stock.Italic, false));
			fontToolPack.AppendButton (Ribbons.ToggleButton.FromStockIcon (Gtk.Stock.Underline, false));
			
			ComboBox font = new ComboBox (new string[] { "Arial", "Verdana" });
			font.Active = 0;
			
			Ribbons.ToolBox flow0 = new ToolBox ();
			flow0.Append (fileToolPack);
			flow0.Append (printerToolPack);
			flow0.Append (fontToolPack);
			flow0.Append (font);
			
			HBox btnFlowBox = new HBox (false, 2);
			btnFlowBox.Add (button1);
			btnFlowBox.Add (flow0);
			
			// Little hack because Gtk+ is not designed to support size negociations
			btnFlowBox.SizeAllocated += delegate(object Sender, SizeAllocatedArgs e)
			{
				flow0.HeightRequest = e.Allocation.Height;
			};
			
			RibbonGroup group1 = new RibbonGroup ();
			group1.Label = "I will be back";
			group1.Child = btnFlowBox;
			
			return group1;
		}
		
		private DropdownRibbonGroup CreateDropdownGroup1 ()
		{
			DropdownRibbonGroup ret = new DropdownRibbonGroup ();
			ret.Label = "I will be back";
			return ret;
		}
		
		private RibbonGroup CreateGroup2 ()
		{
			Gallery gallery = new Gallery ();
			gallery.AppendTile (new SampleTile ("1"));
			gallery.AppendTile (new SampleTile ("2"));
			gallery.AppendTile (new SampleTile ("3"));
			gallery.AppendTile (new SampleTile ("4"));
			gallery.AppendTile (new SampleTile ("5"));
			
			RibbonGroup group2 = new RibbonGroup ();
			group2.Label = "Gallery";
			group2.Child = gallery;
			
			return group2;
		}
		
		private DropdownRibbonGroup CreateDropdownGroup2 ()
		{
			DropdownRibbonGroup ret = new DropdownRibbonGroup ();
			ret.Label = "Gallery";
			return ret;
		}

		private void onClick (object Sender, EventArgs e)
		{
			Dialog d = new Dialog ("Test", this, DialogFlags.DestroyWithParent);
			d.Modal = true;
			d.AddButton ("Close", ResponseType.Close);
			d.Run ();
			d.Destroy ();
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			if(evnt.Key.ToString() == "Alt_L" || evnt.Key.ToString() == "ISO_Level3_Shift")
			{
				ribbon.ShowTopLevelKeyTips ();
			}
			
			return base.OnKeyPressEvent (evnt);
		}

		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			if(evnt.Key.ToString() == "Alt_L" || evnt.Key.ToString() == "ISO_Level3_Shift")
			{
				ribbon.HideKeyTips ();
			}
			
			return base.OnKeyReleaseEvent (evnt);
		}
		
		[GLib.ConnectBefore]
		private void Window_OnExpose (object sender, ExposeEventArgs args)
		{
			Gdk.EventExpose evnt = args.Event;
			Context cr = Gdk.CairoHelper.Create (GdkWindow);
			
			/*if(composeAvailable)
				cr.SetSourceRGBA (0, 0, 0, 0.3);
			else*/
				cr.SetSourceRGB (0.3, 0.3, 0.3);
			
			//cr.SetSourceRGB (0.749, 0.859, 1.0);
			
			cr.Operator = Operator.Source;
			cr.Paint ();
			
			((IDisposable)cr.Target).Dispose();
			((IDisposable)cr).Dispose();			
			
			args.RetVal = false;
		}
		
		private void Window_OnScreenChanged (object Send, ScreenChangedArgs args)
		{
			Gdk.Colormap cm = Screen.RgbaColormap;
			composeAvailable = cm != null;	// FIX: Do not seem to detect compose support in all cases 
			
			if(!composeAvailable) cm = Screen.RgbColormap;
			Colormap = cm;
			
			Console.WriteLine ("Compose Support: " + composeAvailable);
		}
		
		private void Window_OnDelete (object send, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}
	}
}
