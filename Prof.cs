using System;
using System.Runtime.Serialization.Formatters.Soap;
using System.Drawing;
using System.Windows.Forms;

namespace ManList
{
	[Serializable]
	public class Prof: Man
	{
		protected int pubs;	
		static int maxPubs = 400;

		public int Pubs	
		{
			get { return pubs; }
			set { Helper.MakeInt(value.ToString(), 0, maxPubs, out pubs); }
		}
		public static int MaxPubs
		{
			get { return maxPubs; }
			set { maxPubs = value; }
		}
		public Prof() : base() { pubs = 0; }
		public Prof(string n, int a) : base(n, a) { pubs = 0; }
		public Prof(string n, int a, int c) : base(n, a) { pubs = c; }

		public override string Class() { return "Prof: "; }

		public override void In()
		{
			Console.Write (Class());
			base.In();		// Вызов родительской версии
			pubs = Helper.AskInt ("Pubs: ", 0, maxPubs);
		}

		public override void Read(string[] tokens) 
		{
			base.Read(tokens);
			Helper.MakeInt(tokens[5], 0, maxPubs, out pubs);
		}

		public override string ToString()
		{
			return Class() + base.ToString() + ";  Pubs: " + pubs;
		}

	}
}
