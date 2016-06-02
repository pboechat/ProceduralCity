using System;

namespace Pattern
{
	public class FacadeOperation : IComparable<FacadeOperation>, IPatternItem
	{
		private char _symbol;
		private int _index;
		
		public FacadeOperation ()
		{
		}
		
		private FacadeOperation (char symbol, int index)
		{
			this._symbol = symbol;
			this._index = index;
		}
		
		public void Initialize (char symbol, int index, ArchitectureStyle architectureStyle)
		{
			this._symbol = symbol;
			this._index = index;
		}
		
		public bool extrude {
			get {
				return _symbol == 'x';
			}
		}
		
		public bool caveIn {
			get {
				return _symbol == 'c';
			}
		}
		
		public bool none {
			get {
				return _symbol == 'n';
			}
		}
		
		public int level {
			get {
				return _index;
			}
		}
		
		public int CompareTo (FacadeOperation other)
		{
			int c = _symbol.CompareTo (other._symbol);
			if (c == 0) {
				return _index.CompareTo (other._index);
			} else {
				return c;
			}
		}
		
		public object Clone ()
		{
			return new FacadeOperation (_symbol, _index);
		}
		
		public override bool Equals (System.Object obj)
		{
			if (obj == null) {
				return false;
			}

			FacadeOperation other = (FacadeOperation)obj;
			if ((System.Object)other == null) {
				return false;
			}

			return (_symbol == other._symbol) && (_index == other._index);
		}

		public bool Equals (FacadeOperation other)
		{
			if ((object)other == null) {
				return false;
			}

			return (_symbol == other._symbol) && (_index == other._index);
		}
		
		public static bool operator == (FacadeOperation a, FacadeOperation b)
		{
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			return (a._symbol == b._symbol) && (a._index == b._index);
		}

		public static bool operator != (FacadeOperation a, FacadeOperation b)
		{
			return !(a == b);
		}

		public override int GetHashCode ()
		{
			return (int)_symbol ^ _index;
		}
	}
}