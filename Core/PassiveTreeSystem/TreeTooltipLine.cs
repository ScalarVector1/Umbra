using Humanizer;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Umbra.Core.PassiveTreeSystem
{
	internal class TreeTooltipLine : IComparable<TreeTooltipLine>
	{
		public static Regex numeralRegex = new("\\[c\\/AAAAFF\\:(.*?)\\]");
		public static Regex rawNumberRegex = new("[0-9\\-\\.]+");

		public string text;

		public float value = 0;
		public List<float> values = [];
		public List<string> formats = [];

		public string displayedText = "";

		public static string CalculateKey(string raw)
		{
			return rawNumberRegex.Replace(raw, "#");
		}

		public TreeTooltipLine(string text)
		{
			MatchCollection match = numeralRegex.Matches(text);
			this.text = text;

			for (int k = 0; k < match.Count; k++)
			{
				Match m = match[k];

				int index = this.text.IndexOf(m.Value);
				this.text = this.text.Remove(index, m.Value.Length);
				this.text = this.text.Insert(index, $"{{{k}}}");

				Match rawNumber = rawNumberRegex.Match(m.Value);
				values.Add(float.Parse(rawNumber.Value));
				formats.Add(rawNumberRegex.Replace(m.Value, "{0}"));
			}

			if (values.Count > 0)
				value = values.Max();
			else
				value = 0;
		}

		public void AddValuesFromString(string text)
		{
			MatchCollection match = numeralRegex.Matches(text);

			if (match.Count != values.Count)
				throw new ArgumentException("Provided string does not match the format for this Tree Tooltip Line");

			for (int k = 0; k < match.Count; k++)
			{
				Match rawNumber = rawNumberRegex.Match(match[k].Value);

				if (formats[k].Contains("x"))
					values[k] *= float.Parse(rawNumber.Value);

				else
					values[k] += float.Parse(rawNumber.Value);
			}

			if (values.Count > 0)
				value = values.Max();
			else
				value = 0;
		}

		public void CalculateDisplayText()
		{
			if (values.Count == 0)
			{
				displayedText = text;
				return;
			}

			List<string> formatted = [];

			for (int k = 0; k < formats.Count; k++)
			{
				formatted.Add(formats[k].FormatWith(Math.Round(values[k], 2)));
			}

			displayedText = text.FormatWith(formatted.ToArray());
		}

		public int CompareTo(TreeTooltipLine other)
		{
			return -1 * value.CompareTo(other.value);
		}
	}

	internal class TreeTooltipCollection
	{
		public Dictionary<string, TreeTooltipLine> lines = [];
		public List<TreeTooltipLine> ordered = [];

		public void AddFromPassive(Passive passive)
		{
			string[] passiveLines = passive.Tooltip.Split("\n");

			foreach (string line in passiveLines.Where(n => !string.IsNullOrEmpty(n)))
			{
				string key = TreeTooltipLine.CalculateKey(line);

				if (lines.ContainsKey(key))
				{
					lines[key].AddValuesFromString(line);
				}
				else
				{
					var newLine = new TreeTooltipLine(line);
					lines.Add(key, newLine);
					ordered.Add(newLine);
				}
			}
		}

		public void PrepareForDisplay()
		{
			ordered.Sort();

			foreach (TreeTooltipLine line in ordered)
			{
				line.CalculateDisplayText();
			}
		}

		public void Demo()
		{
			foreach (TreeTooltipLine line in ordered)
			{
				Main.NewText(line.displayedText);
			}
		}

		public void Clear()
		{
			lines.Clear();
			ordered.Clear();
		}
	}
}
