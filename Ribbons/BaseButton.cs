using System;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>Foundation of all buttons.</summary>
	public abstract class BaseButton : Bin
	{
		protected Theme theme = new Theme ();
		protected GroupStyle groupStyle;
		protected Theme.ButtonState state = Theme.ButtonState.Default;
		protected PositionType imgPos;
		protected bool drawBg;
		protected Widget img;
		protected Label lbl;
		protected double padding;
		protected bool isSmall;
		protected bool enable;
		
		/// <summary>Spacing between the content and the widget.</summary>
		public double Padding
		{
			set
			{
				if(padding == value) return;
				padding = value;
				QueueDraw ();
			}
			get { return padding; }
		}
		
		/// <summary>Shape of the widget.</summary>
		public GroupStyle GroupStyle
		{
			set { groupStyle = value; }
			get { return groupStyle; }
		}
		
		/// <summary><b>true</b> if the widget should paint a background, <b>false</B> otherwise.</summary>
		public bool DrawBackground
		{
			set
			{
				if(drawBg == value) return;
				drawBg = value;
				QueueDraw ();
			}
			get { return drawBg; }
		}
		
		/// <summary><b>true</b> if the button is enabled, <b>false</B> otherwise.</summary>
		public bool Enabled
		{
			set
			{
				if(enable == value) return;
				enable = value;
				QueueDraw ();
			}
			get { return enable; }
		}
		
		/// <summary>Image to display.</summary>
		public Widget Image
		{
			set
			{
				if(img == value) return;
				if(img != null) UnbindWidget (img);
				img = value;
				if(img != null) BindWidget (img);
				UpdateImageLabel ();
			}
			get { return img; }
		}
		
		/// <summary>Position of the image relative to the label.</summary>
		public PositionType ImagePosition
		{
			set
			{
				if(imgPos == value) return;
				imgPos = value;
				UpdateImageLabel ();
			}
			get { return imgPos; }
		}
		
		/// <summary>Label to display.</summary>
		public string Label
		{
			set
			{
				if(lbl != null) UnbindWidget (lbl);
				lbl = new Gtk.Label (value);
				if(lbl != null) BindWidget (lbl);
				UpdateImageLabel ();
			}
			get
			{
				return lbl == null ? null : lbl.Text;
			}
		}
		
		/// <summary>Theme used to draw the widget.</summary>
		public Theme Theme
		{
			set
			{
				theme = value;
				QueueDraw ();
			}
			get { return theme; }
		}
		
		/// <summary>Binds a widget to listen to all button events.</summary>
		protected void BindWidget (Widget w)
		{
			w.ButtonPressEvent += BindedWidget_ButtonPressEvent;
			w.ButtonReleaseEvent += BindedWidget_ButtonReleaseEvent;
		}
		
		/// <summary>Unbinds a widget to no longer listen to button events.</summary>
		protected void UnbindWidget (Widget w)
		{
			w.ButtonPressEvent -= BindedWidget_ButtonPressEvent;
			w.ButtonReleaseEvent -= BindedWidget_ButtonReleaseEvent;
		}
		
		/// <summary>Called when a mouse button has been pressed on a binded widget.</summary>
		protected abstract void BindedWidget_ButtonPressEvent (object sender, ButtonPressEventArgs evnt);
		
		/// <summary>Called when a mouse button has been release on a binded widget.</summary>
		protected abstract void BindedWidget_ButtonReleaseEvent (object sender, ButtonReleaseEventArgs evnt);
		
		/// <summary>Updates the child widget containing the label and/or image.</summary>
		protected void UpdateImageLabel ()
		{
			if(Child != null)
			{
				Container con = Child as Container;
				if(con != null)
				{
					con.Remove (img);
					con.Remove (lbl);
				}
				Remove (Child);
			}
			
			if(lbl != null && img != null)
			{
				switch(imgPos)
				{
					case PositionType.Top:
					{
						VBox box = new VBox (false, 0);
						box.Add (img);
						box.Add (lbl);
						Child = box;
						break;
					}
					case PositionType.Bottom:
					{
						VBox box = new VBox (false, 0);
						box.Add (lbl);
						box.Add (img);
						Child = box;
						break;
					}
					case PositionType.Left:
					{
						HBox box = new HBox (false, 0);
						box.Add (img);
						box.Add (lbl);
						Child = box;
						break;
					}
					case PositionType.Right:
					{
						HBox box = new HBox (false, 0);
						box.Add (lbl);
						box.Add (img);
						Child = box;
						break;
					}
				}
			}
			else if(lbl != null)
			{
				Child = lbl;
			}
			else if(img != null)
			{
				Child = img;
			}
		}
	}
}
