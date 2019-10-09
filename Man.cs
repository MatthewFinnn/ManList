using System;

namespace ManList
{
	public enum SortMode : byte { byName, byAge, byStatus };
	[Serializable]
	public abstract class Man : IComparable, ICloneable
	{
		protected string name; 
		protected int age;
		private static SortMode sortBy = SortMode.byName, lastSortBy = sortBy;

		static int maxName = 25;
		static int maxAge = 150;

		object ICloneable.Clone()
		{
			Man m = this as Man;
			return m;
		}
		public abstract string Class();
		public int CompareTo(object other)
		{
			Man m = other as Man;
			switch (sortBy) 	// Выбираем ветвь в зависимости от режима сортировки
			{
				case SortMode.byName: return name.CompareTo(m.name);
				case SortMode.byAge: return age.CompareTo(m.age);
				case SortMode.byStatus: 
					if(!Class().Equals(m.Class()))
						return Class().CompareTo(m.Class());
					if (lastSortBy == SortMode.byName)
						return name.CompareTo(m.name);
					return age.CompareTo(m.age);
				default: return 0;
			}
		}
		public Man() { name = "N/A"; age = 0; }	
		public Man(string n, int a) { name = n; age = a; }	
		~Man()	
		{
			name = name.Remove(0, name.Length); 
		}
		public virtual void In()
		{
			Console.Write("Name: ");
			name = Helper.GetName(Console.ReadLine());
			age = Helper.AskInt("Age: ", 0, 100);
		}
		public override string ToString()
		{
			return name.PadRight(maxName) + " ; Age: " + age;
		}
		public virtual void Read(string[] tokens) 
		{
			Name = tokens[1].Trim();
			Helper.MakeInt(tokens[3], 0, maxAge, out age);
		}
		public void Edit()
		{
			Console.WriteLine(this);
			In();
		}
		public override bool Equals(object other)
		{
			Man m = other as Man; return m == null ? false : m.name == name && m.age == age;
		}
		public static string Surname(Man m)	
		{
			return m.name.Substring(m.name.LastIndexOf(' ') + 1);
		}
		public static string Firstname(Man m)
		{
			if (m.name.LastIndexOf(' ') <= 0)
				return m.name;
			else
				return m.name.Substring(0, m.name.IndexOf(' '));
		}
		public static SortMode SortBy
		{
			get { return sortBy; }
			set
			{
				if (value < SortMode.byName || SortMode.byStatus < value )
					throw new ArgumentOutOfRangeException(string.Format("\n\nНеверно задан режим сортировки: {0}\n\n", value));
				if (sortBy != SortMode.byStatus)
					lastSortBy = sortBy;
				sortBy = value;
			}
		}
		public string Name
		{
			get { return name; }
			set
			{
				if (value.Length > maxName)
					name = Helper.GetName(value.Substring(0, maxName - 1));
				else
					name = Helper.GetName(value);
			}
		}
		public int Age
		{
			get { return age; }
			set { Helper.MakeInt(value.ToString(), 0, maxAge, out age); }
		}
		public static int MaxName
		{
			get { return maxName; }
			set { maxName = value; }
		}
		public static int MaxAge
		{
			get { return maxAge; }
			set { maxAge = value; }
		}

	}
}

