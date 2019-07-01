using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Batbot{
	class TwitchUser{
		public string id;
		public string displayName;

		public TwitchUser(JObject jObject){
			id = jObject["data"][0]["id"].ToString();
			displayName = jObject["data"][0]["display_name"].ToString();
		}
	}
}