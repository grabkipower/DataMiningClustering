using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Mining.Classes
{
  public  class Cluster
    {
        public int ID { get; set; }
        private int size
        { get; set; }
        public double mean {get; set;}
        public string Centroid{get; set;}
        public int[] Vector;
        public List<string> Contents = new List<string>();

      //hello
        public Cluster(int id, int p, List<string> tmp)
        {
            // TODO: Complete member initialization
            this.ID = id;
            this.size = p;
            this.Contents = tmp;
        }

        public Cluster(int id,string tmp)
        {
            // TODO: Complete member initialization
            this.ID = id;
            this.Contents.Add(tmp);
        }

        public void AddToCluster(string tmp)
        {
            Contents.Add(tmp);
        }
        public string GetContents(int i)
        {
            string tmp = Contents[i];
            return tmp;
        }
      public int GetSize()
        { return size; }
     public void SetId(int _id)
      {
          this.ID = _id;
      }

      public void CalculateVectorSpace()
      {
          this.DeleteRepeations();
          Vector = new int[Contents.Count()];
          int i = 0;
          foreach( var item in Contents)
          {
                  Vector[i] = item.Count();
                  i++;
          }
      }

      public void DeleteRepeations()
      {
          List<string> NewList = new List<string>();

          foreach( var item in Contents)
          {
              if( !NewList.Contains(item))
              {
                  NewList.Add(item);
              }
          }
          Contents.Clear();
          Contents = NewList;
      }
   

    }
     
}
