using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JobStanceAnalysis
{
	public class ComparsionRecord
	{

		public ObservableCollection<Job> JobCombos { get; set; }
		private bool bismatch = false;

		public bool bIsMatch
		{
			get => bismatch;
			set => bismatch = value;
		}

		public String MatchType { get; set; }

		public ComparsionRecord()
		{
			JobCombos = new ObservableCollection<Job>();
		}

		public void FindMatches2()
		{
			EStanceType tempLeft = JobCombos[0].Stances[0];
			EStanceType tempMiddle = JobCombos[0].Stances[1];
			EStanceType tempRight = JobCombos[0].Stances[2];
			Job firstjob = JobCombos[0];
			for (int i = 0; i < firstjob.Stances.Length; i++)
			{
				for (int j = 0; j < JobCombos[1].Stances.Length; j++)
				{
					if (firstjob.Stances[i] == JobCombos[1].Stances[j] && firstjob.Stances[i] != EStanceType.NONE)
						bIsMatch = true;
				}
			}
		}

		public void FIndMatches_Generic(int matchnum)
		{
			int comparsioncount = 1;
			EStanceType matchingStanceType;

			//if(i + 1 == JobCombos.Count) continue;
			Job BaseComparison = JobCombos[0];
			for (int j = 1; j < JobCombos.Count; j++)
			{
				Job ToCompare = JobCombos[j];
				for (int x = 0; x < BaseComparison.Stances.Length; x++)
				{
					matchingStanceType = BaseComparison.Stances[x];
					for (int y = 0; y < ToCompare.Stances.Length; y++)
					{
						if (BaseComparison.Stances[x] != EStanceType.NONE && ToCompare.Stances[y] == matchingStanceType)
						{
							comparsioncount++;
							j++;
							x = -1;
							y = -1;

							if (j < JobCombos.Count)
								ToCompare = JobCombos[j];
							else 
								goto Exit;
							break;
						}
						else
						{
							//comparsioncount = 1;
							//if (x + 1 < BaseComparison.Stances.Length)
							//{
							//	x++;
							//	matchingStanceType = BaseComparison.Stances[x];
							//}

						}
					}
				}
			}
			

Exit:

			if (comparsioncount >= matchnum)
				bIsMatch = true;
		}

		public void FIndMatches_Generic_test(int matchnum)
		{
			//We are going to try a different method for this.

			//Take every JOB and add all the stances to a list.
			List<EStanceType> AllstanceCombos = new List<EStanceType>();
			foreach (Job j in JobCombos)
			{
				foreach (EStanceType est in j.Stances)
				{
					if(est != EStanceType.NONE)
						AllstanceCombos.Add(est);
				}
			}
			var resultList = AllstanceCombos.GroupBy(x => x)
				.Where(g => g.Count() == matchnum)
				.SelectMany(g => g)
				.ToList();

			if (resultList.Count > 0)
			{
				resultList = resultList.Distinct().ToList();

				bIsMatch = true;
				MatchType = "MATCH = " +resultList[0].ToString();
				if (resultList.Count == 2)
					MatchType += ", " + resultList[1].ToString();
				if (resultList.Count == 3)
					MatchType += ", " + resultList[2].ToString();
			}

		}


	}
}
