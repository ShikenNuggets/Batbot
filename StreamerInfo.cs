using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batbot{
	[Serializable]
	class StreamerInfo : IComparable{
		public StreamerInfo(string id_, DateTime? lastStream_ = null){
			id = id_;
			lastStream = lastStream_;
		}

		public int CompareTo(object obj){
			if(obj == null){
				return 1;
			}

			if(!(obj is StreamerInfo other)){
				throw new ArgumentException("Object is not of type StreamerInfo!");
			}

			if(!lastStream.HasValue && !other.lastStream.HasValue){
				return 0;
			}else if(!lastStream.HasValue){
				return -1;
			}else if(!other.lastStream.HasValue){
				return 1;
			}

			return lastStream.Value.CompareTo(other.lastStream.Value);
		}

		public string id;
		public DateTime? lastStream;
	}
}
