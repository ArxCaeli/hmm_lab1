using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using System.Collections;
using System.Collections.Generic;

namespace HmmFramework
{
//State B Transitions:	B = 0.000	St1 = 0.437	St2 = 0.563	E = 0.000
//=================================
//State St1 Transitions:	B = 0.000	St1 = 0.960	St2 = 0.039	E = 0.001
// Emissions	a = 0.139	b = 0.639	c = 0.222
//=================================
//State St2 Transitions:	B = 0.000	St1 = 0.014	St2 = 0.953	E = 0.034
// Emissions	a = 0.264	b = 0.562	c = 0.175
//=================================
//State E
//=================================
//0	St1	c
//1	St2	b
//2	St2	c
//3	St1	b


	public static class InputManager
	{
		static string StatesRegex = @"^State (\S+).*:(\s+(\S+ = \d\.\d+))+";
		static string StatesEmissions = @"^\s*Emissions(\s+(\S+ = \d\.\d*))+";
		static string SequenceRegex = @"^\d+\s\w+\s\S+";

		static public List<HMM> HmmList {
			get;
			set;
		}

		static private HMM LastState;

		static public List<InputLine> Sequence {
			get;
			set;
		}

		public static void ReadFile (string FileName)
		{
			HmmList = new List<HMM>();
			Sequence = new List<InputLine>();
	
			using (StreamReader SR = new StreamReader(FileName)) 
			{
				string FileLine;
				LastState = null;

				Regex StatesReg = new Regex(StatesRegex);
				Regex StatesEm = new Regex(StatesEmissions);
				Regex SeqReg = new Regex(SequenceRegex);

				while((FileLine = SR.ReadLine()) != null)
				{
					Match M = StatesReg.Match(FileLine);
					if (M.Success)
						AddModifyState(M);
				
					M = StatesEm.Match(FileLine);
					if (M.Success)
						AddEmission(M, LastState);

					M = SeqReg.Match(FileLine);
					if (M.Success)
						AddToSequence(FileLine);
				}		

				foreach(HMM H in HmmList)
				{
					Console.WriteLine(H.Name);
					foreach(HmmLink L in H.Links)
						Console.WriteLine("\t" + L.Hmm.Name + " = " + L.Strength.ToString());
					foreach(Emission E in H.Emissions)
						Console.WriteLine("" +
							"Emmision for " + E.EmissionSequence +
						                  " is " + E.EmissionStrength.ToString());
				}

				foreach(InputLine I in Sequence)
						Console.WriteLine(I.SequenceNo.ToString() + " " +
						                  I.StateName + " " + 
						                  I.Sequence);

			}
		}

		static private void AddModifyState (Match M)
		{
			//State St1 Transitions:	B = 0.000	St1 = 0.960	St2 = 0.039	E = 0.001
			LastState = FindOrCreate(HmmList, M.Groups [1].Value);

			foreach (Capture C in M.Groups[3].Captures)
				AddLink(LastState, C.Value);
		}

		static private void AddLink (HMM Hmm, string Link)
		{
			//State = str
			string[] Expr = Link.Split(' ');
			HmmLink NewLink = new HmmLink();
			NewLink.Hmm = FindOrCreate(HmmList, Expr[0]);
			NewLink.Strength = double.Parse(Expr[2]);

			if (Hmm.Links.Exists(x => x.Hmm.Name == NewLink.Hmm.Name))
				throw (new Exception("Redefining value found for: " + Hmm.Name +
				                     "at " + NewLink.Hmm.Name));
			Hmm.Links.Add(NewLink);

		}

		static private HMM FindOrCreate (List<HMM> HList, string Name)
		{
			HMM Hmm = HList.Find (x => x.Name == Name);
			if (Hmm == null)
			{
				Hmm = new HMM (Name);
				HList.Add (Hmm);
			}

			return Hmm;
		}

		static private void AddToSequence(string Line)
		{
			string[] Expr = Line.Split ('\t');
			Sequence.Add(
				new InputLine(Expr[1], int.Parse(Expr[0]), Expr[2]));
		}

		static private void AddEmission (Match M, HMM StateToEdit)
		{
			// Emissions	a = 0.139	b = 0.639	c = 0.222
			foreach(Capture C in M.Groups[2].Captures)
			{
				string[] Expr = C.Value.Split(' ');
				StateToEdit.Emissions.Add(
					new Emission(Expr[0], double.Parse(Expr[2])));
			}
		}
	}

	public class InputLine
	{
		public string StateName {
			get;
			set;
		}

		public int SequenceNo {
			get;
			set;
		}

		public string Sequence {
			get;
			set;
		}

		public InputLine (string NewStateName, int NewSequenceNo, string NewSequence)
		{
			StateName = NewStateName;
			SequenceNo = NewSequenceNo;
			Sequence = NewSequence;
		}
	}
}

