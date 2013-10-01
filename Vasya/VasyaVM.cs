using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vasya.Utilities;

namespace Vasya
{
    public class VasyaVM : VM
    {
        private readonly Logic _logic;

        private BitmapSource _newTopoFilteredImage;
        private BitmapSource _originalTopoFilteredImage;
        private bool _originalTopoSelected;
        private double _value;
        private bool _valueChanged;

        public VasyaVM(Logic logic)
        {
            _logic = logic;
            Value = (MinValue + MaxValue)/2;
        }

        public BitmapSource NewTopoFilteredImage
        {
            get { return _newTopoFilteredImage; }
            set
            {
                _newTopoFilteredImage = value;
                NotifyPropertyChanged("NewTopoFilteredImage");
            }
        }

        public BitmapSource OriginalTopoFilteredImage
        {
            get { return _originalTopoFilteredImage; }
            set
            {
                _originalTopoFilteredImage = value;
                NotifyPropertyChanged("OriginalTopoFilteredImage");
            }
        }

        public bool OriginalTopoSelected
        {
            get { return _originalTopoSelected; }
            set
            {
                _originalTopoSelected = value;
                if (value && _valueChanged)
                {
                    OriginalTopoFilteredImage = _logic.OriginalImageWithFilteredPoints(Value);
                    _valueChanged = false;
                }
                NotifyPropertyChanged("OriginalTopoSelected");
            }
        }

        public double MinValue
        {
            get { return _logic.MinValue; }
        }

        public double MaxValue
        {
            get { return _logic.MaxValue; }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
                _valueChanged = true;
                //NewTopoFilteredImage = ImageCreator.CreateBitmap(_logic.FilteredImage(value), _logic.ActualImageSize, _logic.ActualImageSize, PixelFormats.BlackWhite);
                NewTopoFilteredImage = _logic.FilteredImage(Value);
            }
        }
    }
}