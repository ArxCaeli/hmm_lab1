using System;
using System.Collections;
using System.Collections.Generic;

namespace HmmFramework
{
	public class HMM
	{
		public List<Emission> Emissions {
			get;
			set;
		}

		public List<HmmLink> Links {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public bool IsFirst {
			get;
			set;
		}

		public bool IsFinal {
			get;
			set;
		}

		public HMM ()
		{
			Links = new List<HmmLink>();
			Emissions = new List<Emission>();
		}

		public HMM (string StateName)
		{
			Links = new List<HmmLink>();
			Emissions = new List<Emission>();
			Name = StateName;
		}

		public double GetEmmisionStrength(string Sequence)
		{
			if (IsFinal)
				return 0;
			if (Sequence == "")
				return 0;
			return Emissions.Find(x => x.EmissionSequence == Sequence).EmissionStrength;
		}

		public double GetTransitionStrength(string StateName)
		{
			return Links.Find(x => x.Hmm.Name == StateName).Strength;
		}
	}

	public class Emission
	{
		public string EmissionSequence {
			get;
			set;
		}

		public double EmissionStrength {
			get;
			set;
		}

		public Emission (string Sequence, double Strength)
		{
			EmissionSequence = Sequence;
			EmissionStrength = Strength;
		}
	}

	public class HmmLink
	{
		public HMM Hmm {
			get;
			set;
		}

		public double Strength {
			get;
			set;
		}

		public HmmLink ()
		{
			Hmm = null;
			Strength = 0;
		}
	}
}

