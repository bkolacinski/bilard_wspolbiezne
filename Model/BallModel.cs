using System.ComponentModel;
using System.Numerics;
using Data;

namespace Model
{
    public class BallModel : IBall, INotifyPropertyChanged
    {
        private int _id;
        private double _positionX;
        private double _positionY;
        private Vector2 _velocity;
        private double _ballRadius;
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public double PositionX
        {
            get => _positionX;
            set
            {
                _positionX = value;
                OnPropertyChanged(nameof(PositionX));
            }
        }

        public double PositionY
        {
            get => _positionY;
            set
            {
                _positionY = value;
                OnPropertyChanged(nameof(PositionY));
            }
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                OnPropertyChanged(nameof(Velocity));
            }
        }

        public double BallRadius
        {
            get => _ballRadius;
            set
            {
                _ballRadius = value;
                OnPropertyChanged(nameof(BallRadius));
            }
        }
        
        public void Move(double deltaTime)
        {
            PositionX += Velocity.X * deltaTime;
            PositionY += Velocity.Y * deltaTime;
        }
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

