using System;
using System.Runtime.InteropServices;
using Gdk;

namespace Ribbons
{
	/// <summary>Managed EventCrossing implementation.</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct SyntheticEventCrossing
	{
		[MarshalAs(UnmanagedType.SysInt)]
		private int type;	// GdkEventType
		private IntPtr window;	// GdkWindow
		private byte send_event;
		private IntPtr subwindow;	// GdkWindow
		private UInt32 time;
		private double x;
		private double y;
		private double root_x;
		private double root_y;
		[MarshalAs(UnmanagedType.SysInt)]
		private int mode;	// GdkCrossingMode
		[MarshalAs(UnmanagedType.SysInt)]
		private int detail;	// GdkNotifyType
		[MarshalAs(UnmanagedType.SysInt)]
		private int focus;	// gboolean
		[MarshalAs(UnmanagedType.SysUInt)]
		private uint state;
		
		public EventType Type
		{
			set { type = (int)value; }
			get { return (EventType)type; }
		}
		
		public Window Window
		{
			set { window = value == null ? IntPtr.Zero : value.Handle; }
			get { return window == IntPtr.Zero ? null : new Window (window); }
		}
		
		public bool SendEvent
		{
			set { send_event = value ? (byte)1 : (byte)0; }
			get { return send_event != 0; }
		}
		
		public Window Subwindow
		{
			set { subwindow = value == null ? IntPtr.Zero : value.Handle; }
			get { return subwindow == IntPtr.Zero ? null : new Window (subwindow); }
		}
		
		public UInt32 Time
		{
			set { time = value; }
			get { return time; }
		}
		
		public double X
		{
			set { x = value; }
			get { return x; }
		}
		
		public double Y
		{
			set { y = value; }
			get { return y; }
		}
		
		public double XRoot
		{
			set { root_x = value; }
			get { return root_x; }
		}
		
		public double YRoot
		{
			set { root_y = value; }
			get { return root_y; }
		}
		
		public CrossingMode Mode
		{
			set { mode = (int)value; }
			get { return (CrossingMode)mode; }
		}
		
		public NotifyType Detail
		{
			set { detail = (int)value; }
			get { return (NotifyType)detail; }
		}
		
		public bool Focus
		{
			set { focus = value ? 1 : 0; }
			get { return focus != 0; }
		}
		
		public ModifierType State
		{
			set { state = (uint)value; }
			get { return (ModifierType)state; }
		}
	}
}
