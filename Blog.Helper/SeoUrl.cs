using System.Text.RegularExpressions;

namespace Blog.Helper;
public static class SeoUrl {
	public static string Url(string name) {
		name = name.Replace("ı", "i")
			   .Replace("ğ", "g")
			   .Replace("ü", "u")
			   .Replace("ş", "s")
			   .Replace("ö", "o")
			   .Replace("ç", "c")
			   .Replace("İ", "I")
			   .Replace("Ğ", "G")
			   .Replace("Ü", "U")
			   .Replace("Ş", "S")
			   .Replace("Ö", "O")
			   .Replace("Ç", "C");

		name = name.Replace(" ", "-")
				   .Replace("(", "-")
				   .Replace(")", "-")
				   .Replace("%", "-");

		name = Regex.Replace(name, @"[^a-zA-Z0-9\-]", "");
		name = name.ToLower();
		name = name.Trim('-');

		return name;
	}
}
