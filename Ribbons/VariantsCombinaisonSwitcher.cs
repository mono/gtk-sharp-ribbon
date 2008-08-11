using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	public class VariantsCombinaisonSwitcher : Container
	{
		private List<VariantsCombinaison> combinaisons;
		private int requestedWidth, requiredHeight;
		private VariantsCombinaison candidateCombinaison;
		private VariantsCombinaison selectedCombinaison;
		
		public int Count
		{
			get { return combinaisons.Count; }
		}
		
		public VariantsCombinaison this[int Index]
		{
			get { return combinaisons[Index]; }
		}
		
		public VariantsCombinaisonSwitcher ()
		{
			Init ();
		}
		
		protected VariantsCombinaisonSwitcher (IntPtr raw) : base (raw)
		{
			Init ();
		}
		
		private void Init ()
		{
			combinaisons = new List<VariantsCombinaison> ();
			
			SetFlag (WidgetFlags.NoWindow);
			
			AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
		}
		
		public void AddCombinaison (VariantsCombinaison Combinaison)
		{
			combinaisons.Add (Combinaison);
		}
		
		public void RemoveCombinaison (VariantsCombinaison Combinaison)
		{
			combinaisons.Remove (Combinaison);
		}
		
		public void RemoveCombinaisonAt (int Index)
		{
			combinaisons.RemoveAt (Index);
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			if(selectedCombinaison != null) callback (selectedCombinaison);
		}
		int counter = 0;
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			Console.WriteLine ("TEST");
			base.OnSizeRequested (ref requisition);
			
			/*combinaisons[0].HeightRequest = HeightRequest;
			requisition = combinaisons[0].SizeRequest ();
			requiredHeight = requisition.Height;
			requestedWidth = requisition.Width;
			candidateCombinaison = combinaisons[0];
			return;*/
			
			candidateCombinaison = null;
			requiredHeight = HeightRequest;
			
			if(WidthRequest == -1)
			{
				int width = int.MaxValue;
				
				foreach(VariantsCombinaison combi in combinaisons)
				{
					//combi.HeightRequest = HeightRequest;
					
					Requisition req = combi.SizeRequest ();
					
					if(req.Width < width)
					{
						candidateCombinaison = combi;
						width = req.Width;
						requiredHeight = req.Height;
					}
				}
				
				requestedWidth = requisition.Width = width;
			}
			else if(HeightRequest == -1)
			{
				int width = -1;
				
				foreach(VariantsCombinaison combi in combinaisons)
				{
					combi.HeightRequest = -1;
					
					Requisition req = combi.SizeRequest ();
					
					if((req.Width <= WidthRequest && req.Width > width) || (width > WidthRequest && req.Width < width))
					{
						candidateCombinaison = combi;
						width = req.Width;
						requiredHeight = req.Height;
					}
				}
				
				requestedWidth = requisition.Width = WidthRequest;
			}
			
			if(HeightRequest == -1) requisition.Height = requiredHeight;
			else requisition.Height = HeightRequest;
			
			Console.WriteLine ("Switcher req: " + requisition.Width + " " + requisition.Height);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{Console.WriteLine (allocation.Width + " " + allocation.Height);
			base.OnSizeAllocated (allocation);
			
			if(requestedWidth != allocation.Width || requiredHeight != allocation.Height)
			{
				int width = -1;
				candidateCombinaison = null;
				
				foreach(VariantsCombinaison combi in combinaisons)
				{
					//combi.HeightRequest = allocation.Height;
					
					Requisition req = combi.SizeRequest ();
					
					if(req.Width <= allocation.Width && req.Width > width)
					{
						candidateCombinaison = combi;
						width = req.Width;
					}
				}
			}
			
			Select (candidateCombinaison);
			
			if(candidateCombinaison != null)
			{
				candidateCombinaison.SizeAllocate (allocation);
			}
		}
		
		private void Select (VariantsCombinaison combi)
		{Console.WriteLine ("Select: " + combi);
			if(selectedCombinaison != combi)
			{
				if(selectedCombinaison != null)
				{
					selectedCombinaison.Unparent ();
				}
				
				if(combi != null)
				{
					combi.Parent = this;
				}
				
				selectedCombinaison = combi;
			}
		}
	}
}
