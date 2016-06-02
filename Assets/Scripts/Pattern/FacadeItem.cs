using UnityEngine;
using System;
using ModelRepository;

namespace Pattern
{
	public class FacadeItem : IPatternItem
	{
		private IModel _model = null;
		private int _size = 0;
		private bool _allowsHeader = true;
		private bool _allowsDetail = true;
		private string _bottomVariant = "none";
		private string _topVariant = "none";
		private bool _invalid = true;
		
		public FacadeItem ()
		{
		}
		
		public FacadeItem (IModel model)
		{
			_model = model;
			
			if (_model == null) {
				throw new Exception ("model cannot be null");
			}
			
			CacheMetadata ();
		}
		
		private FacadeItem (IModel model, int size, bool allowsHeader, bool allowsDetail, string bottomVariant, string topVariant, bool invalid)
		{
			_model = model;
			_size = size;
			_allowsHeader = allowsHeader;
			_allowsDetail = allowsDetail;
			_bottomVariant = bottomVariant;
			_topVariant = topVariant;
			_invalid = invalid;
		}

		void CacheMetadata ()
		{
			_size = _model.MetadataAsInt ("size");
			_allowsHeader = _model.Category () != "top_variant" || _model.MetadataAsBool ("allows_header");
			_allowsDetail = _model.MetadataAsBool ("allows_detail");
			_bottomVariant = _model.Metadata ("bottom_variant");
			_topVariant = _model.Metadata ("top_variant");
		}
		
		public void Initialize (char symbol, int index, ArchitectureStyle style)
		{
			if (symbol == 'e') {
				return;
			}
			
			IResultSet resultSet = Repository.List ("facade_item").Filter ("style", style.name).Filter ("symbol", symbol + "").Filter ("index", index + "");
			
			if (resultSet.Count () == 0) {
				throw new Exception ("model not found (style: " + style.name + ", element: " + symbol + index + ")");
			}
			
			_model = resultSet.Get (0);
			
			if (_model == null) {
				throw new Exception ("invalid model (style: " + style.name + ", element: " + symbol + index + ")");
			}
			
			CacheMetadata ();
			
			_invalid = false;
		}
		
		public IModel model {
			get {
				return _model;
			}
		}
		
		public bool invalid {
			get {
				return _invalid;
			}
		}
		
		public int size {
			get {
				return _size;
			}
		}
		
		public bool allowsHeader {
			get {
				return _allowsHeader;
			}
		}
		
		public bool allowsDetail {
			get {
				return _allowsDetail;
			}
		}
		
		public GameObject NewModelInstance ()
		{
			if (_model == null) {
				return null;
			}
			
			return _model.Clone ();
		}
		
		public void Invalidate ()
		{
			_invalid = true;
		}
		
		public void BottomVariant ()
		{
			if (_bottomVariant == "none") {
				return;
			}
			
			_model = Repository.Get (_bottomVariant);
			
			CacheMetadata ();
		}
		
		public void TopVariant ()
		{
			if (_topVariant == "none") {
				return;
			}
			
			_model = Repository.Get (_topVariant);
			
			CacheMetadata ();
		}
		
		public object Clone ()
		{
			return new FacadeItem (_model, _size, _allowsHeader, _allowsDetail, _bottomVariant, _topVariant, _invalid);
		}
		
		public override bool Equals (System.Object obj)
		{
			if (obj == null) {
				return false;
			}

			FacadeItem other = (FacadeItem)obj;
			if ((System.Object)other == null) {
				return false;
			}

			return (_model == other._model);
		}

		public bool Equals (FacadeItem other)
		{
			if ((object)other == null) {
				return false;
			}

			return (_model == other._model);
		}
		
		public static bool operator == (FacadeItem a, FacadeItem b)
		{
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			return a._model == b._model;
		}

		public static bool operator != (FacadeItem a, FacadeItem b)
		{
			return !(a == b);
		}

		public override int GetHashCode ()
		{
			return _model.GetHashCode ();
		}
		
	}
	
}