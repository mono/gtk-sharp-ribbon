using System;
using System.Collections.Generic;
using Gtk;

namespace Ribbons
{
	/// <summary>Popup gallery.</summary>
	internal class GalleryPopupWindow : Window
	{
		private const int MAX_HEIGHT = 200;
		private const int SCROLLBAR_SIZE = 20;
		
		private Gallery underlyingGallery;
		private List<Tile> tiles;
		private Dictionary<Tile, Tile> mapping;
		private uint rows, columns;
		private Tile selectedTile;

		private ScrolledWindow internalWindow;
		private Table tileTable;
		
		/// <summary>Returns the underlying gallery.</summary>
		public Gallery UnderlyingGallery
		{
			get { return underlyingGallery; }
		}
		
		/// <summary>Default constructor.</summary>
		/// <param name="UnderlyingGallery">The underlying gallery.</param>
		public GalleryPopupWindow (Gallery UnderlyingGallery) : base (WindowType.Popup)
		{
			this.underlyingGallery = UnderlyingGallery;
			this.tiles = new List<Tile> ();
			this.mapping = new Dictionary<Tile, Tile> ();
			foreach(Tile t in UnderlyingGallery.Tiles)
			{
				Tile copy = t.Copy ();
				tiles.Add (copy);
				
				if(t == UnderlyingGallery.SelectedTile)
				{
					copy.Selected = true;
					selectedTile = t;
				}
				
				mapping.Add (copy, t);
			}
			
			int width = UnderlyingGallery.Allocation.Width;
			
			columns = (uint)(width / underlyingGallery.TileWidth);
			rows = (uint)Math.Ceiling ((double)tiles.Count / columns);
			
			this.tileTable = new Table (rows, columns, true);
			this.tileTable.HeightRequest = (int)rows * UnderlyingGallery.TileHeight;
			this.tileTable.WidthRequest = (int)columns * UnderlyingGallery.TileWidth;
			
			Viewport vp = new Viewport ();
			vp.Child = tileTable;
			
			this.internalWindow = new ScrolledWindow ();
			this.internalWindow.Child = vp;
			this.internalWindow.HeightRequest = Math.Min (this.tileTable.HeightRequest, MAX_HEIGHT) + SCROLLBAR_SIZE;
			this.internalWindow.WidthRequest = this.tileTable.WidthRequest + SCROLLBAR_SIZE;
			
			uint x = 0, y = 0;
			foreach(Tile t in tiles)
			{
				ExtraEventBox box = new ExtraEventBox ();
				box.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
				box.Child = t;
				
				tileTable.Attach (box, x, x+1, y, y+1);
				t.Clicked += tile_Clicked;
				
				if(++x == columns)
				{
					x = 0;
					++y;
				}
			}
			
			this.Child = internalWindow;
		}
		
		private void tile_Clicked (object Sender, EventArgs e)
		{
			if(selectedTile != null) selectedTile.Selected = false;
			selectedTile = (Tile)Sender;
			selectedTile.Selected = true;
			
			underlyingGallery.SelectedTile = mapping[selectedTile];
			
			Hide ();
		}
	}
}
