using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data;
using Model;

namespace ViewModel
{
    public class BallViewModel : INotifyPropertyChanged
    {
        private double _positionX;
        private double _positionY;
        private readonly double _ballRadius;

        public int Id { get; }
        public string Color { get; }

        public double PositionX
        {
            get => _positionX;
            set
            {
                _positionX = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanvasLeft));
            }
        }

        public double PositionY
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

        public double Diameter
        {
            get => _ballRadius * 2;
        }

        public BallViewModel(IBall ball)
        {
            Id = ball.Id;
            _ballRadius = ball.BallRadius;
            _positionX = ball.PositionX;
            _positionY = ball.PositionY;
            Color = ball.Color;
        }

        public BallViewModel(BallModel model)
        {
            Id = model.Id;
            _ballRadius = model.BallRadius;
            PositionX = model.PositionX;
            PositionY = model.PositionY;
            Color = model.Color;
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