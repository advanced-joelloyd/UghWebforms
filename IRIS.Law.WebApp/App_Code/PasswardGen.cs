using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IRIS.Law.WebApp.App_Code
{
    public class PasswardGen
    {
		protected System.Random rGen;
		protected  string[] strCharacters = { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
												"1","2","3","4","5","6","7","8","9",
												"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
		protected  string[] strNumbers = {"1","2","3","4","5","6","7","8","9","0"};
        public PasswardGen()
		{
			rGen = new Random();
		}
		/// <summary>
		/// Makes lower/uppercase password
		/// </summary>
		/// <param name="i">How many Characters?</param>
		/// <returns>The generated password</returns>
		
		public string GenPassWithNumbers(int i)
		{
			int p = 0;
			string strPass = "";
			for (int x = 0; x< i; x++)
			{
				p = rGen.Next(0,10);
				strPass += strNumbers[p];
			}
			return strPass;
			
		}
		public string GenPassWithCap(int i)
		{
			int p = 0;
			string strPass = "";
			for (int x = 0; x< i; x++)
			{
				p = rGen.Next(0,61);
				strPass += strCharacters[p];
			}

			return strPass.ToUpper();
		}

		/// <summary>
		/// Makes only lowercase password
		/// </summary>
		/// <param name="i">How many Characters?</param>
		/// <returns>The generated password</returns>
		public string GenPassLowercase(int i)
		{
			int p = 0;
			string strPass = "";
			for (int x = 0; x< i; x++)
			{
				p = rGen.Next(0,35);
				strPass += strCharacters[p];
			}

			return strPass.ToLower();
		}
    }
}
