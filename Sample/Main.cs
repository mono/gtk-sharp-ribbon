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
			win.Show ();
			Application.Run ();
		}
	}
}
