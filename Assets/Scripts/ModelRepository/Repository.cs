using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ModelRepository
{
	public class Repository : MonoBehaviour
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private class NullResultSet : IResultSet
		{
			public List<IModel> ToList ()
			{
				return null;
			}
			
			public IModel Single ()
			{
				return null;
			}
			
			public IModel Get (int index)
			{
				return null;
			}
			
			public IModel Random (bool remove)
			{
				return null;
			}
			
			public int Count ()
			{
				return 0;
			}
			
			public bool IsEmpty ()
			{
				return true;
			}
			
			public IResultSet Filter (string key, string _value)
			{
				return this;
			}
			
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private class Category
		{
			private string _name;
			private List<string> _fields = new List<string> ();
			
			public Category (string name, List<string> fields)
			{
				_name = name;
				_fields = fields;
			}
			
			public string name {
				get {
					return this._name;
				}
			}
			
			public List<string> fields {
				get {
					return this._fields;
				}
			}			

		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private const string KEY_VALUE_PATTERN = @"([a-zA-Z0-9_]+)[ ]*=[ ]*([a-zA-Z0-9_,\.]+)[ ]*";
		private const string FIELD_PATTERN = @"([a-zA-Z0-9_]+)";
		private static readonly IResultSet NULL_RESULT_SET = new NullResultSet ();
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		[SerializeField]
		private string _configurationFile;
		private static string _root;
		private static bool _strict;
		private static bool _initialized = false;
		private static Dictionary<string, IModel> _models = new Dictionary<string, IModel> ();
		private static Dictionary<string, Category> _categories = new Dictionary<string, Category> ();
		private static Dictionary<string, List<IModel>> _categorizedModels = new Dictionary<string, List<IModel>> ();
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		void ValidateModel (Category category, Model model)
		{
			Dictionary<string, string> metadata = model.AllMetadata ();
			
			foreach (string field in category.fields) {
				if (!metadata.ContainsKey (field)) {
					throw new Exception ("missing metadata for model (model: " + model.Uid () + ", metadata: " + field + ")");
				}
			}
			
			if (_strict) {
				foreach (string key in metadata.Keys) {
					if (!category.fields.Contains (key)) {
						throw new Exception ("undefined field for category (category: " + category.name + ", model: " + model.Uid () + ", field: " + key + ")");
					}
				}
			}
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		Category ParseCategory (XmlNode node)
		{
			XmlAttribute nameAttr = node.Attributes ["name"];
			
			if (nameAttr == null) {
				throw new Exception ("name cannot be null");
			}
			
			string name = nameAttr.Value.Trim ();
			
			List<string> fields = new List<string> ();
			string definition = node.InnerText;
			Match match = Regex.Match (definition, FIELD_PATTERN, RegexOptions.Multiline);
			while (match.Success) {
				// FIXME: checking invariants
				if (match.Groups.Count != 2) {
					throw new Exception ("match.Groups.Count > 2");
				}
				
				string field = match.Groups [1].Value;
				fields.Add (field);
				match = match.NextMatch ();
			}
			
			return new Category (name, fields);
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		Model ParseModel (XmlNode node)
		{
			XmlAttribute uidAttr = node.Attributes ["uid"];
			
			if (uidAttr == null) {
				throw new Exception ("uid cannot be null");
			}
			
			string uid = uidAttr.Value.Trim ();
			
			if (string.IsNullOrEmpty (uid)) {
				throw new Exception ("uid cannot be empty");
			}
			
			XmlAttribute pathAttr = node.Attributes ["path"];
			
			if (pathAttr == null) {
				throw new Exception ("path cannot be null (model: " + uid + ")");
			}
			
			string path = pathAttr.Value.Trim ();
			
			if (string.IsNullOrEmpty (path)) {
				throw new Exception ("path cannot be empty (model: " + uid + ")");
			}
			
			XmlNode metadataNode = node.SelectSingleNode ("Metadata");
			
			Dictionary<string, string> metadata = new Dictionary<string, string> ();
			if (metadataNode != null) {
				string metadataText = metadataNode.InnerText;
				
				Match match = Regex.Match (metadataText, KEY_VALUE_PATTERN, RegexOptions.Multiline);
				while (match.Success) {
					// FIXME: checking invariants
					if (match.Groups.Count != 3) {
						throw new Exception ("match.Groups.Count > 3");
					}
					
					string key = match.Groups [1].Value;
					string _value = match.Groups [2].Value;
					metadata.Add (key, _value);
					match = match.NextMatch ();
				}
			}
			
			XmlAttribute categoryAttr = node.Attributes ["category"];
			
			string category = null;
			if (categoryAttr != null) {
				category = categoryAttr.Value.Trim ();
			}
			
			return new Model (uid, path, category, metadata);
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		void LoadFromFile ()
		{
			TextAsset textFile = (TextAsset)Resources.Load (_configurationFile, typeof(TextAsset));
			StringReader reader = new StringReader (textFile.text);
			
			XmlDocument document = new XmlDocument ();
			document.Load (reader);
			
			XmlElement element = document.DocumentElement;
			
			if (element.Name != "ModelRepository") {
				throw new Exception ("first xml element must be <ModelRepository>");
			}
			
			XmlAttribute rootAttr = element.Attributes ["root"];
			
			if (rootAttr != null) {
				_root = rootAttr.Value.Trim ();
			}
			
			XmlAttribute strictAttr = element.Attributes ["strict"];
			
			if (strictAttr != null) {
				bool.TryParse (strictAttr.Value, out _strict);
			} else {
				_strict = false;
			}
			
			XmlNodeList categoriesNodeList = document.SelectNodes ("/ModelRepository/Categories/*");
			
			if (categoriesNodeList.Count > 0) {
				foreach (XmlNode categoryNode in categoriesNodeList) {
					Category category = ParseCategory (categoryNode);
					
					if (_categories.ContainsKey (category.name)) {
						throw new Exception ("duplicate category: " + category.name);
					}
					
					_categories.Add (category.name, category);
				}
			}
			
			XmlNodeList itemsNodeList = document.SelectNodes ("/ModelRepository/Items/*");
			
			if (itemsNodeList.Count > 0) {
				foreach (XmlNode itemNode in itemsNodeList) {
					Model model = ParseModel (itemNode);
					
					if (_models.ContainsKey (model.Uid ())) {
						throw new Exception ("duplicate model: " + model.Uid ());
					}
					
					if (string.IsNullOrEmpty (model.Category ()) && _strict) {
						throw new Exception ("empty category for model (item: " + model.Uid () + ")");
					}
			
					Category category;
					if (!_categories.TryGetValue (model.Category (), out category)) {
						if (_strict) {
							throw new Exception ("unknown category for model (category: " + model.Category () + ", model: " + model.Uid () + ")");
						}
					} else {
						ValidateModel (category, model);
					}
					
					_models.Add (model.Uid (), model);
					
					List<IModel> categorizedModels;
					if (!_categorizedModels.TryGetValue (model.Category (), out categorizedModels)) {
						categorizedModels = new List<IModel> ();
						_categorizedModels.Add (model.Category (), categorizedModels);
					}
					
					categorizedModels.Add (model);
				}
			}
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool IsInitialized ()
		{
			return _initialized;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string GetRoot ()
		{
			return _root;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool IsStrict ()
		{
			return _strict;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static IModel Get (string uid)
		{
			if (_models.ContainsKey (uid)) {
				return _models [uid];
			}
			return null;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static IResultSet All ()
		{
			return new ResultSet (new List<IModel> (_models.Values));
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static IResultSet List (string category)
		{
			if (_categorizedModels.ContainsKey (category)) {
				return new ResultSet (new List<IModel> (_categorizedModels [category]));
			}
			return NULL_RESULT_SET;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			LoadFromFile ();
			_initialized = true;
		}
		
	}
	
}