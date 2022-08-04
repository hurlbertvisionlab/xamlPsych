using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class ImagesDirectorySource : StudyDataSource, IStudyPreloadable
    {
        public static readonly DependencyProperty DirectoryProperty = DependencyProperty.Register(nameof(Directory), typeof(string), typeof(ImagesDirectorySource), new PropertyMetadata("."));
        public static readonly DependencyProperty FilesPatternProperty = DependencyProperty.Register(nameof(FilesPattern), typeof(string), typeof(ImagesDirectorySource), new PropertyMetadata("*"));
        public static readonly DependencyProperty FilesProperty = DependencyProperty.Register(nameof(Files), typeof(TokenStringCollection), typeof(ImagesDirectorySource));

        public TokenStringCollection Files
        {
            get { return (TokenStringCollection)GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }

        public string FilesPattern
        {
            get { return (string)GetValue(FilesPatternProperty); }
            set { SetValue(FilesPatternProperty, value); }
        }

        public string Directory
        {
            get { return (string)GetValue(DirectoryProperty); }
            set { SetValue(DirectoryProperty, value); }
        }

        public ImagesDirectorySource()
        {
            Files = new TokenStringCollection();
        }

        public bool Preload { get; set; }

        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            IEnumerable<string> files = _itemsPaths ?? GenerateItemPaths(context);

            foreach (string file in files)
                if (_itemsCache.TryGetValue(file, out ImageItem image))
                    yield return image;
                else
                    yield return GetImageItem(file);
        }
        protected string[] GenerateItemPaths(StudyContext context)
        {
            string[] files;
            string resolved = System.IO.Path.GetFullPath(Directory);
            try
            {
                if (Files?.Count > 0)
                {
                    files = new string[Files.Count];
                    for (int i = 0; i < files.Length; i++)
                        files[i] = Path.Combine(resolved, Files[i]);
                }
                else
                    files = System.IO.Directory.GetFiles(resolved, FilesPattern);
            }
            catch (ArgumentException e)
            {
                throw new StudyException(context, $"The '{nameof(Directory)}' property of '{nameof(ImagesDirectorySource)}' is either invalid or missing. Please specify valid source directory.", e, this);
            }
            catch (DirectoryNotFoundException e)
            {
                string message = resolved == Directory ? $"The source directory of '{resolved}'" : $"The source directory of '{Directory}' has been resolved to '{System.IO.Path.GetFullPath(Directory)}' which";
                throw new StudyException(context, message + " does not exist. Could you have forgotton to include the files or misspelled the path?", e, this);
            }
            catch (IOException e)
            {
                throw new StudyException(context, $"A system or hardware error occured when trying to list images in the '{resolved}' directory.", e, this);
            }

            return files;
        }

        public virtual ImageItem GetImageItem(string file)
        {
            return new ImageItem
            {
                Source = file,
                Name = Path.GetFileName(file),
                Image = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute)),
            };
        }

        public override int? GetItemsCount(StudyContext context)
        {
            return System.IO.Directory.GetFiles(Directory).Length;
        }

        private IEnumerable<string> _itemsPaths;
        private Dictionary<string, ImageItem> _itemsCache = new Dictionary<string, ImageItem>(StringComparer.OrdinalIgnoreCase);

        public async Task DoPreload(StudyContext context, IProgress<string> progress, CancellationToken cancellationToken)
        {
            progress.Report("Enumerating files...");
            _itemsPaths = GenerateItemPaths(context);

            foreach (string file in _itemsPaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress.Report(Path.GetFileName(file));
                await Task.Run(() => context.Dispatcher.Invoke(() => _itemsCache[file] = GetImageItem(file), System.Windows.Threading.DispatcherPriority.ApplicationIdle));
            }
        }
    }
}
