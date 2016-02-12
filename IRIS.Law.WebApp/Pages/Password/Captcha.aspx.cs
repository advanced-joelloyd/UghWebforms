using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace IRIS.Law.WebApp.Pages.Password
{
    public partial class Captcha : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Drawing.Bitmap objBmp = new System.Drawing.Bitmap(170, 45); 
            System.Drawing.Graphics objGraphics = System.Drawing.Graphics.FromImage(objBmp); 
            objGraphics.Clear(System.Drawing.Color.Gray); 
            objGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            System.Drawing.Font objFont = new System.Drawing.Font("Kristen ITC", 20, System.Drawing.FontStyle.Bold); 
            string strRandom = "";
            string strRandomDisplay = "";
            string[] strArray = new string[36]; 
            strArray = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }; 
            Random autoRand = new Random(); 
            
            int x; 
            
            for (x = 0; x < 6; x++) 
            { 
                int i = Convert.ToInt32(autoRand.Next(0, 36)); 
                strRandom += strArray[i].ToString();
                strRandomDisplay += strArray[i].ToString() + " "; 
            } 
            
            Session.Add("strRandom", strRandom);
            objGraphics.DrawString(strRandomDisplay, objFont, System.Drawing.Brushes.Black, 3, 3);
            Response.ContentType = "image/GIF"; 
            objBmp.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif); 
            objFont.Dispose(); 
            objGraphics.Dispose(); 
            objBmp.Dispose();

            

        }
    }
}
