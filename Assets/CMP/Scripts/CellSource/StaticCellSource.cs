using UnityEngine;

namespace CMP.Scripts.CellSource
{
	public class StaticCellSource : ICellSource
	{
		private readonly Vector2Int _cell;

		public StaticCellSource(Vector2Int cell)
		{
			_cell = cell;
		}
		
		public Vector2Int GetCell()
		{
			return _cell;
		}
	}
}