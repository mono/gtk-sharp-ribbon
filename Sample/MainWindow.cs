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
			
			Ribbons.Button button0 = new Ribbons.Button ("Hello World");
			
			group0 = new RibbonGroup ();
			group0.Label = "Summer of Code";
			group0.Child = button0;
			group0.Expand += onClick;
			
			Menu openMenu = new Menu ();
			MenuItem abc_txt = new MenuItem ("abc.txt");
			openMenu.Append (abc_txt);
			MenuItem foo_txt = new MenuItem ("foo.txt");
			openMenu.Append (foo_txt);
			
			Ribbons.Button open = Ribbons.Button.FromStockIcon (Gtk.Stock.Open, "Open", false);
			open.DropDownMenu = openMenu;
			open.Clicked += onClick;
			
			Ribbons.Button button1 = new Ribbons.Button ("Menu Test");
			button1.Clicked += onClick;
			Menu button1_menu = new Menu ();
			MenuItem option1 = new MenuItem ("Option 1");
			button1_menu.Append (option1);
			button1.DropDownMenu = button1_menu;
			
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
			
			//Ribbons.FlowLayoutContainer flow0 = new FlowLayoutContainer ();
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
			
			group1 = new RibbonGroup ();
			group1.Label = "I will be back";
			group1.Child = btnFlowBox;
			
			Gallery gallery = new Gallery ();
			gallery.AppendTile (new SampleTile ("1"));
			gallery.AppendTile (new SampleTile ("2"));
			gallery.AppendTile (new SampleTile ("3"));
			gallery.AppendTile (new SampleTile ("4"));
			gallery.AppendTile (new SampleTile ("5"));
			
			group2 = new RibbonGroup ();
			group2.Label = "Gallery";
			group2.Child = gallery;
			
			HBox page0 = new HBox (false, 2);
			page0.PackStart (group0, false, false, 0);
			page0.PackStart (group1, false, false, 0);
			page0.PackStart (group2, false, false, 0);
			
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
			
			ribbon = new Ribbon ();
			ribbon.Shortcuts = shortcuts;
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
			
			TextView txt = new TextView ();
			
			master.PackStart (ribbon, false, false, 0);
			master.PackStart (txt, true, true, 0);
			
			Add (master);
			
			ScreenChanged += Window_OnScreenChanged;
			Window_OnScreenChanged (this, null);
			ExposeEvent += Window_OnExpose;
			DeleteEvent += Window_OnDelete;
			
			this.Resize (200, 200);
			this.ShowAll ();
		}

		private void onClick (object Sender, EventArgs e)
		{
			Dialog d = new Dialog ("Test", this, DialogFlags.DestroyWithParent);
			d.Modal = true;
			d.AddButton ("Close", ResponseType.Close);
			d.Run ();
			d.Destroy ();
		}
		
		[GLib.ConnectBefore]
		private void Window_OnExpose (object sender, ExposeEventArgs args)
		{
			Gdk.EventExpose evnt = args.Event;
			Context cr = Gdk.CairoHelper.Create (GdkWindow);
			
			if(composeAvailable)
				cr.SetSourceRGBA (0, 0, 0, 0.3);
			else
				cr.SetSourceRGB (0.3, 0.3, 0.3);
			
			cr.Operator = Operator.Source;
			cr.Paint ();
			
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
