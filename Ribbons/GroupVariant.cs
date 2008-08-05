using System;
using Gtk;

namespace Ribbons
{
	public class GroupVariant
	{
		private Widget child;
		private int width;
		
		public Widget Child
		{
			get { return child; }
		}
		
		public int Width
		{
			get { return width; }
		}
		
		public GroupVariant (Widget Child)
		{
			this.child = Child;
			this.width = -1;
		}
		
		public void Measure (int Height)
		{
			child.HeightRequest = Height;
			Requisition req = child.SizeRequest ();
			width = req.Width;
		}
	}
}
