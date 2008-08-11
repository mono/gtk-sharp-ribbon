using System;
using Cairo;
using Gtk;
using Ribbons;

namespace Sample
{
	public class DropdownGroupTest : SyntheticWindow
	{
		protected bool composeAvailable = false;
		
		protected Label pageLabel1;
		
		public DropdownGroupTest() : base (WindowType.Toplevel)
		{
			AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		
			HBox master = new HBox ();
			master.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			Title = "Ribbons Sample";
			AppPaintable = true;
			
			Ribbons.Button button0 = new Ribbons.Button ("Hello World");
			
			RibbonGroup group0 = new RibbonGroup ();
			group0.Label = "Summer of Code";
			group0.Child = button0;
			group0.Expand += onClick;
			
			DropdownRibbonGroup dropGroup0 = new DropdownRibbonGroup ();
			dropGroup0.Group = group0;
			dropGroup0.Label = "Drop 1";
			
			DropdownRibbonGroup dropGroup1 = new DropdownRibbonGroup ();
			dropGroup1.Group = group0;
			dropGroup1.Label = "Drop 2";
			
			master.PackStart (dropGroup0, false, false, 0);
			master.PackStart (dropGroup1, false, false, 0);
			
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
