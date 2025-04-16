using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data;

namespace ViewModel
{
    public class BallViewModel : INotifyPropertyChanged
    {
        private double _positionX;
        private double _positionY;
        private readonly double _ballRadius;

        public int Id { get; }

        private double PositionX
        {
            get => _positionX;
            set
            {
                _positionX = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanvasLeft));
            }
        }

        private double PositionY
        {
            get => _positionY;
            set
            {
                _positionY = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanvasTop));
            }
        }

        public double CanvasLeft => PositionX - BallRadius;
        public double CanvasTop => PositionY - BallRadius;
        
        private double BallRadius => _ballRadius;
        public double Diameter => _ballRadius * 2;

        public BallViewModel(IBall ball)
        {
            Id = ball.Id;
            _ballRadius = ball.BallRadius;
            _positionX = ball.PositionX;
            _positionY = ball.PositionY;
        }

        public void Update(IBall ball)
        {
            PositionX = ball.PositionX;
            PositionY = ball.PositionY;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}