using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace AssemblyCSharp
{
	//New type for Dictionary indexing, would've used Tuple<Guid, Guid> instead if Unity supported it
	public class guid2
	{
		public Guid x;
		public Guid y;
		
		public guid2(Guid t1, Guid t2)
		{
			x = t1;
			y = t2;
		}
		//Rough guess on a way to handle the hashcode check used for .disctinct()
		public override int GetHashCode ()
		{
			return x.GetHashCode() & y.GetHashCode();
		}
	}
	//Allow us to compare the new guid2 type
	public class guid2Comparer : IEqualityComparer<guid2>
	{
		public bool Equals(guid2 t1, guid2 t2)
		{
			return t2.x.Equals (t1.x) && t2.y.Equals (t1.y);
		}
		
		public int GetHashCode(guid2 t1)
		{
			return t1.GetHashCode();
		}
	}
}
