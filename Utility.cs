namespace Batbot{
	public class Utility{
		public static string SanitizeForMarkdown(string text){
			string safeText = text;
			safeText = safeText.Replace("*", "\\*");
			safeText = safeText.Replace("_", "\\_");
			safeText = safeText.Replace("~", "\\~");

			return safeText;
		}
	}
}