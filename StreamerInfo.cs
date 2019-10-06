using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batbot{
	[Serializable]
	class StreamerInfo{
		public StreamerInfo(string id_, DateTime? lastStream_ = null){
			id = id_;
			lastStream = lastStream_;
		}

		public string id;
		public DateTime? lastStream;
	}
}
