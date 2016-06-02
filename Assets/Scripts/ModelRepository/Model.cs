using UnityEngine;
using System;
using System.Collections.Generic;

namespace ModelRepository
{
	public class Model : IModel
	{
		private string _uid;
		private string _path;
		private string _category;
		private Dictionary<string, string> _metadata;
		private GameObject _original = null;
		private bool _firstLoad = true;
	
		public Model (string uid, string path, string category, Dictionary<string, string> metadata)
		{
			_uid = uid;
			_path = path;
			_category = category;
			_metadata = metadata;
		}
		
		public string Uid ()
		{
			return _uid;
		}
		
		public string Category ()
		{
			return _category;
		}
		
		public GameObject Original ()
		{
			if (_firstLoad) {
				_original = (GameObject)Resources.Load (Repository.GetRoot () + "/" + _path, typeof(GameObject));
				_firstLoad = false;
			}
			
			if (_original == null) {
				throw new Exception ("asset not found: " + _path);
			}
			
			return _original;
		}
		
		public GameObject Clone ()
		{
			GameObject original = Original ();
			return (GameObject)MonoBehaviour.Instantiate (original);
		}
		
		public string Metadata (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value)) {
				return _value;
			} else {
				return null;
			}
		}
		
		public char MetadataAsChar (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value) && _value.Length == 1) {
				return _value [0];
			} else {
				return '\0';
			}
		}
		
		public short MetadataAsShort (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value)) {
				short result;
				if (short.TryParse (_value, out result)) {
					return result;
				} else {
					return 0;
				}
			} else {
				return 0;
			}
		}
		
		public int MetadataAsInt (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value)) {
				int result;
				if (int.TryParse (_value, out result)) {
					return result;
				} else {
					return 0;
				}
			} else {
				return 0;
			}
		}
		
		public long MetadataAsLong (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value)) {
				long result;
				if (long.TryParse (_value, out result)) {
					return result;
				} else {
					return 0;
				}
			} else {
				return 0;
			}
		}
		
		public float MetadataAsFloat (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value)) {
				float result;
				if (float.TryParse (_value, out result)) {
					return result;
				} else {
					return 0;
				}
			} else {
				return 0;
			}
		}
		
		public double MetadataAsDouble (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value)) {
				double result;
				if (double.TryParse (_value, out result)) {
					return result;
				} else {
					return 0;
				}
			} else {
				return 0;
			}
		}
		
		public bool MetadataAsBool (string key)
		{
			string _value;
			if (_metadata.TryGetValue (key, out _value)) {
				bool result;
				if (bool.TryParse (_value, out result)) {
					return result;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}
		
		public Dictionary<string, string> AllMetadata ()
		{
			return _metadata;
		}
		
		public override bool Equals (System.Object obj)
		{
			if (obj == null) {
				return false;
			}
			
			Model other = (Model)obj;
			if ((System.Object)other == null) {
				return false;
			}
			
			return _uid.Equals (other._uid);
		}
		
		public bool Equals (Model other)
		{
			if ((object)other == null) {
				return false;
			}

			return _uid.Equals (other._uid);
		}
		
		public static bool operator == (Model a, Model b)
		{
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}

			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			return a._uid == b._uid;
		}

		public static bool operator != (Model a, Model b)
		{
			return !(a == b);
		}
		
		public override int GetHashCode ()
		{
			return _uid.GetHashCode ();
		}
		
	}
	
}