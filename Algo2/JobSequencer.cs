using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo2
{
    class Job
    {
        public int Weight;
        public int Length;

        public override string ToString()
        {
            return string.Format("W: {0} Len: {1}", Weight, Length);
        }
    }

    class JobSequencer
    {
        public static List<Job> ReadJobs(string filename)
        {
            var jobs = new List<Job>();

            using (var sr = File.OpenText(filename))
            {
                sr.ReadLine();

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split(' ');

                    jobs.Add(new Job { Weight = int.Parse(parts[0]), Length = int.Parse(parts[1]) });
                }
            }

            return jobs;
        }

        public static Job[] OrderByDiff(IEnumerable<Job> jobs)
        {
            return jobs.OrderByDescending(o => o.Weight - o.Length).ThenByDescending(o => o.Weight).ToArray();
        }

        public static Job[] OrderByRatio(IEnumerable<Job> jobs)
        {
            return jobs.OrderByDescending(o => (double)o.Weight / o.Length).ToArray();
        }

        public static long GetSumCompletionTime(IEnumerable<Job> sortedJobs)
        {
            long sum = 0;
            int currentTime = 0;

            foreach (var job in sortedJobs)
            {
                currentTime += job.Length;

                sum += currentTime * job.Weight;
            }

            return sum;
        }
    }
}
