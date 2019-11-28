using System.Collections.Generic;
using System.Linq;

namespace Batbot{
	public class Utility{
		public static string SanitizeForMarkdown(string text){
			string safeText = text;
			safeText = safeText.Replace("*", "\\*");
			safeText = safeText.Replace("_", "\\_");
			safeText = safeText.Replace("~", "\\~");

			return safeText;
		}

		public static List<List<T>> SplitList<T>(List<T> list, int size){
			return list
				.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / size)
				.Select(x => x.Select(v => v.Value).ToList())
				.ToList();
		}
	}
}