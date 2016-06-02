using UnityEngine;
using System;
using System.Collections.Generic;

namespace ModelRepository
{
	public interface IResultSet
	{
		List<IModel> ToList ();
		
		IModel Single ();
		
		IModel Get (int index);
		
		IModel Random (bool remove = false);
		
		int Count ();
		
		bool IsEmpty ();

		IResultSet Filter (string key, string _value);
			
	}
	
}