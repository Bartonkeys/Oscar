using System;
namespace Oscar.Core.Enums
{
	public class Enum<T> where T : Enum
	{
		public static IEnumerable<T> GetAllValuesAsIEnumerable()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

	}
}

