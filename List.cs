using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Collections;

namespace ManList
{
	public class List
	{
		ArrayList men;		// Список ссылок на объекты
		bool bModified,	// Флаг состояния: список изменен
				bNew;			// Флаг состояния: новый список
		string version,	// Строка аутентификации файла
					fileName;	// Имя файла со списком

		[XmlElement(typeof(Stud)), XmlElement(typeof(Prof))]
        public ArrayList Men { get { return men; } }

		public List()
		{
			men = new ArrayList();
			bModified = false;
			bNew = true;
			version = "List of Man, v.1.0";
			fileName = FindFolder("Data") + "List.txt"; // Определяем местоположение файлов с данными
		}

		string FindFolder(string name)
		{
			string dir = Application.StartupPath;
			for (char slash = '\\'; dir != null; dir = Path.GetDirectoryName(dir))
			{
				string res = dir.TrimEnd(slash) + slash + name;
				if (Directory.Exists(res))
					return res + slash;
			}
			return null;
		}

		public void Show()
		{
			Console.Write((men.Count == 0 ? "\n\tArrayList is Empty\n" : "\n\tArrayList of Man\n"));
			int i = 0;
			foreach (Man m in men)
			{
				i++;
				Console.Write(i + " " + m.ToString() + "\n");
			}
		}

		public void Add()
		{
			while (true) // Пусть пользователь вводит, пока ему не надоест	
			{
				Man m = null;				
				switch (ManMenu())
				{
					case 'q': return;
					case 's': m = new Stud(); break;
					case 'p': m = new Prof(); break;
					default: continue;
				}
				m.In();
				men.Add(m);
				bModified = true;	// Список изменился
			}
		}

		public void Delete()
		{
			if (men.Count == 0)
			{
				Console.WriteLine("ArrayList is empty");
				return;
			}
			while (men.Count > 0)
			{
				Console.Write("\n\tDelete\t0...{0} (0 - to Quit): ", men.Count);
				int n = Helper.AskInt("Enter number of del object", 0, men.Count);
				if (n <= 0)
					break;
				men.RemoveAt(n - 1);
				bModified = true;
				Show();
			}
		}

		public object FileOpen(string mode)
		{
			object file = null;
			switch (mode)
			{
				case "read":
					try
					{

						file = new StreamReader(fileName, false);
					}
					catch (FileNotFoundException)
					{
						Console.WriteLine("\nFile: " + fileName + " not found");
						return null;
					}
					StreamReader sr = file as StreamReader;
					string line = sr.ReadLine();
					if (!line.Equals(version))
					{
						Console.WriteLine("\nWrong file format: " + fileName);
						sr.Close();
						return null;
					}
					return file;
				case "write":
					try
					{
						file = new StreamWriter(fileName, false);
					}
					catch (IOException)
					{
						Console.WriteLine("\nCan not write to: " + fileName);
						return null;
					}
					StreamWriter sw = file as StreamWriter;
					sw.WriteLine(version + "\r\n");
					return file;
				default: return null;
			}
		}

		public void Write()
		{
			if (men.Count == 0)
			{
				Console.WriteLine("ArrayList is empty");
				return;
			}
			WriteText();
			WriteBinary();
			WriteXml();
			Console.WriteLine("ArrayList has been stored in all formats");
			bNew = bModified = false;
		}

		void WriteText()
		{
			fileName = Path.ChangeExtension(fileName, ".txt");
			StreamWriter sw = FileOpen("write") as StreamWriter;
			if (sw != null)
			{
			foreach (Man m in men)
			sw.WriteLine (m);
			}
			sw.Close ();
		}

		void WriteBinary()
		{
			fileName = Path.ChangeExtension(fileName, ".bin");
			IFormatter fmt = new BinaryFormatter();
			Stream stream = new FileStream(fileName, FileMode.Create);
			fmt.Serialize(stream, men);
			stream.Close();

		}

		void WriteXml()
		{
			fileName = Path.ChangeExtension(fileName, ".xml");
			XmlSerializer serializer = new XmlSerializer(typeof(List));
			TextWriter sw = new StreamWriter(fileName);
			serializer.Serialize(sw, this);
			sw.Close();
		}

		public void Read()
		{
			if (men.Count != 0)
			{
				Console.WriteLine("Deleting current ArrayList");
				men.Clear();
			}

			switch (ReadWriteMenu())
			{
				default: return;
				case 't': ReadText(); break;
				case 'b': ReadBinary(); break;
				case 'x': ReadXml(); break;
			}
			bModified = false;
			Show();
		}

		void ReadText()
		{
			fileName = Path.ChangeExtension(fileName, ".txt");
			StreamReader sr = FileOpen("read") as StreamReader; 
			if (sr == null)
			{
				Console.WriteLine("Could not open: " + fileName);
				return;
			}
			Console.WriteLine("Reading from: " + Path.GetFileName(fileName));
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				if (line == "")
					continue;
				Man m;
				switch (line[0])
				{
					case 'S': m = new Stud(); break;
					case 'P': m = new Prof(); break;
					default: return;
				}
				char[] separator = { ':', ';' };
				string[] tokens = line.Split(separator); 
				m.Read (tokens); 
				men.Add(m);
			}
			sr.Close();
		}

		void ReadBinary()
		{
			fileName = Path.ChangeExtension(fileName, ".bin");
			men.Clear();	
			IFormatter fmt = new BinaryFormatter();
			Stream stream = new FileStream(fileName, FileMode.Open);
			men = (ArrayList)fmt.Deserialize(stream);
			stream.Close();

		}

		void ReadXml()
		{
			fileName = Path.ChangeExtension(fileName, ".xml");
			XmlSerializer serializer = new XmlSerializer(typeof(List));
			TextReader sw = new StreamReader(fileName);
			men = (serializer.Deserialize(sw) as List).men;
			sw.Close();
		}

		public void Quit()
		{
			if (bModified)
			{
				Console.Write("\n\t\tArrayList has been changed. Save (y/n)?  ");

				string s = Console.ReadLine();
				if (s.Equals("Y") || s.Equals("y"))
					Write();
			}
		}

		public void Sort()
		{
			if (men.Count == 0)
			{
				Console.WriteLine("\n\tArrayList is empty");
				return;
			}
			switch (SortMenu())		// Установите режим сортировки
			{
				default: return;
				case 'n': Man.SortBy = SortMode.byName; break;
				case 'a': Man.SortBy = SortMode.byAge; break;
				case 's': Man.SortBy = SortMode.byStatus; break;
			}
			men.Sort();
			Show();
		}

		public void Work()
		{
			Console.WriteLine("\n\t\t\tArrayList");
			while (true)
			{
				switch (Menu())
				{
					case 'q': Quit(); return;
					case 's': Show(); break;
					case 'a': Add(); break;
					case 'd': Delete(); break;
					case 'e': Edit(); break;
					case 't': Sort(); break;
					case 'r': Read(); break;
					case 'w': Write(); break;
					case 'h': Handle(); break;
				}
			}
		}

		public void Edit()
		{
			if (men.Count == 0)
			{
				Console.WriteLine("ArrayList is empty");
				return;
			}
			while (men.Count > 0)
			{
				Console.Write("\n\tEdit\t0...{0}  (0 - to Quit): ", men.Count);
				int n = Helper.AskInt("", 0, men.Count);
				if (n == 0)
					break;
				Console.Write("\nEdit ");
				this[n - 1].Edit();			// Здесь работает индексатор
				bModified = true;
				Show();
			}
		}

		public char Menu()
		{
			Console.Write("\n" +
				"\n\t\tq - Quit\tt - Sort" +
				"\n\t\ts - Show\tr - Read" +
				"\n\t\ta - Add \tw - Write" +
				"\n\t\td - Delete\te - Edit" +
				"\n\t\th - Handle\n\n");
            try{return Console.ReadLine().ToLower()[0];}
            catch (Exception){return '-';}
			
		}

		public char SortMenu()
		{
			Console.Write("\n\t\tSorting\n" +
				"\n\t\tn - By Name" +
				"\n\t\ta - By Age" +
				"\n\t\ts - By Status" +
				"\n\t\tq - Quit\n\n");
			return Console.ReadLine().ToLower()[0];
		}

		public char ManMenu()
		{
			Console.Write(
				"\n\t\ts - Student" +
				"\n\t\tp - Professor" +
				"\n\t\tq - Quit\n\n");
			return Console.ReadLine().ToLower()[0];
		}

		public char ShowMenu()
		{
			Console.Write(
				"\n\t\tf - First Names" +
				"\n\t\ts - Surnames" +
				"\n\t\tn - Normal" +
				"\n\t\tq - Don't show\n\n");
			return Console.ReadLine().ToLower()[0];
		}

		public char ReadWriteMenu()
		{
			Console.Write("\n" +
				"\n\t\tq - Quit" +
				"\n\t\tt - Text" +
				"\n\t\tb - Binary" +
				"\n\t\tx - XML\n\n");
			return Console.ReadLine().ToLower()[0];
		}

		void Handle()
		{
			if (men.Count == 0)
			{
				Console.WriteLine("ArrayList is empty");
				return;
			}
			switch (ShowMenu())
			{
				case 'q': return;
				case 'f':
					HandleMen("\n\tArrayList of Man Names:", new Func<Man, string>(Man.Firstname));
					break;
				case 's':
					HandleMen("\n\tArrayList of Man Surnames:", new Func<Man, string>(Man.Surname));
					break;
				case 'n': Show(); break;
			}
		}

		void HandleMen(string title, Func<Man, string> func)
		{
			Console.WriteLine(men.Count == 0 ? "\n\tArrayList is empty\n" : title);
			for (int i = 0; i < men.Count; i++)
				Console.WriteLine("  {0}. {1}", i + 1, func((Man)men[i]));
		}

		public Man this[int id]
		{
			get
			{
				if (id < 0 || men.Count <= id)
					throw new ArgumentOutOfRangeException("Man.Indexer: Wrong id " + id);
				return (Man)men[id];
			}
			set
			{
				if (men.Count <= id || id < 0)
					throw new ArgumentOutOfRangeException("Man.Indexer: Wrong id " + id);
				men[id] = value;
			}
		}
	}

}
