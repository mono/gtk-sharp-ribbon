using System;

namespace Ribbons
{
	public class TileSelectedEventArgs
	{
		private Tile selTile;
		
		/// <summary>The tile that has been selected.</summary>
		public Tile SelectedTile
		{
			get { return selTile; }
		}
		
		public TileSelectedEventArgs (Tile SelectedTile)
		{
			selTile = SelectedTile;
		}
	}
}
