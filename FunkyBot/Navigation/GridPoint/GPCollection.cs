using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;


namespace FunkyBot.Movement
{

	[Flags]
	public enum LocationFlags
	{
		None = 0,
		Right = 1,
		Left = 2,
		Bottom = 3,
		Top = 4,

		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right,

		TopLeft = Top | Left,
		TopRight = Top | Right,
	}


	///<summary>
	///Point collection that manipulates points to test specific things about the points contained using sectors or the entire area it covers.
	///</summary>
	public class PointCollection : IDictionary<GridPoint, QuadrantLocation>
	{
		//To quickly lookup points locations within the grid.
		//public Hashtable OutCodes=new Hashtable();
		internal Dictionary<GridPoint, QuadrantLocation> Points = new Dictionary<GridPoint, QuadrantLocation>();
		private List<GridPoint> BasePoints = new List<GridPoint>();

		public Dictionary<QuadrantLocation, GridPoint> CornerPoints = new Dictionary<QuadrantLocation, GridPoint>
				{
						  {QuadrantLocation.BottomRight, null},	 {QuadrantLocation.BottomLeft, null},
						  {QuadrantLocation.TopRight, null},		 {QuadrantLocation.TopLeft, null}, 
					 };

		private GridPoint tl, br, tr, bl;


		private GridPoint centerpoint_;
		public GridPoint centerpoint
		{
			get
			{
				return centerpoint_;
			}
			set
			{
				centerpoint_ = value;
				tl = value;//reset tl,br
				br = value;
			}
		}


		#region Quadrant Bytes
		private const int INSIDE = 0; // 0000
		private const int LEFT = 1;   // 0001
		private const int RIGHT = 2;  // 0010
		private const int BOTTOM = 4; // 0100
		private const int TOP = 8;    // 1000
		private byte ComputeOutCode(double x, double y)
		{
			byte code = INSIDE;
			if (x <= centerpoint_.X)           // to the left of center
				code |= LEFT;
			else if (x > centerpoint_.X)      // to the right of center
				code |= RIGHT;
			if (y >= centerpoint_.Y)           // below the center
				code |= BOTTOM;
			else if (y < centerpoint_.Y)      // above the center
				code |= TOP;
			return code;
		}
		#endregion


		public GridPoint[] QuadrantPoints(QuadrantLocation SectorCode, out GridPoint EndPoint)
		{
			GridPoint[] containedPoints =
			(from p in Points
			 where p.Value == SectorCode
			 select p.Key).ToArray();

			EndPoint = CornerPoints[SectorCode];

			GridPoint[] SortedPoints = containedPoints.OrderBy(p => !p.Ignored).ToArray();


			if (!SortedPoints.Any(p => !p.Ignored))
				return null;
			return SortedPoints;
		}

		public void Update(GridPoint point, QuadrantLocation SectorCode)
		{
			switch (SectorCode)
			{
				case QuadrantLocation.BottomRight:
					if (point > br) br = point;
					break;
				case QuadrantLocation.TopLeft:
					if (point < tl) tl = point;
					break;
			}
			bl = new GridPoint(tl.X, br.Y);
			tr = new GridPoint(br.X, tl.Y);

			CornerPoints[QuadrantLocation.BottomRight] = br;
			CornerPoints[QuadrantLocation.BottomLeft] = bl;
			CornerPoints[QuadrantLocation.TopLeft] = tl;
			CornerPoints[QuadrantLocation.TopRight] = tr;
		}

		public PointCollection()
		{

		}
		public PointCollection(PointCollection clone)
		{
			//this.bl=clone.bl;
			//this.br=clone.br;
			//this.tl=clone.tl;
			//this.tr=clone.tr;
			BasePoints = clone.Points.Keys.ToList();
			//this.CornerPoints=clone.CornerPoints;
			//this.centerpoint_=clone.centerpoint_;
		}


		#region ICollection<GridPoint> Members

		public void Add(GridPoint item)
		{
			QuadrantLocation code_ = (QuadrantLocation)ComputeOutCode(item.X, item.Y);
			Add(item, code_);
		}

		public void Clear()
		{
			//this.OutCodes.Clear();
			Points.Clear();
		}

		public bool Contains(GridPoint item)
		{
			return (Points.Keys.Contains(item) || BasePoints.Contains(item));
		}

		public void CopyTo(GridPoint[] array, int arrayIndex)
		{
			Points.Keys.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return Points.Count; }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public void Remove(GridPoint item)
		{
			Points.Remove(item);
		}

		#endregion



		#region IDictionary<GridPoint,QuadrantLocation> Members

		public void Add(GridPoint key, QuadrantLocation value)
		{
			Points.Add(key, value);
			Update(key, value);
		}

		public bool ContainsKey(GridPoint key)
		{
			return Points.ContainsKey(key);
		}

		public ICollection<GridPoint> Keys
		{
			get { return Points.Keys; }
		}

		bool IDictionary<GridPoint, QuadrantLocation>.Remove(GridPoint key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(GridPoint key, out QuadrantLocation value)
		{
			return Points.TryGetValue(key, out value);
		}

		public ICollection<QuadrantLocation> Values
		{
			get { return Points.Values; }
		}

		public QuadrantLocation this[GridPoint key]
		{
			get
			{
				return Points[key];
			}
			set
			{
				Points[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<GridPoint,QuadrantLocation>> Members

		public void Add(KeyValuePair<GridPoint, QuadrantLocation> item)
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<GridPoint, QuadrantLocation> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<GridPoint, QuadrantLocation>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<GridPoint, QuadrantLocation> item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<KeyValuePair<GridPoint,QuadrantLocation>> Members

		public IEnumerator<KeyValuePair<GridPoint, QuadrantLocation>> GetEnumerator()
		{
			return Points.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Points.GetEnumerator();
		}

		#endregion
	}


}