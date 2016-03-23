using Data_Mining.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data_Mining.Functions
{
   public static class Algorithm
    {
        public static List<Cluster> InitializeClusters (string document)
        {

            char[] delimiters = new char[] { '\r', '\n', ' ', '\t' };
            string[] w = document.Split(delimiters,
				     StringSplitOptions.RemoveEmptyEntries);
            int id=0;
            List<Cluster> Clusters = new List<Cluster>();

            foreach (string s in w)
            {
              
                List<string> tmp= new List<string>();
                tmp.Add(s);
                Cluster c = new Cluster(id, s.Length, tmp);
                Clusters.Add(c);
                id++;
               
            }
            return Clusters;

        }

       public static List<Cluster> Cluster(List<Cluster> Clusters, int Constant)
        {
      
           int distance=0;
           while(Clusters.Count!=1)
           {
               for (int i = 0; i < Clusters.Count; i++)
               {
                   int MinLength = Int32.MaxValue;
                   int MinCluster = 0;
                   string str1 = Clusters[i].Contents.First();

                   for (int j = 0; j < Clusters.Count; j++)
                   {
                       if (i != j)
                       {
                           distance = CalculateLevenshtein(Clusters[j].Contents.First(), str1);
                           if (distance < MinLength)
                           {
                               MinLength = distance;
                               MinCluster = j;
                           }
                          

                       }
                   }
                   Clusters[i] = MergeClusters(Clusters[i], Clusters[MinCluster]);
                   Clusters.RemoveAt(MinCluster);
                   //jesli najmniejszy znaleziony dystans miedzy clusterami jest wiekszy niz nasz constant, to przerywamy liczenie.
                   if (MinLength > Constant)
                       return Clusters;
               }
           }

           //Calculate Centroids
            return Clusters; 
        }
       public static Cluster MergeClusters(Cluster c1, Cluster c2)
       {
           foreach ( string t in c2.Contents)
                c1.Contents.Add( t );
           return c1;
       }


       public static List<Cluster> KMeans(List<Cluster> Clusters)
       {
           //Variable is a centroid for each Cluster in Clusters
           int[] centroid = new int[Clusters.Count()];

           List<Cluster> NewClusters = new List<Cluster>();
           List<string> WordContainer = new List<string>();

           FindCentroid(Clusters);
           GiveNewIds(Clusters);
           bool IsTheSame = false;
           int counter = 0;



           while (!IsTheSame)
           {
               counter = 0;
               foreach (var cluster in Clusters)
                   if (cluster.Centroid == cluster.Contents[0])
                   {
                   
                       counter++;
                   }
               if( counter == Clusters.Count())
                   IsTheSame = true;
               if (!IsTheSame)
               {
                   NewClusters = new List<Cluster>();
                   WordContainer = new List<string>();
                   int id = 0;
                   foreach (var cluster in Clusters)
                   {
                       Cluster c = new Cluster(id, cluster.Centroid);
                       id++;
                       NewClusters.Add(c);
                       for (int i = 0; i < cluster.Contents.Count(); i++)
                           if (cluster.Contents[i] != cluster.Centroid)
                               WordContainer.Add(cluster.Contents[i]);
                   }

                   CalculateClosests(NewClusters, WordContainer);
                   Clusters.Clear();
                   Clusters = NewClusters;
                   FindCentroid(Clusters);
               }

           }


           return Clusters;
       }

       public static void CalculateClosests(List<Cluster> Clusters, List<string> Words)
       {
           while (Words.Count() != 0)
           {
               foreach (var c in Clusters)
               {
                   if (Words.Count() != 0)
                   {
                       int MinLen = int.MaxValue;
                       int index = 0;
                       for (int i = 0; i < Words.Count(); i++)
                       {
                           if (MinLen > CalculateLevenshtein(c.Contents[0], Words[i]))
                           {
                               MinLen = CalculateLevenshtein(c.Contents[0], Words[i]);
                               index = i;
                           }

                       }

                       c.AddToCluster(Words[index]);
                       Words.RemoveAt(index);
                   }
               }
           }
       }


       public static void GiveNewIds(List<Cluster>Clusters)
       {
           int i = 1;
           foreach(Cluster c in Clusters)
           {
               c.SetId(i);
               i++;
           }
       }

       public static void FindCentroid(List<Cluster> Clusters)
       {
           int sum = 0;
           int value;
           foreach (var item in Clusters)
           {
               item.CalculateVectorSpace();
               sum = 0;
               for (int i = 0; i < item.Vector.Count(); i++)
               {
                   sum += item.Vector[i];
                   item.mean = sum / item.Vector.Count();
               }

               //Find the word close to the mean value [centroid]
               value = (int)item.mean;
               int original = value;
               bool lastadded = true;
               int addition = 1;

               for (int i = 0; i < Clusters.Count; i++)
               {
                   for (int j = 0; j < Clusters[i].Contents.Count(); j++)
                   {
                       string tmp = Clusters[i].Contents[j];
                       tmp = Regex.Replace(tmp, "[^0-9a-zA-Z]+", "");
                       Clusters[i].Contents[j] = tmp;

                   }

               }
               while (true)
               {
                   int tmp2 = -1;

                   for (int i = 0; i < item.Vector.Count(); i++)
                   {
                       if (item.Vector[i] == value)
                       {
                           tmp2 = i;
                           break;
                       }
                   }
                   if (tmp2 != -1)
                   {
                       item.Centroid = item.Contents.ElementAt(tmp2);
                       break;
                   }
                   if (lastadded)
                   {
                       value = original + addition;
                       lastadded = false;
                   }
                   else
                   {
                       value = original - addition;
                       lastadded = true;
                       addition++;
                   }
               }
           }
       }

       public static int CalculateLevenshtein(string a, string b)
        {
            int n = a.Length;
            int m = b.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
                return m;
            if (m == 0)
                return n;
            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++);
            for (int j = 0; j <= m; d[0, j] = j++);

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
