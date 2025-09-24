using databinding0.Models;
using NCalc;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace databinding0.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private double _intervalStart;
        private double _intervalEnd;
        private double _precision;
        private string _function;
        private string _resultText;
        private int _decimalPlaces;
        private PlotModel _plotModel;

        public double IntervalStart
        {
            get => _intervalStart;
            set { _intervalStart = value; OnPropertyChanged(); }
        }

        public double IntervalEnd
        {
            get => _intervalEnd;
            set { _intervalEnd = value; OnPropertyChanged(); }
        }

        public double Precision
        {
            get => _precision;
            set { _precision = value; OnPropertyChanged(); }
        }

        public string Function
        {
            get => _function;
            set { _function = value; OnPropertyChanged(); }
        }

        public string ResultText
        {
            get => _resultText;
            set { _resultText = value; OnPropertyChanged(); }
        }

        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set { _decimalPlaces = value; OnPropertyChanged(); }
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            set { _plotModel = value; OnPropertyChanged(); }
        }

        public ICommand FindRootCommand { get; }
        public ICommand PlotCommand { get; }

        public MainViewModel()
        {
            IntervalStart = -5;
            IntervalEnd = 5;
            Precision = 0.01;
            Function = "x^2 - 4";
            DecimalPlaces = 3;

            FindRootCommand = new RelayCommand(_ => FindRoot());
            PlotCommand = new RelayCommand(_ => PlotFunction());
        }

        private double Truncate(double value, int decimals)
        {
            double factor = Math.Pow(10, decimals);
            return Math.Truncate(value * factor) / factor;
        }

        private void FindRoot()
        {
            try
            {
                var solver = new DichotomyMethod(Function);
                double root = solver.Solve(IntervalStart, IntervalEnd, Precision, out int iterations);

                root = Truncate(root, DecimalPlaces);

                ResultText = $"Найден корень: x ≈ {root}";
                // IterationsText = $"Количество итераций: {iterations}";
            }
            catch (Exception ex)
            {
                ResultText = "Ошибка: " + ex.Message;
            }
        }

        private void PlotFunction()
        {
            try
            {
                var evaluator = new FunctionEvaluator(Function);

                var model = new PlotModel { Title = $"График функции: {Function}" };
                var series = new LineSeries { MarkerType = MarkerType.None };

                for (double x = IntervalStart; x <= IntervalEnd; x += (IntervalEnd - IntervalStart) / 100.0)
                {
                    series.Points.Add(new DataPoint(x, evaluator.Evaluate(x)));
                }

                model.Series.Add(series);
                PlotModel = model;
            }
            catch (Exception ex)
            {
                ResultText = "Ошибка при построении графика: " + ex.Message;
            }
        }

        public string this[string columnName]
        {
            get
            {
                try
                {
                    switch (columnName)
                    {
                        case nameof(IntervalStart):
                            return double.IsNaN(IntervalStart) ? "Неверное число" : null;
                        case nameof(IntervalEnd):
                            return double.IsNaN(IntervalEnd) ? "Неверное число" : null;
                        case nameof(Precision):
                            return Precision <= 0 ? "Точность должна быть > 0" : null;
                        case nameof(DecimalPlaces):
                            return DecimalPlaces < 0 ? "Знаков после запятой не может быть меньше 0" : null;
                        case nameof(Function):
                            if (string.IsNullOrWhiteSpace(Function))
                                return "Введите функцию";
                            try
                            {
                                var evaluator = new FunctionEvaluator(Function);
                                evaluator.Evaluate(0); // проверка хотя бы одной точки
                                return null;
                            }
                            catch
                            {
                                return "Некорректная функция";
                            }
                    }
                }
                catch
                {
                    return "Ошибка ввода";
                }
                return null;
            }
        }

        public string Error => null;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Утилита для ICommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
