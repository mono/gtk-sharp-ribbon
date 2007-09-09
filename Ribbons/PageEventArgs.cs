using System;

namespace Ribbons
{
	public class PageEventArgs
	{
		private Ribbon.RibbonPage page;
		
		public Ribbon.RibbonPage Page
		{
			get { return this.page; }
		}
		
		public PageEventArgs (Ribbon.RibbonPage page)
		{
			this.page = page;
		}
	}
}
