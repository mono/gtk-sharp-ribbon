using System;
using Cairo;
using Gtk;
using Ribbons;

namespace Sample
{
	public class VariantsCombinaisonTest : SyntheticWindow
	{
		protected bool composeAvailable = false;
		
		protected Label pageLabel1;
		
		public VariantsCombinaisonTest() : base (WindowType.Toplevel)
		{
			AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			Title = "Ribbons Sample";
			AppPaintable = true;
			
			VariantsCombinaisonSwitcher switcher = new VariantsCombinaisonSwitcher ();
			
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
			switcher.AddCombinaison (combi0);
			
			VariantsCombinaison combi1 = new VariantsCombinaison ();
			combi1.Append (group0);
			combi1.Append (group1);
			combi1.Append (dgroup2);
			switcher.AddCombinaison (combi1);
			
			VariantsCombinaison combi2 = new VariantsCombinaison ();
			combi2.Append (dgroup0);
			combi2.Append (dgroup1);
			combi2.Append (dgroup2);
			switcher.AddCombinaison (combi2);
			
			Add (switcher);

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
		
		[GLib.ConnectBefore]
		private void Window_OnExpose (object sender, ExposeEventArgs args)
		{
			Gdk.EventExpose evnt = args.Event;
			Context cr = Gdk.CairoHelper.Create (GdkWindow);
			
			/*if(composeAvailable)
				cr.SetSourceRGBA (0, 0, 0, 0.3);
			else*/
				cr.SetSourceRGB (0.6, 0.6, 0.6);
			
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
