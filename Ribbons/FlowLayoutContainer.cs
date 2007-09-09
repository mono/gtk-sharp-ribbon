using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	/// <summary>Container displaying children using a flow layout.</summary>
	public class FlowLayoutContainer : Container
	{
		private List<Widget> children;
		private Requisition[] childReqs;
		
		/// <summary>Returns the number of children.</summary>
		public int NChildren
		{
			get { return children.Count; }
		}
		
		/// <summary>Default constructor.</summary>
		public FlowLayoutContainer()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.children = new List<Widget> ();
		}
		
		/// <summary>Adds a widget before all existing widgetw.</summary>
		/// <param name="w">The widget to add.</param>
		public void Prepend (Widget w)
		{
			Insert (w, 0);
		}
		
		/// <summary>Adds a widget after all existing widgets.</summary>
		/// <param name="w">The widget to add.</param>
		public void Append (Widget w)
		{
			Insert (w, -1);
		}
		
		/// <summary>Inserts a widget at the specified location.</summary>
		/// <param name="w">The widget to add.</param>
		/// <param name="WidgetIndex">The index (starting at 0) at which the widget must be inserted, or -1 to insert the widget after all existing widgets.</param>
		public void Insert (Widget w, int WidgetIndex)
		{
			w.Parent = this;
			w.Visible = true;
			
			if(WidgetIndex == -1)
				children.Add (w);
			else
				children.Insert (WidgetIndex, w);
			
			ShowAll ();
		}
		
		/// <summary>Removes the widget at the specified index.</summary>
		/// <param name="WidgetIndex">Index of the widget to remove.</param>
		public void Remove (int WidgetIndex)
		{
			children[WidgetIndex].Parent = null;
			
			if(WidgetIndex == -1)
				children.RemoveAt (children.Count - 1);
			else
				children.RemoveAt (WidgetIndex);
			
			ShowAll ();
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			foreach(Widget w in children)
			{
				if(w.Visible) callback (w);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			int n = children.Count, nVisible = 0;
			childReqs = new Requisition[n];
			for(int i = 0 ; i < n ; ++i)
			{
				if(children[i].Visible)
				{
					childReqs[i] = children[i].SizeRequest ();
					++nVisible;
				}
			}
			
			if(WidthRequest != -1)
			{
				if(HeightRequest == -1)
				{
					int currentLineHeight = 0, currentLineWidth = 0;
					foreach(Widget w in children)
					{
						if(w.Visible)
						{
							Requisition childReq = w.SizeRequest ();
							currentLineHeight = Math.Max (childReq.Height, currentLineHeight);
							currentLineWidth += childReq.Width;
							if(currentLineWidth >= WidthRequest)
							{
								currentLineHeight = 0;
								currentLineWidth = 0;
							}
						}
					}
					requisition.Height = currentLineHeight;
				}
			}
			else // (WidthRequest == -1)
			{
				if(HeightRequest == -1)
				{
					foreach(Widget w in children)
					{
						if(w.Visible)
						{
							Requisition childReq = w.SizeRequest ();
							requisition.Height = Math.Max (childReq.Height, requisition.Height);
							requisition.Width += childReq.Width;
						}
					}
				}
				else
				{
#if !EXPERIMENTAL
					int totalWidth = 0, maxWidth = 0;
					for(int i = 0 ; i < n ; ++i)
					{
						if(children[i].Visible)
						{
							totalWidth += childReqs[i].Width;
							maxWidth = Math.Max (childReqs[i].Width, maxWidth);
						}
					}
					
					// TODO: the following algorithm a dichotomic-like search approach (lower bound: 1, upper bound: number of widgets)
					
					int lineCount = 0;
					int totalHeight = 0;
					
					do
					{
						++lineCount;
						int lineWidth = (int)Math.Ceiling ((double)totalWidth / lineCount);
						if(lineWidth < maxWidth) break;
						int currentLineWidth = 0, currentLineHeight = 0;
						for(int i = 0 ; i < n ; ++i)
						{
							if(children[i].Visible)
							{
								currentLineWidth += childReqs[i].Width;
								if(currentLineWidth > lineWidth)
								{
									totalHeight += currentLineHeight;
									currentLineWidth = childReqs[i].Width;
									currentLineHeight = 0;
								}
								currentLineHeight = Math.Max (childReqs[i].Height, currentLineHeight);
							}
						}
						totalHeight += currentLineHeight;
						
						if(totalHeight <= HeightRequest)
						{
							requisition.Width = lineWidth;
						}
					} while(totalHeight < HeightRequest && lineCount < nVisible);
#else
					int height = 0;
					for(int i = 0 ; i < n ; ++i)
					{
						height = Math.Max (childReqs[i].Height, height);
					}
					
					List<int> segments = new List<int>();
					segments.Add (0);
					segments.Add (n);
					
					for(;;)
					{
						int bestCandidateHeight = int.MaxValue;
						int bestSplit = -1;
						for(int i = 1 ; i < segments.Count ; ++i)
						{
							int oldSum, newSum;
							int splitPos = SplitWidgetsInTwo (childReqs, segments[i-1], segments[i] - 1, out oldSum, out newSum);
							int candidate = height - oldSum + newSum;
							if(newSum < oldSum && candidate < bestCandidateHeight)
							{
								bestSplit = splitPos;
								bestCandidateHeight = candidate;
							}
						}
						
						if(bestCandidateHeight < HeightRequest)
						{
							segments.Insert (~segments.BinarySearch (bestSplit), bestSplit);
						}
						else break;
					}
					
					int currentSegmentWidth = 0, currentSegmentNr = 1;
					for(int i = 0 ; i < n ; ++i)
					{
						if(i == segments[currentSegmentNr])
						{
							++currentSegmentNr;
							WidthRequest = Math.Max (currentSegmentNr, WidthRequest);
							currentSegmentNr = 0;
						}
						currentSegmentWidth += childReqs[i].Width;
					}
					requisition.Width = Math.Max (currentSegmentNr, WidthRequest);
#endif
				}
			}
		}
		
#if EXPERIMENTAL
		private int SplitWidgetsInTwo (Requisition[] Requisitions, int Index, int Length, out int PreviousSum, out int NewSum)
		{
			int[] maxLeft = new int[Length], maxRight = new int[Length];
			maxLeft[0] = Requisitions[Index].Width;
			maxRight[Length-1] = Requisitions[Index+Length-1].Width;
			PreviousSum = 0;
			for(int i = 1 ; i < Length ; ++i)
			{
				maxLeft[i] = Math.Max (maxLeft[i-1] , Requisitions[i+Index].Width);
				maxRight[Length-1-i] = Math.Max (maxLeft[Length-i] , Requisitions[Length-1-i+Index].Width);
				PreviousSum += Requisitions[i+Index].Height;
			}
			
			int ret = 0, smallestSum = int.MaxValue;
			for(int i = 0 ; i < Length ; ++i)
			{
				int sum = maxLeft[i] + maxRight[i];
				if(sum < smallestSum)
				{
					smallestSum = sum;
					ret = i + Index + 1;
				}
			}
			
			NewSum = smallestSum;
			return ret;
		}
#endif
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			int n = children.Count;
			Gdk.Rectangle childAlloc = allocation;
			int lineHeight = 0;
			for(int i = 0 ; i < n ; ++i)
			{
				if(children[i].Visible)
				{
					childAlloc.Width = childReqs[i].Width;
					childAlloc.Height = childReqs[i].Height;
					
					if(childAlloc.X != allocation.X && childAlloc.Right > allocation.Right)
					{
						childAlloc.X = allocation.X;
						childAlloc.Y += lineHeight;
						lineHeight = 0;
					}
					
					children[i].SizeAllocate (childAlloc);
					childAlloc.X += childAlloc.Width;
					lineHeight = Math.Max (childAlloc.Height, lineHeight);
				}
			}
		}
	}
}
