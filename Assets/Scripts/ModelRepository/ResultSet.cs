using UnityEngine;
using System;
using System.Collections.Generic;

namespace ModelRepository
{
	public class ResultSet : IResultSet
	{
		private List<IModel> _models;
			
		public ResultSet ()
		{
			_models = null;
		}
			
		public ResultSet (List<IModel> models)
		{
			_models = models;
		}
			
		public List<IModel> ToList ()
		{
			return _models;
		}
		
		public IModel Single ()
		{
			if (_models == null || _models.Count == 0) {
				return null;
			} else {
				return _models [0];
			}
		}
		
		public int Count ()
		{
			return _models.Count;
		}
		
		public IModel Get (int index)
		{
			return _models [index];
		}
		
		public IModel Random (bool remove)
		{
			IModel randomModel = _models [UnityEngine.Random.Range (0, _models.Count)];
			if (remove) {
				_models.Remove (randomModel);
			}
			return randomModel;
		}
			
		public bool IsEmpty ()
		{
			return _models == null || _models.Count > 0;
		}
			
		public IResultSet Filter (string key, string _value)
		{
			List<IModel> filteredItems = new List<IModel> ();
			foreach (IModel model in _models) {
				string __value;
				if ((__value = model.Metadata (key)) != null && __value == _value) {
					filteredItems.Add (model);
				}
			}
				
			return new ResultSet (filteredItems);
		}
	}
	
}