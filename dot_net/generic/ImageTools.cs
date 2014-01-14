/***
* This is used to manipulate images without http://imageresizing.net/
* Most other functions could make use of http://imageresizing.net/, it's easy and good
***/

using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Net;
using System.Drawing.Imaging;
using System.Collections.Specialized;

public class ImageTools
{
  public ImageTools()
  {
    //
    // TODO: Add constructor logic here
    //
  }
  
  public static Stream ToStream(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat formaw)
  {
    var stream = new System.IO.MemoryStream();
    image.Save(stream, formaw);
    stream.Position = 0;
    return stream;
  }

  public static System.Drawing.Image drawText(System.Drawing.Image imgBackground, String strText, RectangleF objTextRectangle, 
    StringFormat format, Font objFont, Color objColor)
  {
    var targetImageCloned = imgBackground.Clone() as Image;

    // Create a graphics object to measure the text's width and height.
    Graphics objGraphics = Graphics.FromImage(targetImageCloned);

    // int width of box
    int intBoxWidth = (int)Math.Ceiling(objTextRectangle.Width);

    float fltTextWidth = objGraphics.MeasureString(strText, objFont).Width;
    float fltTextHeight = objGraphics.MeasureString(strText, objFont).Height;

    if (objTextRectangle.Width == 0)
    {
      objTextRectangle.Width = fltTextWidth;

      if (fltTextWidth > intBoxWidth)
      {
        objTextRectangle.Width = intBoxWidth;
      }
      if (intBoxWidth < fltTextWidth || fltTextWidth == 0)
      {
        intBoxWidth = targetImageCloned.Width - (int)Math.Ceiling(objTextRectangle.X);
      }
    }
    if (objTextRectangle.Height == 0)
    {
      objTextRectangle.Height = fltTextHeight;
    }

    int intLines = Convert.ToInt32(Math.Ceiling(fltTextWidth / intBoxWidth));

    // Write text on image
    string[] arySeperators = new string[] { "\r\n", " " };
    string[] aryWords = strText.Split(arySeperators, StringSplitOptions.RemoveEmptyEntries);

    string strLineWords = "";
    foreach (string strWord in aryWords)
    {
      double dblLineWidth = objGraphics.MeasureString(strLineWords, objFont).Width;
      double dblCurrentWordWidth = objGraphics.MeasureString(strWord + " ", objFont).Width;

      if (dblLineWidth + dblCurrentWordWidth >= intBoxWidth)
      {
        objGraphics.DrawString(strLineWords,
                  objFont, new SolidBrush(objColor),
                  objTextRectangle,
                  format);
        objTextRectangle.Y = (float)Convert.ToDouble(objTextRectangle.Y + fltTextHeight);
        strLineWords = "";
        strLineWords = strLineWords + strWord + " ";
      }
      else
      {
        strLineWords = strLineWords + strWord + " ";
      }
    }

    if (!String.IsNullOrEmpty(strLineWords))
    {
      // last line
      objGraphics.DrawString(strLineWords,
                objFont, new SolidBrush(objColor),
                objTextRectangle,
                format);
    }

    /*
    objGraphics.DrawString(strText, objFont, new SolidBrush(objColor), objStartPoint.Left, objStartPoint.Top);
    objGraphics.Flush();
    */
    return targetImageCloned;
  }

  // Taken from http://www.bobpowell.net/grayscale.htm
  public static System.Drawing.Image convertToGreyScale(System.Drawing.Image imgToConvert)
  {
    Bitmap bm = new Bitmap(imgToConvert.Width, imgToConvert.Height, imgToConvert.PixelFormat);
    Graphics g = Graphics.FromImage(bm);

    //Gilles Khouzams colour corrected grayscale shear
    ColorMatrix cm = new ColorMatrix(new float[][]{   new float[]{0.3f,0.3f,0.3f,0,0},
                              new float[]{0.59f,0.59f,0.59f,0,0},
                              new float[]{0.11f,0.11f,0.11f,0,0},
                              new float[]{0,0,0,1,0,0},
                              new float[]{0,0,0,0,1,0},
                              new float[]{0,0,0,0,0,1}});

    ImageAttributes ia = new ImageAttributes();
    ia.SetColorMatrix(cm);
    g.DrawImage(imgToConvert, new Rectangle(0, 0, imgToConvert.Width, imgToConvert.Height), 0, 0, imgToConvert.Width, imgToConvert.Height, GraphicsUnit.Pixel, ia);
    g.Dispose();

    return (System.Drawing.Image)(bm);
  }

  public static System.Drawing.Image OverlayImage(System.Drawing.Image imgBackground, System.Drawing.Image imgOverlay)
  {
    Bitmap bmBackground = new Bitmap(imgBackground);
    Bitmap bmOverlay = new Bitmap(imgOverlay);
    var target = new Bitmap(bmBackground.Width, bmBackground.Height, PixelFormat.Format32bppArgb);
    var graphics = Graphics.FromImage(target);
    graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear

    graphics.DrawImage(bmBackground, 0, 0);
    graphics.DrawImage(bmOverlay, 0, 0);

    graphics.Dispose();

    return (System.Drawing.Image)(target);
  }

  public static System.Drawing.Image SitchImageLeftRight(System.Drawing.Image imgLeft, System.Drawing.Image imgRight)
  {
    int intResultWidth = imgLeft.Width + imgRight.Width;
    int intResultHeight = (imgLeft.Height > imgRight.Height ? imgLeft.Height : imgRight.Height);

    Bitmap bmResult = new Bitmap(intResultWidth, intResultHeight, imgLeft.PixelFormat);

    Graphics g = Graphics.FromImage(bmResult);
    g.DrawImage(imgLeft, (int)0, (int)0, (int)imgLeft.Width, (int)imgLeft.Height);
    g.DrawImage(imgRight, (int)imgLeft.Width, (int)0, (int)imgRight.Width, (int)imgRight.Height);

    return (System.Drawing.Image)(bmResult);
  }

  public static System.Drawing.Image SitchImageTopBottom(System.Drawing.Image imgTop, System.Drawing.Image imgBottom)
  {
    int intResultWidth = (imgTop.Width > imgBottom.Width ? imgTop.Width : imgBottom.Width);
    int intResultHeight = imgTop.Height + imgBottom.Height;

    Bitmap bmResult = new Bitmap(intResultWidth, intResultHeight, imgTop.PixelFormat);

    Graphics g = Graphics.FromImage(bmResult);
    g.DrawImage(imgTop, (int)0, (int)0, (int)imgTop.Width, (int)imgTop.Height);
    g.DrawImage(imgBottom, (int)0, (int)imgTop.Height, (int)imgTop.Width, (int)imgTop.Height);

    return (System.Drawing.Image)(bmResult);
  }

  /// <summary>  
  /// method for changing the opacity of an image  
  /// </summary>  
  /// <param name="image">image to set opacity on</param>  
  /// <param name="opacity">percentage of opacity, 0.0 to 1.0</param>  
  /// <returns></returns>  
  public static System.Drawing.Image SetImageOpacity(System.Drawing.Image image, float opacity)
  {
    try
    {
      //create a Bitmap the size of the image provided  
      Bitmap bmp = new Bitmap(image.Width, image.Height);

      //create a graphics object from the image  
      using (Graphics gfx = Graphics.FromImage(bmp))
      {

        //create a color matrix object  
        ColorMatrix matrix = new ColorMatrix();

        //set the opacity  
        matrix.Matrix33 = opacity;

        //create image attributes  
        ImageAttributes attributes = new ImageAttributes();

        //set the color(opacity) of the image  
        attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        //now draw the image  
        gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
      }
      return bmp;
    }
    catch (Exception ex)
    {
      return null;
    }
  }
}