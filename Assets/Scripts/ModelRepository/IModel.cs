using UnityEngine;
using System;
using System.Collections.Generic;

namespace ModelRepository
{
	public interface IModel
	{
		string Uid ();
		
		string Category ();
		
		GameObject Original ();
		
		GameObject Clone ();
		
		string Metadata (string key);
		
		char MetadataAsChar (string key);
		
		short MetadataAsShort (string key);
		
		int MetadataAsInt (string key);
		
		long MetadataAsLong (string key);
		
		float MetadataAsFloat (string key);
		
		double MetadataAsDouble (string key);
		
		bool MetadataAsBool (string key);

		Dictionary<string, string> AllMetadata ();
		
	}
	
}