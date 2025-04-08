using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Xml.XPath;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.LightBurn
{
    public class Plugin : IViewer
    {
        public int Priority => 0;

        public void Init()
        {
        }

        public bool CanHandle(string path)
        {
            return !Directory.Exists(path) && path.ToLower().EndsWith(".lbrn2");
        }

        public void Prepare(string path, ContextObject context)
        {
            context.PreferredSize = new Size {Width = 600, Height = 400};
        }

        public void View(string path, ContextObject context)
        {
            try
            {
                var xml = XDocument.Load(path);
                var thumbnailBase64 = xml.XPathSelectElement("LightBurnProject/Thumbnail")?.Attribute("Source").Value;
                if (!string.IsNullOrEmpty(thumbnailBase64))
                {
                    try
                    {
                        var imageBytes = Convert.FromBase64String(thumbnailBase64);
                        using (var stream = new MemoryStream(imageBytes))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();

                            var image = new Image { Source = bitmap };
                            context.ViewerContent = image;
                            context.Title = $"{Path.GetFileName(path)}";
                        }
                    }
                    catch (FormatException)
                    {
                        context.ViewerContent = new Label { Content = "Error: Thumbnail data is not a valid Base64 string." };
                    }
                    catch (Exception ex)
                    {
                        context.ViewerContent = new Label { Content = $"Error rendering thumbnail: {ex.Message}" };
                    }
                }
                else
                {
                    context.ViewerContent = new Label { Content = $"Error: No thumbnail data found in the file. {path}" };
                }
            }
            catch (Exception ex)
            {
                context.ViewerContent = new Label { Content = $"Error loading file: {ex.Message}" };
            }
            finally
            {
                context.IsBusy = false;
            }
        }

        public void Cleanup()
        {
        }
    }
}