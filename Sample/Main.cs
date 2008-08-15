using System;
using Gtk;

namespace Sample
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine ("Starting ...");
			Application.Init ();
			MainWindow win = new MainWindow ();
			//VariantsCombinaisonTest win = new VariantsCombinaisonTest ();
			win.Show ();
			Application.Run ();
		}
	}
}
