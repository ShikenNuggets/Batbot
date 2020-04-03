using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Batbot
{
	class OAuthToken{
		string token = null;
		DateTime expiration = DateTime.Now;

		public OAuthToken(JObject obj = null){
			if(obj == null){
				Init(null, 0);
			}else{
				string token = obj["access_token"].ToString();
				int expiresIn = obj["expires_in"].Value<int>();
				Init(token, expiresIn);
			}
		}

		private void Init(string token_, int timeRemaining){
			token = token_;
			expiration = DateTime.Now + new TimeSpan(0, 0, timeRemaining);
		}

		public bool IsValid(){
			if(token == null || expiration < DateTime.Now){
				return false;
			}

			return true;
		}

		public double SecondsRemaining(){
			if(!IsValid()){
				return 0;
			}

			return (DateTime.Now - expiration).TotalSeconds;
		}

		public override string ToString(){
			return token;
		}
	}
}