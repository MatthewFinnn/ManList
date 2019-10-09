using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ManList
{
	public static class Helper
	{
		public static int AskInt(string prompt, int min, int max)
		{
			int res = min;		// Пессимистический прогноз
			for (bool ok = false; !ok; )
			{
				Console.Write(prompt);
				ok = MakeInt(Console.ReadLine(), min, max, out res);
			}
			return res;
		}

		public static bool MakeInt(string s, int min, int max, out int res)
		{
			try
			{
				res = int.Parse(s);	// Эта функция может выдать исключение
				if (res < min || max < res)
				{
					Console.WriteLine("Value must be in ({0}, {1})", min, max);
					res = min;
					return false;
				}
			}
			catch	// Ловим все типы исключений
			{
				Console.WriteLine("It is not valid integer");
				res = min;
				return false;
			}
			return true;
		}

		public static string GetName(string text)
		{
			if (text == null)
				return "No Name";
			Regex reg = new Regex("[А-Я|а-я|A-Z|a-z]+[ ]*");
			string[] captures = { "", "", "" };
			MatchCollection mc = reg.Matches(text);

			int count = 0;
			foreach (Match m in mc)
			{
				foreach (Group g in m.Groups)
				{
					foreach (Capture c in g.Captures)
					{
						if (count < 3)
							captures[count++] = c.Value.Trim().ToLower();
					}
				}
			}

			string result = "";
			foreach (string s in captures)
			{
				StringBuilder word = new StringBuilder(s);
				if (word.ToString() == "")
					break;
				word[0] = char.ToUpper(word[0]);
				result += word + " ";
			}
			return string.IsNullOrEmpty(result) ? "No Name" : result.Trim();
		}

	}
}
