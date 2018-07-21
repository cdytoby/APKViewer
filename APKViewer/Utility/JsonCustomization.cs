using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace APKViewer.Utility
{
	public static class JsonCustomization
	{
		//https://stackoverflow.com/questions/23830206/json-convert-empty-string-instead-of-null

		public class NullToEmptyStringResolver : DefaultContractResolver
		{
			protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
			{
				return type.GetProperties().Select(
					p =>
					{
						JsonProperty jp = base.CreateProperty(p, memberSerialization);
						jp.ValueProvider = new NullToEmptyStringValueProvider(p);
						return jp;
					}).ToList();
			}
		}

		public class NullToEmptyStringValueProvider : IValueProvider
		{
			PropertyInfo _MemberInfo;
			public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
			{
				_MemberInfo = memberInfo;
			}

			public object GetValue(object target)
			{
				object result = _MemberInfo.GetValue(target);
				if (_MemberInfo.PropertyType == typeof(string) && result == null)
					result = "";
				return result;

			}

			public void SetValue(object target, object value)
			{
				_MemberInfo.SetValue(target, value);
			}
		}
	}
}
