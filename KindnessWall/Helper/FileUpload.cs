using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace KindnessWall.Helper
{
    public class ImageModel
    {
        public string Src { get; set; }
        public List<Size> Sizes { get; set; }
        public bool Resize { get; set; }
    }

    public class FileUpload
    {

        public static string ImagePreviewPrefix = "size(preview)_";
        public static string ImageResizedSymbol = "size(";

        public FileUpload()
        {
            RiseException = false;
        }
        public FileUpload(bool _riseException)
        {
            RiseException = _riseException;
        }


        public string Message
        {
            get;
            private set;

        }
        public bool RiseException { get; set; }




        public Size GetImageDimensions(Size Image, Size Region)
        {
            Size Resize = new Size();
            Resize.Height = Region.Width * Image.Height / Image.Width;
            Resize.Width = Region.Height * Image.Width / Image.Height;

            if (Image.Width > Image.Height)
            {
                if (Region.Height < Resize.Height)
                {
                    Resize.Height = Region.Height;
                    Resize.Width = Region.Height * Image.Width / Image.Height;
                }
                else Resize.Width = Region.Width;
            }
            else
            {
                if (Region.Width < Resize.Width)
                {
                    Resize.Width = Region.Width;
                    Resize.Height = Region.Width * Image.Height / Image.Width;
                }
                else Resize.Height = Region.Height;
            }
            if (Image.Width < Region.Width && Image.Height < Region.Height)
            {
                Resize.Width = Image.Width;
                Resize.Height = Image.Height;
            }
            return Resize;
        }


        public bool GetThumbNail(string filePath, string savePath, Size new_size, ImageFormat Format)
        {
            Size Resize;
            Image o_image;
            Image o_newImage;
            try
            {
                o_image = Image.FromFile(filePath);
                Resize = GetImageDimensions(o_image.Size, new_size);
                o_newImage = o_image.GetThumbnailImage(Resize.Width, Resize.Height, null, IntPtr.Zero);
                o_newImage.Save(savePath, ImageFormat.Jpeg);
                o_image.Dispose();
                o_newImage.Dispose();
                return true;
            }
            catch (Exception exp)
            {
                Message = exp.Message;
                CheckRiseExcepttion(exp);
                return false;
            }
        }

        private void CheckRiseExcepttion(Exception exp)
        {
            if (RiseException)
                throw exp;
        }

        public void ResizeImage(ImageModel image, bool crop)
        {
            var server = HttpContext.Current.Server;
            var fileName = Path.GetFileName(image.Src);
            var ext = Path.GetExtension(fileName);
            var savepath = Path.GetDirectoryName(image.Src) + "/";
            if (image.Sizes != null)
            {
                foreach (var size in image.Sizes)
                {
                    var newName = fileName.Replace(ext, "_size(" + size.Width + "x" + size.Height + ")" + ext);
                    if (crop)
                    {
                        CropAndResizeImage(image.Src, savepath + newName, size, ImageFormat.Jpeg, false);
                    }
                    else
                    {
                        ResizeHighQuality(image.Src, savepath, newName, size, ImageFormat.Jpeg);
                    }
                }
            }

        }

        public void ResizeHighQuality(string filePath, string savePath, string NewName, Size Region, ImageFormat Format)
        {

            try
            {
                Size Resize;
                Image src_image = Image.FromFile(filePath);
                Resize = GetImageDimensions(src_image.Size, Region);
                Bitmap bitmap = new Bitmap(src_image, Resize);
                Graphics new_g = Graphics.FromImage(bitmap);
                new_g.SmoothingMode = SmoothingMode.HighQuality;
                new_g.InterpolationMode = InterpolationMode.Default;
                new_g.CompositingMode = CompositingMode.SourceCopy;
                new_g.DrawImage(src_image, 0, 0, bitmap.Width, bitmap.Height);
                src_image.Dispose();
                bitmap.Save(savePath + NewName, ImageFormat.Jpeg);
                bitmap.Dispose();
                new_g.Dispose();
            }
            catch (Exception exp)
            {
                CheckRiseExcepttion(exp);

            }
        }

        public string CropAndResizeImage(string filePath, string savePath, Size size, ImageFormat format, bool waterMark)
        {
            return CropAndResizeImage(File.ReadAllBytes(HttpContext.Current.Server.MapPath(filePath)), savePath, size, format, waterMark);
        }

        public string CropAndResizeImage(HttpPostedFileBase postedFile, string savePath, Size size, ImageFormat format, bool waterMark)
        {
            using (Stream inputStream = postedFile.InputStream)
            {
                var memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                return CropAndResizeImage(memoryStream.ToArray(), savePath, size, format, waterMark);
            }
        }

        public string CropAndResizeImage(byte[] originalBytes, string savePath, Size size, ImageFormat format, bool waterMark)
        {
            using (var streamOriginal = new MemoryStream(originalBytes))
            using (var imgOriginal = Image.FromStream(streamOriginal))
            {
                //get original width and height of the incoming image
                var originalWidth = imgOriginal.Width; // 1000
                var originalHeight = imgOriginal.Height; // 800

                //get the percentage difference in size of the dimension that will change the least
                var percWidth = ((float)size.Width / (float)originalWidth); // 0.2
                var percHeight = ((float)size.Height / (float)originalHeight); // 0.25
                var percentage = Math.Max(percHeight, percWidth); // 0.25

                //get the ideal width and height for the resize (to the next whole number)
                var width = (int)Math.Max(originalWidth * percentage, size.Width); // 250
                var height = (int)Math.Max(originalHeight * percentage, size.Height); // 200

                //actually resize it
                using (var resizedBmp = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage((Image)resizedBmp))
                    {
                        graphics.InterpolationMode = InterpolationMode.Default;
                        graphics.DrawImage(imgOriginal, 0, 0, width, height);
                    }

                    //work out the coordinates of the top left pixel for cropping
                    var x = (width - size.Width) / 2; // 25
                    var y = (height - size.Height) / 2; // 0

                    //create the cropping rectangle
                    var rectangle = new Rectangle(x, y, size.Width, size.Height); // 25, 0, 200, 200

                    //crop
                    using (var croppedBmp = resizedBmp.Clone(rectangle, resizedBmp.PixelFormat))
                    using (var ms = new MemoryStream())
                    {
                        //watermark
                        if (waterMark)
                        {
                            var mark = Image.FromFile(HttpContext.Current.Server.MapPath("~/Upload/Watermark/watermark.png"));
                            if (mark != null && croppedBmp.Width > mark.Width && croppedBmp.Height > mark.Height)
                            {
                                using (var graphics = Graphics.FromImage((Image)croppedBmp))
                                {
                                    graphics.InterpolationMode = InterpolationMode.Default;
                                    graphics.DrawImage(mark, croppedBmp.Width - mark.Width - 15, croppedBmp.Height - mark.Height - 10, mark.Width, mark.Height);
                                }
                            }
                        }
                        //get the codec needed
                        var imgCodec = getFormat(ImageCodecInfo.GetImageEncoders(), format.Guid);

                        //make a paramater to adjust quality
                        var codecParams = new EncoderParameters(1);

                        //reduce to quality of 80 (from range of 0 (max compression) to 100 (no compression))
                        codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                        //save to the memorystream - convert it to an array and send it back as a byte[]
                        croppedBmp.Save(ms, imgCodec, codecParams);
                        var res = ms.ToArray();
                        var absSavePath = (savePath.StartsWith("/") || savePath.StartsWith("~/")) ? HttpContext.Current.Server.MapPath(savePath) : savePath;
                        File.WriteAllBytes(absSavePath, res);
                        return savePath;
                    }
                }
            }
        }

        public ImageCodecInfo getFormat(ImageCodecInfo[] info, Guid guid)
        {
            foreach (var item in info)
            {
                if (item.FormatID == guid) return item;
            }
            return null;
        }


        public bool DeleteSourceFile(string filePath)
        {
            try
            {
                System.IO.File.Delete(filePath);
                return true;
            }
            catch (Exception exp)
            {
                Message = exp.Message;
                CheckRiseExcepttion(exp);
                return false;
            }
        }

        public static bool IsExistFile(string Path)
        {
            bool result = false;
            if (Path != string.Empty)
            {
                Path = Path.Replace("../", "");
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(Path)))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}