using System;
using System.Collections.Generic;
using Cairo;
using Gtk;

namespace Ribbons
{
	/// <summary>Gallery of Tiles.</summary>
	public class Gallery : Container
	{
		protected Theme theme = new Theme ();
		private int tileWidth, tileHeight;
		private List<Tile> tiles;
		private Button up, down, expand;
		private int defaultTilesPerRow;
		private int tileSpacing;
		
		private Tile selectedTile;
		private int firstDisplayedTileIndex, lastDisplayedTileIndex;
		private int btnWidth;
		private Requisition upReq, downReq, expandReq;
		private Gdk.Rectangle tilesAlloc;
		
		private const double space = 2.0;
		private const double lineWidth = 1.0;
		
		/// <summary>Fired when a Tile has been selected.</summary>
		public event TileSelectedHandler TileSelected;
		
		/// <summary>Gets or sets the width of Tiles.</summary>
		public int TileWidth
		{
			set
			{
				tileWidth = value;
				QueueDraw ();
			}
			get { return tileWidth; }	
		}
		
		/// <summary>Gets or sets the height of Tiles.</summary>
		public int TileHeight
		{
			set
			{
				tileHeight = value;
				QueueDraw ();
			}
			get { return tileHeight; }
		}
		
		/// <summary>Gets or sets the spacing between Tiles.</summary>
		public int TileSpacing
		{
			set
			{
				tileSpacing = value;
				QueueDraw ();
			}
			get { return tileSpacing; }
		}
		
		/// <summary>Gets or sets the default number of Tiles per row.</summary>
		public int DefaultTilesPerRow
		{
			set
			{
				defaultTilesPerRow = value;
				QueueDraw ();
			}
			get { return defaultTilesPerRow; }
		}
		
		/// <summary>Gets or sets the selected Tile.</summary>
		public Tile SelectedTile
		{
			set
			{
				if(selectedTile != null) selectedTile.Selected = false;
				selectedTile = value;
				if(selectedTile != null)
				{
					selectedTile.Selected = true;
					
					int idx = tiles.FindIndex (delegate (Tile t) { return t == selectedTile; });
					if(idx != -1)
					{
						firstDisplayedTileIndex = idx;
						lastDisplayedTileIndex = -1;
						UpdateTilesLayout ();
						QueueDraw ();
					}
				}
			}
			get { return selectedTile; }
		}
		
		/// <summary>Returns all Tiles.</summary>
		public IEnumerable<Tile> Tiles
		{
			get
			{
				return tiles;
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
		
		/// <summary>Default constructor.</summary>
		public Gallery()
		{
			this.SetFlag (WidgetFlags.NoWindow);
			
			this.AddEvents ((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask));
			
			this.tiles = new List<Tile> ();
			
			this.defaultTilesPerRow = 3;
			this.firstDisplayedTileIndex = 0;
			this.lastDisplayedTileIndex = -1;
			
			this.tileHeight = 56;
			this.tileWidth = 72;
			this.tileSpacing = 0;
			this.BorderWidth = 2;
			
			this.up = new Button ("\u25B2");
			this.up.Parent = this;
			this.up.Padding = 0;
			this.up.Clicked += up_Clicked;
			
			this.down = new Button ("\u25BC");
			this.down.Parent = this;
			this.down.Padding = 0;
			this.down.Clicked += down_Clicked;
			
			this.expand = new Button ("\u2193");
			this.expand.Parent = this;
			this.expand.Padding = 0;
			this.expand.Clicked += expand_Clicked;
		}
		
		/// <summary>Adds a tile before all existing tiles.</summary>
		/// <param name="t">The tile to add.</param>
		public void PrependTile (Tile t)
		{
			InsertTile (t, 0);
		}
		
		/// <summary>Adds a tile after all existing tiles.</summary>
		/// <param name="t">The tile to add.</param>
		public void AppendTile (Tile t)
		{
			InsertTile (t, -1);
		}
		
		/// <summary>Inserts a tile at the specified location.</summary>
		/// <param name="t">The tile to add.</param>
		/// <param name="TileIndex">The index (starting at 0) at which the tile must be inserted, or -1 to insert the tile after all existing tiles.</param>
		public void InsertTile (Tile t, int TileIndex)
		{
			if(TileIndex == -1 || TileIndex == tiles.Count)
			{
				tiles.Add (t);
			}
			else
			{
				tiles.Insert (TileIndex, t);
			}
			
			//t.Parent = this;
			t.Visible = true;
			t.Clicked += Tile_Clicked;
		}
		
		/// <summary>Removes the tile at the specified index.</summary>
		/// <param name="TileIndex">Index of the tile to remove.</param>
		public void RemoveTile (int TileIndex)
		{
			Tile t = tiles[TileIndex];
			t.Clicked -= Tile_Clicked;
			t.Unparent ();
			if(selectedTile == t) selectedTile = null;
			
			tiles.RemoveAt (TileIndex);
		}
		
		private void up_Clicked(object Sender, EventArgs e)
		{
			MoveUp ();
		}
		
		private void down_Clicked(object Sender, EventArgs e)
		{
			MoveDown ();
		}
		
		private void expand_Clicked(object Sender, EventArgs e)
		{
			GalleryPopupWindow popup = new GalleryPopupWindow (this);
			popup.ShowAll ();
			
			int x, y;
			ParentWindow.GetRootOrigin (out x, out y);
			x += Allocation.X;
			y += Allocation.Y;
			
			popup.GdkWindow.Move (x, y);
		}
		
		private void Tile_Clicked(object Sender, EventArgs e)
		{
			if(selectedTile != null) selectedTile.Selected = false;
			selectedTile = (Tile)Sender;
			selectedTile.Selected = true;
			OnTileSelected (selectedTile);
		}
		
		private void MoveUp ()
		{
			if(firstDisplayedTileIndex > 0)
			{
				firstDisplayedTileIndex = -1;
				lastDisplayedTileIndex = firstDisplayedTileIndex - 1;
			}
			UpdateTilesLayout ();
			QueueDraw ();
		}
		
		private void MoveDown ()
		{
			if(lastDisplayedTileIndex < tiles.Count - 1)
			{
				firstDisplayedTileIndex = lastDisplayedTileIndex + 1;
				lastDisplayedTileIndex = -1;
			}
			UpdateTilesLayout ();
			QueueDraw ();
		}
		
		/// <summary>Fires the SelectedTile event.</summary>
		/// <param name="SelectedTile">The Tile that has been selected.</param>
		protected void OnTileSelected (Tile SelectedTile)
		{
			if(TileSelected != null) TileSelected (this, new TileSelectedEventArgs (SelectedTile));
		}
		
		private void UpdateTilesLayout ()
		{
			Gdk.Rectangle tileAlloc;
			tileAlloc.X = tilesAlloc.X;
			tileAlloc.Y = tilesAlloc.Y;
			tileAlloc.Height = tilesAlloc.Height;
			tileAlloc.Width = tileWidth;
			
			int maxTiles = (tilesAlloc.Width + tileSpacing) / (tileWidth + tileSpacing);
			
			if(firstDisplayedTileIndex == -1)
			{
				firstDisplayedTileIndex = lastDisplayedTileIndex - maxTiles + 1;
				if(firstDisplayedTileIndex < 0)
				{
					lastDisplayedTileIndex -= firstDisplayedTileIndex;
					firstDisplayedTileIndex = 0;
				}
			}
			else if(lastDisplayedTileIndex == -1)
			{
				lastDisplayedTileIndex = firstDisplayedTileIndex + maxTiles - 1;
			}
			
			if(lastDisplayedTileIndex >= tiles.Count)
			{
				lastDisplayedTileIndex = tiles.Count - 1;
			}
			
			up.Enabled = firstDisplayedTileIndex > 0;
			down.Enabled = lastDisplayedTileIndex < tiles.Count - 1;
			
			for(int tileIndex = 0 ; tileIndex < firstDisplayedTileIndex ; ++tileIndex)
			{
				if(tiles[tileIndex].Parent != null) tiles[tileIndex].Unparent ();
			}
			for(int tileIndex = lastDisplayedTileIndex + 1 ; tileIndex < tiles.Count ; ++tileIndex)
			{
				if(tiles[tileIndex].Parent != null) tiles[tileIndex].Unparent ();
			}
			
			for(int tileIndex = firstDisplayedTileIndex ; tileIndex <= lastDisplayedTileIndex ; ++tileIndex)
			{
				Tile t = tiles[tileIndex];
				
				if(t.Parent == null) t.Parent = this;
				
				t.SizeRequest ();
				
				t.SizeAllocate (tileAlloc);
				tileAlloc.X += tileAlloc.Width + tileSpacing;
			}
		}
		
		protected override void ForAll (bool include_internals, Callback callback)
		{
			//if(include_internals)
			{
				callback (up);
				callback (down);
				callback (expand);
			}
			
			for(int i = firstDisplayedTileIndex ; i <= lastDisplayedTileIndex ; ++i)
			{
				callback (tiles[i]);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			base.OnSizeRequested (ref requisition);
			
			upReq = up.SizeRequest ();
			downReq = down.SizeRequest ();
			expandReq = expand.SizeRequest ();
			
			btnWidth = Math.Max (upReq.Width, Math.Max (downReq.Width, expandReq.Width));
			int btnHeight = upReq.Height + downReq.Height + expandReq.Height;
			
			int count = Math.Min (tiles.Count, defaultTilesPerRow);
			requisition.Width = btnWidth + (int)space + 2 * (int)lineWidth + count * tileWidth + (count + 1) * tileSpacing + 2 * (int)BorderWidth;
			requisition.Height = Math.Max (tileHeight + 2 * (tileSpacing + (int)lineWidth), btnHeight) + 2 * (int)BorderWidth;
			
			if(WidthRequest != -1) requisition.Width = WidthRequest;
			if(HeightRequest != -1) requisition.Height = HeightRequest;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			allocation.X += (int)BorderWidth;
			allocation.Y += (int)BorderWidth;
			allocation.Width -= 2 * (int)BorderWidth;
			allocation.Height -= 2 * (int)BorderWidth;
			
			Gdk.Rectangle btnAlloc;
			btnAlloc.Width = btnWidth;
			btnAlloc.X = allocation.X + allocation.Width - btnAlloc.Width + 1;
			
			btnAlloc.Y = allocation.Y;
			btnAlloc.Height = upReq.Height;
			up.SizeAllocate (btnAlloc);
			
			btnAlloc.Y += btnAlloc.Height;
			btnAlloc.Height = downReq.Height;
			down.SizeAllocate (btnAlloc);
			
			btnAlloc.Y += btnAlloc.Height;
			btnAlloc.Height = expandReq.Height;
			expand.SizeAllocate (btnAlloc);
			
			tilesAlloc.X = allocation.X + (int)lineWidth + tileSpacing;
			tilesAlloc.Y = allocation.Y + (int)lineWidth + tileSpacing;
			tilesAlloc.Width = btnAlloc.X - tilesAlloc.X - tileSpacing - (int)space - 2 * (int)lineWidth;
			tilesAlloc.Height = allocation.Height - 2 * (tileSpacing + (int)lineWidth); 
			
			UpdateTilesLayout ();
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (this.GdkWindow);
			
			cr.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
			cr.Clip ();
			Draw (cr);
			
			return base.OnExposeEvent (evnt);
		}
		
		protected void Draw (Context cr)
		{
			Rectangle alloc = new Rectangle (Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
			Rectangle tiles = new Rectangle (tilesAlloc.X - tileSpacing, tilesAlloc.Y - tileSpacing, tilesAlloc.Width + 2 * tileSpacing, tilesAlloc.Height + 2 * tileSpacing);
			theme.DrawGallery (cr, alloc, tiles, this);
		}
	}
}
