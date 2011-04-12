using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Controller.Utilities
{
	public static class ImageUtilities
	{
		//http://social.msdn.microsoft.com/forums/en-US/wpf/thread/0f037b9c-779d-45ad-b797-01c25999491b
		public static BitmapImage ImageFromGDIPlus(byte[] bytes)
		{
			var guid = Guid.NewGuid();
			if (Directory.Exists(@"C:\Temp\") == false)
				Directory.CreateDirectory(@"C:\Temp\");

			var stream = new MemoryStream(bytes);
			var badMetadataImage = new Bitmap(stream);
			ImageCodecInfo myImageCodecInfo;
			Encoder myEncoder;
			EncoderParameter myEncoderParameter;
			EncoderParameters myEncoderParameters;
			// get an ImageCodecInfo object that represents the JPEG codec
			myImageCodecInfo = GetEncoderInfo("image/jpeg");
			// Create an Encoder object based on the GUID for the Quality parameter category
			myEncoder = Encoder.Quality;
			// Create an EncoderParameters object
			// An EncoderParameters object has an array of EncoderParameter objects.
			// In this case, there is only one EncoderParameter object in the array.
			myEncoderParameters = new EncoderParameters(1);
			// Save the image as a JPEG file with quality level 75.
			myEncoderParameter = new EncoderParameter(myEncoder, 75L);
			myEncoderParameters.Param[0] = myEncoderParameter;
			badMetadataImage.Save(@"C:\Temp\" + guid + ".bmp", myImageCodecInfo, myEncoderParameters);
			// Create the source to use as the tb source
			var bi = new BitmapImage();
			bi.BeginInit();
			bi.UriSource = new Uri(@"C:\Temp\" + guid + ".bmp", UriKind.Absolute);
			bi.EndInit();
			return bi;
		}

		public static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;

			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
					return encoders[j];
			}

			return null;
		}

		public static byte[] BufferFromImage(BitmapImage imageSource)
		{
			var stream = imageSource.StreamSource;
			byte[] buffer = null;
			if (stream != null && stream.Length > 0)
			{
				stream.Position = 0;
				using (var br = new BinaryReader(stream))
				{
					buffer = br.ReadBytes((Int32)stream.Length);
				}
			}

			return buffer;
		}

		public static BitmapImage ImageFromBuffer(byte[] bytes)
		{
			var stream = new MemoryStream(bytes);
			var image = new BitmapImage();
			image.BeginInit();
			image.StreamSource = stream;
			image.EndInit();
			return image;
		}
	}
}