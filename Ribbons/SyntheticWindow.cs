using System;
using System.Collections.Generic;
using Gdk;
using Gtk;

namespace Ribbons
{
	/// <summary>Window generating synthetic events to window-less widgets.</summary>
	public class SyntheticWindow : Gtk.Window
	{
		private List<Widget> lastHoveredWidgets;
		
		public SyntheticWindow (Gtk.WindowType type) : base (type)
		{
			lastHoveredWidgets = new List<Gtk.Widget> ();
		}
		
		protected override bool OnWidgetEvent (Gdk.Event evnt)
		{
			// This method is hooked to block the event as soon as possible if required
			
			if(evnt.Window.Equals (this.GdkWindow))
			{
				switch(evnt.Type)
				{
					case Gdk.EventType.ButtonPress:
					case Gdk.EventType.ButtonRelease:
					case Gdk.EventType.ThreeButtonPress:
					case Gdk.EventType.TwoButtonPress:
						Gdk.EventButton eb = new Gdk.EventButton (evnt.Handle);
						return PropagateEventGivenCoordinate (evnt, eb.X, eb.XRoot, eb.Y, eb.YRoot);
					
					case Gdk.EventType.MotionNotify:
						Gdk.EventMotion em = new Gdk.EventMotion (evnt.Handle);
						return PropagateEventGivenCoordinate (evnt, em.X, em.XRoot, em.Y, em.YRoot);
					
					case Gdk.EventType.LeaveNotify:
						foreach(Widget w in lastHoveredWidgets)
						{
							w.ProcessEvent (evnt);
						}
						lastHoveredWidgets.Clear();
						return base.OnWidgetEvent (evnt);
				}
			}
			return base.OnWidgetEvent (evnt);
		}
		
		private bool PropagateEventGivenCoordinate (Gdk.Event evnt, double X, double XRoot, double Y, double YRoot)
		{
			int x = (int)X, y = (int)Y;
			Container current = this;	// Current container containing the coordinate
			Widget match = this;	// Current match for the position
			int matchedPos = 0;	// Current position in lastHoveredWidgets
			
			while(matchedPos < lastHoveredWidgets.Count)
			{
				Widget candidate = lastHoveredWidgets[matchedPos];
				if(candidate.Parent == current)	// Is it still a child of the current container ?
				{
					Gdk.Rectangle alloc = candidate.Allocation;
					if(!alloc.Contains (x, y))	// Does it contain the coordinate ?
					{
						break;
					}
				}
				current = candidate as Container;
				match = candidate;
				++matchedPos;
			}
			
			if(matchedPos < lastHoveredWidgets.Count)	// Not all widgets match
			{
				// Send a leave notify
				SendSyntheticEvent (EventType.LeaveNotify, evnt, X, XRoot, Y, YRoot, lastHoveredWidgets, matchedPos);
				
				// Remove them
				lastHoveredWidgets.RemoveRange (matchedPos, lastHoveredWidgets.Count - matchedPos);
			}
			
			while (current != null)
			{
				Container next = null;
				foreach(Widget child in current.Children)
				{
					if(child.IsNoWindow)
					{
						Gdk.Rectangle alloc = child.Allocation;
						if(alloc.Contains (x, y))
						{
							lastHoveredWidgets.Add (child);
							match = child;
							next = child as Container;
							break;
						}
					}
				}
				current = next;
			}
			
			if(matchedPos < lastHoveredWidgets.Count)	// New widgets have been found
			{
				// Send an enter notify
				SendSyntheticEvent (EventType.EnterNotify, evnt, X, XRoot, Y, YRoot, lastHoveredWidgets, matchedPos);
			}
			
			if(match == this)	// No widget found, the window keeps the event
			{
				return base.OnWidgetEvent (evnt);
			}
			else	// A widget has been found, let's send it the event
			{
				match.ProcessEvent (evnt);
				return true;
			}
		}
		
		private void SendSyntheticEvent (EventType Type, Event OriginalEvent, double X, double XRoot, double Y, double YRoot, IList<Widget> Widgets, int Index)
		{
			SyntheticEventCrossing se = new SyntheticEventCrossing();
			se.Detail = NotifyType.Ancestor;
			se.Focus = false;
			se.Mode = CrossingMode.Normal;
			se.SendEvent = false;
			se.State = ModifierType.None;
			se.Subwindow = null;
			//se.Time = DateTime.Now.Ticks / 10000;	// TODO: the real value shoud be the uptime I think
			se.Time = 0;
			se.Type = Type;
			se.Window = OriginalEvent.Window;
			se.X = X;
			se.XRoot = XRoot;
			se.Y = Y;
			se.YRoot = YRoot;
			
			unsafe
			{
				Event managedEvent = new Event(new IntPtr(&se));
				
				for(int i = Index ; i < Widgets.Count ; ++i)
				{
					Widgets[i].ProcessEvent (managedEvent);
				}
			}
		}
	}
}
