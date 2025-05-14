using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Model;

namespace ViewModel
{
    public class PoolTableViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly PoolTable _poolTable;
        private string _ballCountInput = "50";
        private bool _isRunning;
        private Dictionary<int, BallViewModel> _ballViewModelMap = new();
        private readonly Stopwatch _stopwatch = new();
        private double _lastFrameTime;
        private readonly DispatcherTimer _renderTimer;
        private bool _isDisposed;

        public ObservableCollection<BallViewModel> Balls { get; } = new();

        public string BallCountInput
        {
            get => _ballCountInput;
            set
            {
                _ballCountInput = value;
                OnPropertyChanged();
                ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public bool CanStart => !_isRunning && int.TryParse(BallCountInput, out int count) && count > 0;
        public bool CanStop => _isRunning;
        
        public PoolTableViewModel(PoolTable poolTable)
        {
            _poolTable = poolTable;
            StartCommand = new RelayCommand(ExecuteStart, () => CanStart);
            StopCommand = new RelayCommand(ExecuteStop, () => CanStop);
            
            _renderTimer = new DispatcherTimer();
            _renderTimer.Interval = TimeSpan.FromMilliseconds(16);
            _renderTimer.Tick += (s, e) => {
                if (Balls.Count > 0)
                {
                    OnPropertyChanged(nameof(Balls));
                }
            };
            _renderTimer.Start();
            
            _stopwatch.Start();
            _lastFrameTime = _stopwatch.Elapsed.TotalSeconds;
            CompositionTarget.Rendering += OnRendering;
        }

        private async void ExecuteStart()
        {
            if (!int.TryParse(BallCountInput, out int ballCount) || ballCount <= 0)
                return;

            _isRunning = true;
            
            Balls.Clear();
            _ballViewModelMap.Clear();
            await _poolTable.BallManager.RemoveAllBalls();

            List<Task> ballCreationTasks = new List<Task>();
            for (int i = 0; i < ballCount; i++)
            {
                ballCreationTasks.Add(_poolTable.AddBall());
            }
            await Task.WhenAll(ballCreationTasks);
            
            var ballModels = await _poolTable.GetBallModels();

            foreach (BallModel ballModel in ballModels) 
            {
                var ballViewModel = new BallViewModel(ballModel);
                Balls.Add(ballViewModel);
                _ballViewModelMap.Add(ballViewModel.Id, ballViewModel);
            }

            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanStop));
            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopCommand).RaiseCanExecuteChanged();
        }

        private async void ExecuteStop()
        {
            _isRunning = false;
            Balls.Clear();
            _ballViewModelMap.Clear();
            
            await _poolTable.BallManager.RemoveAllBalls();

            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanStop));
            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopCommand).RaiseCanExecuteChanged();
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            double currentTime = _stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - _lastFrameTime;
            _lastFrameTime = currentTime;

            if (deltaTime > 0.1)
            {
                deltaTime = 0.1;
            }

            Update(deltaTime);
        }

        private async void Update(double deltaTime)
        {
            if (!_isRunning) return;
            
            await _poolTable.Update(deltaTime);
            
            var logicBalls = await _poolTable.BallManager.GetBalls();
            if (logicBalls != null)
            {
                foreach (var logicBall in logicBalls)
                {
                    if (_ballViewModelMap.TryGetValue(logicBall.Id, out var ballViewModel))
                    {
                        ballViewModel.Update(logicBall);
                    }
                }
            }

        }

        public void Dispose()
        {
            if (_isDisposed) return;
            
            CompositionTarget.Rendering -= OnRendering;
            _renderTimer.Stop();
            _stopwatch.Stop();
            _isDisposed = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}