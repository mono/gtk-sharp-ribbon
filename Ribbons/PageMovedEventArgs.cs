using System;

namespace Ribbons
{
	public class PageMovedEventArgs
	{
		private Ribbon.RibbonPage page;
		
		public Ribbon.RibbonPage page
		{
			get { return this.page; }
		}
		
		public PageMovedEventArgs (Ribbon.RibbonPage page)
		{
			this.page = page;
		}
	}
}
