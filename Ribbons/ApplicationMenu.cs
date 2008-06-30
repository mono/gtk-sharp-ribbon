using System;
using Gtk;

namespace Ribbons
{
	public class ApplicationMenu : Window
	{
		public void Prepend (MenuItem i)
		{
			
		}
		
		public void Append (MenuItem i)
		{
			
		}
		
		public void Insert (MenuItem i, int ItemIndex)
		{
			
		}
		
		public void Remove (int ItemIndex)
		{
			
		}
		
		public ApplicationMenu (Gtk.WindowType type) : base (type)
		{
			
		}
		
		public class MenuItem
		{
			private Image mIcon;
			private string mLabel;
			private Container mContainer;
			
			[GLib.Signal("action")]
			public event EventHandler Action;
			
			public Image Icon
			{
				get { return mIcon; }
				set
				{
					throw new NotImplementedException();
				}
			}
			
			public string Label
			{
				get { return mLabel; }
				set
				{
					throw new NotImplementedException();
				}
			}
			
			public Container Container
			{
				get { return mContainer; }
				set
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
