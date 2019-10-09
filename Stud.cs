using System;
using System.Runtime.Serialization.Formatters.Soap;
using System.Drawing;
using System.Windows.Forms;

namespace ManList
{
	[Serializable]
	public class Stud: Man
	{
		protected int course;	
		static int maxCourse = 6;

		public override string Class() { return "Stud: "; }

		public int Course
		{
			get { return course; }
			set 
			{ 
				Helper.MakeInt(value.ToString(), 0, maxCourse, out course); 
			}
		}

		public Stud() : base() { course = 0; }
		public Stud(string n, int a) : base(n, a) {course = 0; }
		public Stud(string n, int a, int c) : base(n, a) { course = c; }

		public override void In()
		{
			Console.Write (Class());
			base.In();		
			course = Helper.AskInt ("Course: ", 0, maxCourse);
		}

		public override void Read(string[] tokens)
		{
			base.Read(tokens);
			Helper.MakeInt(tokens[5], 0, maxCourse, out course);
		}

		public override string ToString()
		{
			return Class() + base.ToString() + ";  Course: " + course;
		}

	}
}
