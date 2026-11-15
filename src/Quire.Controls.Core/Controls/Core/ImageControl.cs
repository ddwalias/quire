#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Win32;
using Quire.Controls.Core.Whiteboard;
using Quire.IO;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Services;
using Quire.Wpf;
using Quire.Xml;

namespace Quire.Controls.Core;

public partial class ImageControl
{
    private static readonly Type _typeOfThis;

    private static ImageControlDictionary _dictionary;

    private Uri _originalImageUri;

    public static readonly DependencyProperty SourceProperty;

    public static readonly RoutedEvent ChangedEvent;

    public static readonly RoutedCommand BrowseForImageCommand;

    public byte[] OriginalStreamHash { get; private set; }

    private static void HandleSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ImageControl imageControl = d as ImageControl;
        if (imageControl != null)
        {
            if (imageControl.BitmapImage == null && DebugTools.DevAssert(imageControl.OriginalImageUri == null).Passed)
            {
                imageControl.OriginalImageUri = new Uri(e.NewValue.ToString());
            }
            imageControl.FireChanged();
            if (DebugTools.AssertNotNull(imageControl.Source).Passed)
            {
                DoubleComparer defaultDoubleComparer = DoubleComparer.DefaultDoubleComparer;
                if ((defaultDoubleComparer.Compare(imageControl.Width, imageControl.Source.Width) != 0 || defaultDoubleComparer.Compare(imageControl.Height, imageControl.Source.Height) != 0) && DebugTools.DevAssert(double.IsNaN(imageControl.Width) && double.IsNaN(imageControl.Height)).Passed)
                {
                    imageControl.Width = imageControl.Source.Width;
                    imageControl.Height = imageControl.Source.Height;
                }
                imageControl.FireRequestResizeToFit();
            }
        }
    }

    private void FireRequestResizeToFit()
    {
        if (RequestResizeToFitOrClearRestriction != null)
        {
            RequestResizeToFitOrClearRestriction(this);
        }
    }

    private void FireChanged()
    {
        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    static ImageControl()
    {
        _typeOfThis = typeof(ImageControl);
        _dictionary = ImageControlDictionary.Instance;
        BrowseForImageCommand = new RoutedCommand("BrowseForImage", typeof(ImageControl));
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        SourceProperty = System.Windows.Controls.Image.SourceProperty.AddOwner(typeof(ImageControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HandleSourcePropertyChanged));
        ChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(typeof(ImageControl));
    }

    public ImageControl()
    {
        CommandBinding commandBinding = new CommandBinding(BrowseForImageCommand, BrowseForImage);
        CommandBindings.Add(commandBinding);
    }

    public static ImageControl CreateImageControl(bool isNewImageControl = true)
    {
        if (!isNewImageControl)
        {
            return new ImageControl();
        }
        ImageControl imageControl = new ImageControl();
        imageControl.BrowseForImage();
        if (!(imageControl.OriginalImageUri == null))
        {
            return imageControl;
        }
        return null;
    }

    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    [DefaultValue((string)null)]
    public Uri OriginalImageUri
    {
        get => _originalImageUri;
        set
        {
            try
            {
                byte[] streamHash;
                BitmapSource bitmapSource = GetBitmapSourceFromUri(value, out streamHash);
                _originalImageUri = value;
                OriginalStreamHash = streamHash;
                Source = bitmapSource;
            }
            catch (Exception)
            {
            }
        }
    }

    private static BitmapSource GetBitmapSourceFromUri(Uri sourceUri, out byte[] streamHash)
    {
        BitmapSource bmImage = null;
        byte[] localStreamHash = null;
        Action<Stream> action = (Stream stream) =>
        {
            bmImage = ImageUtils.GetBitmapImage(stream);
            localStreamHash = StreamUtils.GetStreamHash(stream);
        };
        if (sourceUri != null)
        {
            if (sourceUri.IsFile)
            {
                using (FileStream fileStream = File.Open(sourceUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    action(fileStream);
                }
            }
            else if (sourceUri.Scheme == PackUriHelper.UriSchemePack)
            {
                Uri packageUri = PackUriHelper.GetPackageUri(sourceUri);
                Uri partUri = PackUriHelper.GetPartUri(sourceUri);
                using (FileStream fileStream2 = PackageOperationsProvider.OpenFileStreamForReadWithFullShare(packageUri.LocalPath))
                {
                    using (Package package = Package.Open(fileStream2))
                    {
                        PackagePart part = package.GetPart(partUri);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (Stream stream2 = part.GetStream())
                            {
                                stream2.CopyTo(memoryStream);
                                memoryStream.Position = 0L;
                                action(memoryStream);
                            }
                        }
                    }
                }
            }
            else
            {
                DebugTools.DevAssert(false);
            }
        }
        streamHash = localStreamHash;
        return bmImage;
    }

    public static string GetImageExtension(Uri imageUri)
    {
        if (!(imageUri == null))
        {
            return Path.GetExtension(imageUri.LocalPath).Substring(1);
        }
        return xmlWorksheetNodeNames.defaultImageExtention;
    }

    public string GetImageExtension() => GetImageExtension(_originalImageUri);

    public bool ShouldSerializeSource()
    {
        bool flag = Source != null;
        if (flag)
        {
            flag = Source.GetType().IsPublic;
        }
        if (flag && SourceIsBitmapImage())
        {
            flag = null != BitmapImage.UriSource;
        }
        return flag;
    }

    private bool SourceIsBitmapImage() => Source is BitmapImage;

    private BitmapImage BitmapImage => Source as BitmapImage;

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        Focus();
        e.Handled = true;
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        OnGotFocusEvent(this);
    }

    private ImageControl _boundTo;

    [DefaultValue((string)null)]
    public ImageControl BoundTo
    {
        get => _boundTo;
        set
        {
            if (_boundTo != null)
            {
                BindingOperations.ClearBinding(this, SourceProperty);
            }
            _boundTo = value;
            while (_boundTo.BoundTo != null)
            {
                _boundTo = _boundTo.BoundTo;
            }
            if (_boundTo != null)
            {
                Binding binding = new Binding
                {
                    Source = _boundTo,
                    Path = new PropertyPath(SourceProperty)
                };
                BindingOperations.SetBinding(this, SourceProperty, binding);
            }
        }
    }

    public void BrowseForImage(object sender, ExecutedRoutedEventArgs args)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = _dictionary["BrowseForImageFileOpenDialogTitle"] as string,
            Filter = _dictionary["BrowseForImageFileOpenDialogFilter"] as string
        };
        if (openFileDialog.ShowDialog().GetValueOrDefault())
        {
            string fileName = openFileDialog.FileName;
            OriginalImageUri = new Uri(fileName);
        }
    }

    public void BrowseForImage()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = _dictionary["BrowseForImageFileOpenDialogTitle"] as string,
            Filter = _dictionary["BrowseForImageFileOpenDialogFilter"] as string
        };
        if (openFileDialog.ShowDialog().GetValueOrDefault())
        {
            string fileName = openFileDialog.FileName;
            OriginalImageUri = new Uri(fileName);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event Action<IWhiteboardItem> OnGotFocusEvent = delegate { };

    public event Action<IWhiteboardItem, Direction> RequestRemove;

    public double VerticalAlignmentOffset => 0.0;

    public bool IsEmpty => false;

    public void Activate(bool isActive)
    {
        if (isActive)
        {
            AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLoad(this, () => Focus());
        }
    }

    public Vector? NavigateIntoItem(Direction direction)
    {
        Activate(true);
        return null;
    }

    public void SimulateMouseClick(Point location, int numberOfClicks) => Activate(true);

    public object Clone()
    {
        ImageControl imageControl = new ImageControl
        {
            Source = Source,
            Width = Width,
            Height = Height
        };
        if (WhiteboardDependencyProperties.GetIsRegionBorderVisible(this))
        {
            WhiteboardDependencyProperties.SetIsRegionBorderVisible(imageControl, true);
        }
        return imageControl;
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        if (data is pictureType pictureType2)
        {
            if (pictureType2.InnerXml != null)
            {
                XmlNode xmlNode = pictureType2.InnerXml[0];
                if (xmlNode != null)
                {
                    string attribute = ((XmlElement)xmlNode).GetAttribute("item-idref");
                    Uri targetUri = d11NProvider.PackagePart.GetRelationship(attribute).TargetUri;
                    Uri originalImageUri = PackUriHelper.Create(new Uri(Path.GetFullPath(d11NProvider.FilePath), UriKind.Absolute), targetUri);
                    OriginalImageUri = originalImageUri;
                }
                return null;
            }
        }
        else
        {
            throw new ArgumentException("data should be of pictureType");
        }
        return null;
    }

    private string GetImageRelationshipId(IS11NProvider s11NProvider)
    {
        ImagePackagingInformation imagePackagingInformation = new ImagePackagingInformation
        {
            ImageExtension = GetImageExtension().ToLower(),
            ImageSourceUri = OriginalImageUri,
            ImageOriginalStreamHash = OriginalStreamHash,
            Image = Source as BitmapSource,
            ImageId = s11NProvider.Helper.GetRegionId(this).ToString(),
            ImageNameFormat = "Image{0}"
        };
        return s11NProvider.PackImage(imagePackagingInformation);
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        XmlElement xmlElement = null;
        string text;
        if (Source != null)
        {
            text = GetImageExtension().ToLower();
            if (!Enum.GetNames(typeof(ItemChoiceType)).Contains(text))
            {
                text = ItemChoiceType.unknown.ToString();
            }
            xmlElement = s11NProvider.XmlContentDocument.CreateElement(text, s11NProvider.NameSpaceManager.LookupNamespace(string.Empty));
            xmlElement.SetAttribute("item-idref", GetImageRelationshipId(s11NProvider));
            if (!double.IsNaN(Width))
            {
                xmlElement.SetAttribute("display-width", Width.ToString(CultureInfo.InvariantCulture));
            }
            if (!double.IsNaN(Height))
            {
                xmlElement.SetAttribute("display-height", Height.ToString(CultureInfo.InvariantCulture));
            }
        }
        return new pictureType
        {
            InnerXml = new XmlNode[1] { xmlElement }
        };
    }

    public bool PreserveAspectRatio => true;

    public SubElementToResizeData SubElementToResizeData => null;

    public ResizeToFitState ResizeState
    {
        get
        {
            if (Source != null)
            {
                return ResizeToFitState.ResizingEnabled;
            }
            return ResizeToFitState.ResizingNotInitialized;
        }
    }

    public event Action<IResizeToFitWhiteboardItem> RequestResizeToFitOrClearRestriction;

    public string GetHelpContext() => "ImageControl";
}
